using System;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.Jump.Components
{
    [Serializable]
    public struct Jumpable : IComponent
    {
        public float JumpForce;
        public int MaxJumps;
        public float GroundCheckDistance;
        public LayerMask GroundLayer;

        [HideInInspector] public int JumpsLeft;
        [HideInInspector] public bool IsGrounded;
    }
}