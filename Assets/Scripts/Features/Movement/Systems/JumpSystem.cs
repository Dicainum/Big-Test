using Scellecs.Morpeh;
using UnityEngine;

namespace Features.Jump.Systems
{
    public sealed class JumpSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        private Stash<Components.Jumpable> _jumpable;
        private Stash<Movement.Components.Movable> _movable;
        private Stash<Input.Components.PlayerInput> _input;

        public void OnAwake()
        {
            _jumpable = World.GetStash<Components.Jumpable>();
            _movable = World.GetStash<Movement.Components.Movable>();
            _input = World.GetStash<Input.Components.PlayerInput>();

            _filter = World.Filter
                .With<Components.Jumpable>()
                .With<Movement.Components.Movable>()
                .With<Input.Components.PlayerInput>()
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
                ref var jumpable = ref _jumpable.Get(entity);
                ref var movable = ref _movable.Get(entity);
                ref var input = ref _input.Get(entity);

                if (movable.Rigidbody == null) continue;

                Vector3 origin = movable.Rigidbody.position + Vector3.up * 0.1f;
                jumpable.IsGrounded = Physics.Raycast(origin, Vector3.down, jumpable.GroundCheckDistance, jumpable.GroundLayer);

                if (jumpable.IsGrounded && movable.Rigidbody.linearVelocity.y <= 0.1f)
                {
                    jumpable.JumpsLeft = jumpable.MaxJumps;
                }

                if (input.JumpPressed && jumpable.JumpsLeft > 0)
                {
                    jumpable.JumpsLeft--;

                    Vector3 vel = movable.Rigidbody.linearVelocity;
                    vel.y = jumpable.JumpForce;
                    movable.Rigidbody.linearVelocity = vel;
                }
            }
        }
        public void Dispose()
        {
            _filter.Dispose();
        }
    }
}