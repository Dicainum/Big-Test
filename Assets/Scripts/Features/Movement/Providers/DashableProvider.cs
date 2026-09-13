using Scellecs.Morpeh.Providers;
using UnityEngine;

namespace Features.Dash.Providers
{
    [AddComponentMenu("ECS/Dash/" + nameof(DashableProvider))]
    public sealed class DashableProvider : MonoProvider<Components.Dashable>
    {
    }
}