using Scellecs.Morpeh.Providers;
using UnityEngine;

namespace Features.FirstPersonLook.Providers
{
    [AddComponentMenu("ECS/FirstPersonLook/" + nameof(FirstPersonLookProvider))]
    public sealed class FirstPersonLookProvider : MonoProvider<Components.FirstPersonLook>
    {
        protected override void Initialize()
        {
            ref var look = ref GetData();
            look.BodyTransform = transform;
        }
    }
}