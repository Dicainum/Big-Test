using System;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.Movement.Components
{
    [Serializable]
    public struct Movable : IComponent
    {
        public float Speed;

        public Rigidbody Rigidbody;
    }
}