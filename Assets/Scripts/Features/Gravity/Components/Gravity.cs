using System;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.Gravity.Components
{
    [Serializable]
    public struct Gravity : IComponent
    {
        [Tooltip("Direction the gravity pulls to. Normalized by the system.")]
        public Vector3 Direction;

        [Tooltip("Acceleration in m/s^2 applied along Direction.")]
        public float Force;

        [HideInInspector]
        public Rigidbody Rigidbody;
    }
}
