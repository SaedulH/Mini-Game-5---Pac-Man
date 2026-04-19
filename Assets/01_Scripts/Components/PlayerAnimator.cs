using System;
using UnityEngine;
using Utilities;

namespace CoreSystem
{

    public class PlayerAnimator : EntityAnimator
    {
        private void Awake()
        {
            Anim = GetComponent<Animator>();
            Movement = GetComponent<PlayerMovement>();
            Body = GetComponentInChildren<MeshRenderer>().gameObject;
        }

        private void Start()
        {
            CurrentDirection = Movement.CachedDirection;
            if (CurrentDirection != ControlInput.None)
            {
                RotateToDirection();
            }
        }

        private void Update()
        {
            GetDirection();
        }

        private void GetDirection()
        {
            if (!Movement.CurrentDirection.Equals(CurrentDirection))
            {
                CurrentDirection = Movement.CurrentDirection;
                RotateToDirection();
            }
        }

        private void RotateToDirection()
        {
            float yRotation = CurrentDirection switch
            {
                ControlInput.Right => 0f,
                ControlInput.Down => 90f,
                ControlInput.Left => 180f,
                ControlInput.Up => 270f,
                _ => throw new ArgumentOutOfRangeException()
            };
            Body.transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }

        public void SetPowerMode(bool enabled)
        {
            if (enabled)
            {
                Anim.SetTrigger("PowerMove");
            }
            else 
            { 
                Anim.SetTrigger("Move");
            }
        }

        public void SetDeath()
        {
            Anim.SetTrigger("Death");
        }
    }
}