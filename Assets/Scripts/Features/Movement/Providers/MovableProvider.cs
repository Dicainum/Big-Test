using Scellecs.Morpeh.Providers;
using UnityEngine;

namespace Features.Movement.Providers
{
    [AddComponentMenu("ECS/Movement/" + nameof(MovableProvider))]
    public sealed class MovableProvider : MonoProvider<Components.Movable>
    {
        protected override void Initialize()
        {
            ref var movable = ref GetData();
            movable.Rigidbody = GetComponent<Rigidbody>();
        }
    }
}