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
                if (gravity.Rigidbody == null || gravity.Rigidbody.isKinematic)
                {
                    continue;
                }

                var direction = gravity.Direction;
                if (direction.sqrMagnitude <= Mathf.Epsilon || Mathf.Approximately(gravity.Force, 0f))
                {
                    continue;
                }
                var velocityDelta = direction.normalized * (gravity.Force * deltaTime);
                gravity.Rigidbody.AddForce(velocityDelta, ForceMode.VelocityChange);
            }
        }

        public void Dispose()
        {
            _filter.Dispose();
        }
        public void OnDisable()
        {
            Dispose();
        }
    }
}
