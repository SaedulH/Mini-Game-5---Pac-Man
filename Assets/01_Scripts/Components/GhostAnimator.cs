using System;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    public class GhostAnimator : EntityAnimator
    {
        public new GhostMovement Movement { get => (GhostMovement)base.Movement; protected set => base.Movement = value; }

        [field: SerializeField] public SkinnedMeshRenderer MeshRenderer { get; protected set; }
        [field: SerializeField] public Material OuterMaterial { get; protected set; }
        [field: SerializeField] public Material InnerMaterial { get; protected set; }
        [field: SerializeField] public Material ScatterMaterial { get; protected set; }
        [field: SerializeField] public Material ReturnMaterial { get; protected set; }

        protected override void Awake()
        {
            base.Awake();
            MeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }

        private void OnEnable()
        {
            Movement.InputHandler.SetReturningState += ToggleSkin;
        }

        private void OnDisable()
        {
            Movement.InputHandler.SetReturningState -= ToggleSkin;
        }

        private void ToggleSkin(bool isReturning)
        {
            if (isReturning)
            {
                SetMaterials(ReturnMaterial, ReturnMaterial);
            }
            else
            {
                SetMaterials(OuterMaterial, InnerMaterial);
            }
        }

        public override void SetPowerMode(bool enabled)
        {
            base.SetPowerMode(enabled);
            if (enabled)
            {
                SetMaterials(ScatterMaterial, InnerMaterial);
            }
            else
            {
                SetMaterials(OuterMaterial, InnerMaterial);
            }
        }

        private void SetMaterials(Material outer, Material inner)
        {
            if (MeshRenderer == null) return;

            Material[] currentMaterials = MeshRenderer.materials;
            currentMaterials[0] = outer;
            currentMaterials[1] = inner;
            MeshRenderer.materials = currentMaterials;
        }

        public void SetBaseMaterial(Material material)
        {
            if (MeshRenderer == null) return;
            OuterMaterial = material;
            MeshRenderer.material = material;
        }

        protected override void RotateToDirection()
        {

            float yRotation = CurrentDirection switch
            {
                ControlInput.Up => 90f,
                ControlInput.Right => 180f,
                ControlInput.Down => 270f,
                ControlInput.Left => 0f,
                _ => throw new ArgumentOutOfRangeException()
            };
            Rotator.transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
        } 
    }
}