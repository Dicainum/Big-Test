using Scellecs.Morpeh;
using UnityEngine;

namespace Features.FirstPersonLook.Systems
{
    public sealed class FirstPersonLookSystem : ISystem
    {
        public World World { get; set; }
        private Filter _filter;

        private Stash<Components.FirstPersonLook> _stash;
        private Stash<Input.Components.PlayerInput> _inputStash;

        public void OnAwake()
        {
            _stash = World.GetStash<Components.FirstPersonLook>();
            _inputStash = World.GetStash<Input.Components.PlayerInput>();

            _filter = World.Filter
                .With<Components.FirstPersonLook>()
                .With<Input.Components.PlayerInput>()
                .Build();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void OnDisable()
        {
            Dispose();
        }

        public void OnUpdate(float deltaTime)
        {

            foreach (var entity in _filter)
            {
                ref var look = ref _stash.Get(entity);
                ref var input = ref _inputStash.Get(entity);

                look.BodyTransform.Rotate(Vector3.up * (input.LookInput.x * look.Sensitivity));

                look.Pitch -= input.LookInput.y * look.Sensitivity;
                look.Pitch = Mathf.Clamp(look.Pitch, -90f, 90f);
                look.CameraTransform.localRotation = Quaternion.Euler(look.Pitch, 0f, 0f);
            }
        }

        public void Dispose()
        {
            _filter.Dispose();
        }
    }
}