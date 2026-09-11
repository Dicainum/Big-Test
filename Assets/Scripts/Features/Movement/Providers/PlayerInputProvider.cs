using Scellecs.Morpeh.Providers;
using UnityEngine;

namespace Features.Input.Providers
{
    [AddComponentMenu("ECS/Input/" + nameof(PlayerInputProvider))]
    public sealed class PlayerInputProvider : MonoProvider<Components.PlayerInput>
    {
    }
}