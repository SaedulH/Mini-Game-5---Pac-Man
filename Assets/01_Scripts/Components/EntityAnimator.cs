using System;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    public class EntityAnimator : MonoBehaviour
    {
        [field: SerializeField] public Animator Anim { get; protected set; }
        [field: SerializeField] public Movement Movement { get; protected set; }
        [field: SerializeField] public GameObject Body { get; protected set; }
        [field: SerializeField] public ControlInput CurrentDirection { get; protected set; }

        private float _moveSpeed = 1f;
        private float _topSpeed = 1f;

        protected virtual void Awake()
        {
            Anim = GetComponent<Animator>();
            Movement = GetComponent<Movement>();
            Body = GetComponentInChildren<MeshRenderer>().gameObject;
        }
        protected virtual void Start()
        {
            CurrentDirection = Movement.CachedDirection;
            if (CurrentDirection != ControlInput.None)
            {
                RotateToDirection();
            }
        }

        protected virtual void Update()
        {
            if (Movement == null || Movement.RB == null) return;

            SetMoveAnim();
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

        private void SetMoveAnim()
        {
            float currentVelocity = Movement.RB.linearVelocity.magnitude;

            float targetSpeed = currentVelocity > 0.1f ? _topSpeed : 0f;

            _moveSpeed = Mathf.MoveTowards(_moveSpeed, targetSpeed, 10f * Time.deltaTime);
            Anim.SetFloat("Speed", _moveSpeed);
        }

        public virtual void SetPowerMode(bool enabled)
        {
            _topSpeed = enabled ? 2f : 1f;
        }

        public virtual void SetDeath(bool value)
        {
            Anim.SetBool("Death", value);
        }

        public virtual void Disappear()
        {
            Anim.SetTrigger("Disappear");
        }

        public virtual void Reappear()
        {
            Anim.SetTrigger("Reappear");
            SetDeath(false);
        }
    }
}