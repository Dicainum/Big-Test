using Scellecs.Morpeh;
using UnityEngine;

namespace Features.Movement.Systems
{
    public sealed class MovementSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        private Stash<Components.Movable> _movable;
        private Stash<Input.Components.PlayerInput> _input;

        public void OnAwake()
        {
            _movable = World.GetStash<Components.Movable>();
            _input = World.GetStash<Input.Components.PlayerInput>();

            _filter = World.Filter
                .With<Components.Movable>()
                .With<Input.Components.PlayerInput>()
                .Without<Dash.Components.DashingTag>()
                .Build();
        }

        public void OnDisable()
        {
            Dispose();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref var movable = ref _movable.Get(entity);
                ref var input = ref _input.Get(entity);

                if (movable.Rigidbody != null)
                {
                    Vector3 inputDirection = new Vector3(input.MoveInput.x, 0f, input.MoveInput.y).normalized;
                    Vector3 globalDirection = movable.Rigidbody.transform.TransformDirection(inputDirection);

                    Vector3 newVelocity = globalDirection * movable.Speed;
                    newVelocity.y = movable.Rigidbody.linearVelocity.y;
                    movable.Rigidbody.linearVelocity = newVelocity;
                }
            }
        }

        public void Dispose()
        {
            _filter.Dispose();
        }
    }
}