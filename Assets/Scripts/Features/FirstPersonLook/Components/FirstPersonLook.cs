using System;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.FirstPersonLook.Components
{
    [Serializable]
    public struct FirstPersonLook : IComponent
    {
        public Transform CameraTransform;
        public Transform BodyTransform;

        public float Sensitivity;

        [HideInInspector] public float Pitch;
    }
}