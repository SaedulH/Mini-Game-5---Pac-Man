using System;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    public class EntityAnimator : MonoBehaviour
    {
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int DeathHash = Animator.StringToHash("Death");
        private static readonly int ReappearHash = Animator.StringToHash("Reappear");
        private static readonly int DisappearHash = Animator.StringToHash("Disappear");

        [field: SerializeField] public Animator Anim { get; protected set; }
        [field: SerializeField] public Movement Movement { get; protected set; }
        [field: SerializeField] public GameObject Rotator { get; protected set; }
        [field: SerializeField] public ControlInput CurrentDirection { get; protected set; }

        private float _moveSpeed = 1f;
        private float _currentSpeed = 1f;
        private float _targetSpeed = 1f;

        protected virtual void Awake()
        {
            Anim = GetComponent<Animator>();
            Movement = GetComponent<Movement>();
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
            if (Movement == null) return;

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

        protected virtual void RotateToDirection()
        {
            float yRotation = CurrentDirection switch
            {
                ControlInput.Up => 180f,
                ControlInput.Right => 270f,
                ControlInput.Down => 0f,
                ControlInput.Left => 90f,
                _ => throw new ArgumentOutOfRangeException()
            };
            Rotator.transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }

        protected virtual void SetMoveAnim()
        {
            _targetSpeed = Movement.IsMoving ? _moveSpeed : 0f;

            _moveSpeed = Mathf.MoveTowards(_moveSpeed, _targetSpeed, 10f * Time.deltaTime);
            Anim.SetFloat(SpeedHash, _moveSpeed);
        }

        public virtual void SetPowerMode(bool enabled)
        {
            _moveSpeed = enabled ? 2f : 1f;
        }

        public virtual void SetDeath(bool value)
        {
            Anim.SetBool(DeathHash, value);
        }

        public virtual void Disappear()
        {
            Anim.SetTrigger(DisappearHash);
        }

        public virtual void Reappear()
        {
            Anim.SetTrigger(ReappearHash);
            SetDeath(false);
        }
    }
}