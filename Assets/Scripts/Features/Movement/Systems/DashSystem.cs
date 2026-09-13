using Scellecs.Morpeh;
using UnityEngine;

namespace Features.Dash.Systems
{
    public sealed class DashSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        private Stash<Components.Dashable> _dashStash;
        private Stash<Movement.Components.Movable> _movableStash;
        private Stash<Input.Components.PlayerInput> _inputStash;
        private Stash<Components.DashingTag> _dashingTagStash;

        public void OnAwake()
        {
            _dashStash = World.GetStash<Components.Dashable>();
            _movableStash = World.GetStash<Movement.Components.Movable>();
            _inputStash = World.GetStash<Input.Components.PlayerInput>();
            _dashingTagStash = World.GetStash<Components.DashingTag>();

            _filter = World.Filter
                .With<Components.Dashable>()
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
                ref var dash = ref _dashStash.Get(entity);
                ref var movable = ref _movableStash.Get(entity);
                ref var input = ref _inputStash.Get(entity);

                if (dash.CurrentCooldown > 0)
                    dash.CurrentCooldown -= deltaTime;

                if (_dashingTagStash.Has(entity))
                {
                    dash.DashTimer -= deltaTime;

                    if (dash.DashTimer <= 0)
                    {
                        _dashingTagStash.Remove(entity);

                        Vector3 vel = movable.Rigidbody.linearVelocity;
                        vel.x = 0;
                        vel.z = 0;
                        movable.Rigidbody.linearVelocity = vel;
                    }
                }
                else if (input.DashPressed && dash.CurrentCooldown <= 0)
                {
                    dash.CurrentCooldown = dash.Cooldown;
                    dash.DashTimer = dash.Duration;

                    _dashingTagStash.Add(entity);

                    Vector3 inputDir = new Vector3(input.MoveInput.x, 0f, input.MoveInput.y);

                    if (inputDir == Vector3.zero)
                        inputDir = Vector3.forward;

                    Vector3 globalDir = movable.Rigidbody.transform.TransformDirection(inputDir.normalized);

                    Vector3 vel = movable.Rigidbody.linearVelocity;
                    vel.x = globalDir.x * dash.DashForce;
                    vel.y = 0;
                    vel.z = globalDir.z * dash.DashForce;

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