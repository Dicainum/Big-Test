using System;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.Input.Components
{
    [Serializable]
    public struct PlayerInput : IComponent
    {
        [HideInInspector] public Vector2 MoveInput;
        [HideInInspector] public Vector2 LookInput;
        [HideInInspector] public bool JumpPressed;
        [HideInInspector] public bool DashPressed;
    }
}