using System.Collections.Generic;
//using Features.Gravity.Systems;
//using Features.Movement.Systems;
using Scellecs.Morpeh;
using UnityEngine;

namespace FeatureRunners
{
    [DefaultExecutionOrder(-10000)]
    [AddComponentMenu("ECS/" + nameof(FeatureRunner))]
    public sealed class FeatureRunner : MonoBehaviour
    {
        private World _world;

        private SystemsGroup _updateGroup;
        private SystemsGroup _fixedUpdateGroup;
        private SystemsGroup _lateUpdateGroup;
        
        private static IEnumerable<ISystem> UpdateSystems()
        {
            yield return new Features.Input.Systems.PlayerInputSystem();
            yield return new Features.Jump.Systems.JumpSystem();
        }

        private static IEnumerable<ISystem> FixedUpdateSystems()
        {
            yield return new Features.Gravity.Systems.GravitySystem();
            yield return new Features.Movement.Systems.MovementSystem();
        }

        private static IEnumerable<ISystem> LateUpdateSystems()
        {
            yield return new Features.FirstPersonLook.Systems.FirstPersonLookSystem();
        }
        
        private void Awake()
        {
            _world = World.Default;

            _updateGroup = CreateGroup(UpdateSystems());
            _fixedUpdateGroup = CreateGroup(FixedUpdateSystems());
            _lateUpdateGroup = CreateGroup(LateUpdateSystems());

            _updateGroup.Initialize();
            _fixedUpdateGroup.Initialize();
            _lateUpdateGroup.Initialize();
        }

        private void FixedUpdate()
        {
            _fixedUpdateGroup.Update(Time.fixedDeltaTime);
        }

        private void Update()
        {
            _updateGroup.Update(Time.deltaTime);
        }

        private void LateUpdate()
        {
            _lateUpdateGroup.Update(Time.deltaTime);
        }

        private void OnDestroy()
        {
            DisposeGroup(ref _updateGroup);
            DisposeGroup(ref _fixedUpdateGroup);
            DisposeGroup(ref _lateUpdateGroup);
            _world = null;
        }

        private SystemsGroup CreateGroup(IEnumerable<ISystem> systems)
        {
            var group = _world.CreateSystemsGroup();
            foreach (var system in systems)
            {
                if (system is IFixedSystem || system is ILateSystem || system is ICleanupSystem)
                {
                    Debug.LogError($"[{nameof(FeatureRunner)}] {system.GetType().Name} must implement plain ISystem: the list it is declared in already defines its loop.", this);
                    continue;
                }

                group.AddSystem(system);
            }

            return group;
        }

        private void DisposeGroup(ref SystemsGroup group)
        {
            if (group == null)
            {
                return;
            }

            if (!_world.IsNullOrDisposed())
            {
                group.Dispose();
            }

            group = null;
        }
    }
}
