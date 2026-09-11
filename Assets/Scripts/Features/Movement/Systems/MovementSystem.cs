using Scellecs.Morpeh;
using UnityEngine;

namespace Features.Movement.Systems
{
    public sealed class MovementSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        private Stash<Components.Movable> _movable;

        public void OnAwake()
        {
            _movable = World.GetStash<Components.Movable>();
            _filter = World.Filter
                .With<Components.Movable>()
                .Build();
        }
        public void OnDisable()
        {
            Dispose();
        }

        public void OnUpdate(float deltaTime)
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveZ = Input.GetAxisRaw("Vertical");

            Vector3 inputDirection = new Vector3(moveX, 0f, moveZ).normalized;

            foreach (var entity in _filter)
            {
                ref var movable = ref _movable.Get(entity);

                if (movable.Rigidbody != null)
                {
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