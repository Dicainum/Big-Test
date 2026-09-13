using System;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.Dash.Components
{
    [Serializable]
    public struct Dashable : IComponent
    {
        public float DashForce;
        public float Duration;
        public float Cooldown;

        [HideInInspector] public float CurrentCooldown;
        [HideInInspector] public float DashTimer;
    }
}