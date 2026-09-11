using Scellecs.Morpeh.Providers;
using UnityEngine;

namespace Features.Jump.Providers
{
    [AddComponentMenu("ECS/Jump/" + nameof(JumpableProvider))]
    public sealed class JumpableProvider : MonoProvider<Components.Jumpable>
    {
    }
}