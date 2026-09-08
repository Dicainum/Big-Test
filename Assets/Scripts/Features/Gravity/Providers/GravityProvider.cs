using Scellecs.Morpeh.Providers;
using UnityEngine;

namespace Features.Gravity.Providers
{
    [RequireComponent(typeof(Rigidbody))]
    [AddComponentMenu("ECS/Gravity/" + nameof(GravityProvider))]
    public sealed class GravityProvider : MonoProvider<Components.Gravity>
    {
        protected override void Initialize()
        {
            ref var gravity = ref GetData();
            gravity.Rigidbody = GetComponent<Rigidbody>();
            gravity.Rigidbody.useGravity = false;
        }

        private void Reset()
        {
            ref var gravity = ref GetSerializedData();
            gravity.Direction = Vector3.down;
            gravity.Force = Mathf.Abs(Physics.gravity.y);
        }
    }
}
