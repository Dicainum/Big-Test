# ECS-гайд: Morpeh + FeatureRunner

Как устроена ECS в этом проекте и как добавлять в неё новый код.

---

## Главная идея

В ECS данные и логика разделены:

- **сущность (entity)** — просто айдишник, к которому прицеплены компоненты;
- **компонент** — данные без логики (структура с полями);
- **система** — логика без данных (класс, который каждый кадр обходит нужные сущности);
- **фильтр** — список сущностей с нужным набором компонентов;
- **стеш (stash)** — хранилище всех компонентов одного типа, из него достают данные конкретной сущности.

Пример: у куба есть компонент `Gravity` с вектором и силой, а `GravitySystem` каждый физический кадр берёт все сущности с `Gravity` и толкает их `Rigidbody`.

---

## Из чего состоит фича

| Часть | Зачем | Где лежит |
|---|---|---|
| Компонент | хранит данные | `Features/<Feature>/Components/` |
| Система | делает работу | `Features/<Feature>/Systems/` |
| Провайдер | вешает компонент на объект сцены | `Features/<Feature>/Providers/` |
| FeatureRunner | запускает системы, один на всю игру | `Assets/Scripts/FeatureRunners/` |

```
Assets/Scripts/
    FeatureRunners/
        FeatureRunner.cs
    Features/
        Gravity/
            Components/Gravity.cs
            Providers/GravityProvider.cs
            Systems/GravitySystem.cs
```

---

## Шаг 1. Компонент

```csharp
using System;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.Gravity.Components
{
    [Serializable]
    public struct Gravity : IComponent
    {
        public Vector3 Direction;
        public float Force;
    }
}
```

- всегда `struct` и `IComponent`, никогда `class`;
- `[Serializable]` обязателен, иначе значения не сохранятся и не будут видны в инспекторе;
- только публичные поля. Никаких методов и свойств — вся логика живёт в системе;
- компонент без полей тоже нормален: это метка, по которой можно фильтровать.

## Шаг 2. Система

```csharp
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.Gravity.Systems
{
    public sealed class GravitySystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        private Stash<Components.Gravity> _gravity;

        public void OnAwake()
        {
            _gravity = World.GetStash<Components.Gravity>();
            _filter = World.Filter
                .With<Components.Gravity>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref var gravity = ref _gravity.Get(entity);
                // ...
            }
        }

        public void Dispose()
        {
            _filter.Dispose();
        }
    }
}
```

Скелет всегда одинаковый:

- `OnAwake` — один раз собрать фильтр и стеши;
- `OnUpdate` — пройтись по фильтру и сделать работу;
- `Dispose` — закрыть все фильтры, которые создали.

Что важно не забыть:

- `public World World { get; set; }` — именно с сеттером, движок сам его заполнит;
- фильтр и стеш собираем в `OnAwake`, а не каждый кадр в `OnUpdate`;
- пишем `ref var gravity = ref _gravity.Get(entity)`. Без `ref` вы измените копию, и изменения пропадут;
- время берём из аргумента `deltaTime`, а не из `Time.deltaTime`;
- система наследует только `ISystem`. `IFixedSystem` и `ILateSystem` не нужны — где крутиться, решает шаг 3.

## Шаг 3. Включить систему

Всё, что работает в игре, перечислено в трёх списках в `FeatureRunner.cs`. Добавить систему — это одна строка:

```csharp
private static IEnumerable<ISystem> UpdateSystems()
{
    yield break;
}

private static IEnumerable<ISystem> FixedUpdateSystems()
{
    yield return new GravitySystem();
}

private static IEnumerable<ISystem> LateUpdateSystems()
{
    yield break;
}
```

Какой список выбрать:

- `FixedUpdateSystems` — физика: силы, скорости, `Rigidbody`;
- `UpdateSystems` — ввод, геймплей, таймеры;
- `LateUpdateSystems` — камера и всё, что должно видеть уже готовый кадр.

Список и решает, когда система тикает. Порядок выполнения — сверху вниз по списку. На сцене должен быть ровно один `FeatureRunner`.

## Шаг 4. Повесить компонент на объект

Чтобы объект сцены попал в ECS, ему нужен провайдер. Обычно это пустой класс:

```csharp
using Scellecs.Morpeh.Providers;
using UnityEngine;

namespace Features.Gravity.Providers
{
    [AddComponentMenu("ECS/Gravity/" + nameof(GravityProvider))]
    public sealed class GravityProvider : MonoProvider<Components.Gravity>
    {
    }
}
```

Дальше на объекте: `Add Component → ECS → Gravity → GravityProvider`. Провайдер сам создаст сущность и положит на неё компонент со значениями из инспектора.

Если нужно подставить ссылку, которой нет в инспекторе (например `Rigidbody` этого же объекта), переопределите `Initialize`:

```csharp
protected override void Initialize()
{
    ref var gravity = ref GetData();
    gravity.Rigidbody = GetComponent<Rigidbody>();
}
```

Помните: пока объекту нужна фича, провайдер должен быть включён. Выключили провайдер — компонент снялся с сущности.

Для объектов, которые создаются в рантайме пачками, провайдер можно не вешать, а создать сущность из кода:

```csharp
var world = World.Default;
var entity = world.CreateEntity();
world.GetStash<Gravity>().Set(entity, new Gravity { Direction = Vector3.down, Force = 9.81f });
```

---

## Смотреть и править данные на ходу

`Tools → Morpeh → WorldBrowser` — там видно все сущности, их компоненты, и значения можно менять прямо во время игры.

Чтобы правка сразу влияла на игру, система должна читать данные из стеша каждый тик и не кэшировать их в своих полях.

---

## Если что-то не работает

| Симптом | Причина |
|---|---|
| Система не вызывается | не добавили её в список в `FeatureRunner` или на сцене нет самого раннера |
| В консоли `must implement plain ISystem` | система наследует `IFixedSystem`/`ILateSystem` — уберите, петлю задаёт список |
| Изменения компонента пропадают | забыли `ref` в `stash.Get` |
| Компонент исчез с сущности | провайдер выключился или удалился вместе с объектом |
| В World Browser у компонента только строка `Script` | не установлен Tri-Inspector или Odin |
| Физика дрожит | система с силами лежит в `UpdateSystems` вместо `FixedUpdateSystems` |

---

## Живой пример

Фича `Gravity` целиком: [компонент](Assets/Scripts/Features/Gravity/Components/Gravity.cs), [система](Assets/Scripts/Features/Gravity/Systems/GravitySystem.cs), [провайдер](Assets/Scripts/Features/Gravity/Providers/GravityProvider.cs), [регистрация](Assets/Scripts/FeatureRunners/FeatureRunner.cs).
