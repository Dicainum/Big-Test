using Scellecs.Morpeh;
using UnityEngine;

namespace Features.Input.Systems
{
    public sealed class PlayerInputSystem : ISystem
    {
        public World World { get; set; }
        private Filter _filter;
        private Stash<Components.PlayerInput> _inputStash;

        public void OnAwake()
        {
            _inputStash = World.GetStash<Components.PlayerInput>();
            _filter = World.Filter.With<Components.PlayerInput>().Build();
        }
        public void OnDisable()
        {
            Dispose();
        }

        public void OnUpdate(float deltaTime)
        {
            Vector2 move = new Vector2(UnityEngine.Input.GetAxisRaw("Horizontal"), UnityEngine.Input.GetAxisRaw("Vertical"));
            Vector2 look = new Vector2(UnityEngine.Input.GetAxis("Mouse X"), UnityEngine.Input.GetAxis("Mouse Y"));
            bool jump = UnityEngine.Input.GetKeyDown(KeyCode.Space);
            bool dash = UnityEngine.Input.GetKeyDown(KeyCode.LeftShift);

            foreach (var entity in _filter)
            {
                ref var input = ref _inputStash.Get(entity);
                input.MoveInput = move;
                input.LookInput = look;
                input.JumpPressed = jump;
                input.DashPressed = dash;
            }
        }

        public void Dispose()
        {
            _filter.Dispose();
        }
    }
}