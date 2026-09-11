using Scellecs.Morpeh;
using UnityEngine;

namespace Features.FirstPersonLook.Systems
{
    public sealed class FirstPersonLookSystem : ISystem
    {
        public World World { get; set; }
        private Filter _filter;
        private Stash<Components.FirstPersonLook> _stash;

        public void OnAwake()
        {
            _stash = World.GetStash<Components.FirstPersonLook>();
            _filter = World.Filter.With<Components.FirstPersonLook>().Build();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void OnUpdate(float deltaTime)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            foreach (var entity in _filter)
            {
                ref var look = ref _stash.Get(entity);

                look.BodyTransform.Rotate(Vector3.up * (mouseX * look.Sensitivity));

                look.Pitch -= mouseY * look.Sensitivity;
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