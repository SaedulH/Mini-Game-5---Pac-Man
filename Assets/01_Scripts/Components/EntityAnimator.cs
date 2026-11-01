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
        [field: SerializeField] public float TurnSpeed { get; protected set; } = 100f;
        private float _moveSpeed = 1f;
        private float _targetSpeed = 1f;
        private float _targetAngle = 1f;

        protected virtual void Awake()
        {
            Anim = GetComponent<Animator>();
            Movement = GetComponent<Movement>();
        }

        protected virtual void Start()
        {
            if (Movement.CachedDirection != ControlInput.None)
            {
                _targetAngle = GetTargetAngle(Movement.CachedDirection);
                Rotator.transform.rotation = Quaternion.Euler(0f, _targetAngle, 0f);
            }
        }

        protected virtual void FixedUpdate()
        {
            if (Movement == null) return;

            SetMoveAnim();
            _targetAngle = GetTargetAngle(Movement.CurrentDirection);
        }

        protected virtual void Update()
        {
            if (Movement == null) return;

            if (!_targetAngle.Equals(Rotator.transform.rotation.eulerAngles.y))
            {
                RotateToTargetAngle();
                return;
            }
        }

        protected virtual void RotateToTargetAngle()
        {
            float newAngle = Mathf.MoveTowardsAngle(
                Rotator.transform.rotation.eulerAngles.y,
                _targetAngle,
                10f * TurnSpeed * Time.deltaTime
            );

            Rotator.transform.rotation = Quaternion.Euler(0f, newAngle, 0f);

            if (Mathf.Abs(Mathf.DeltaAngle(newAngle, _targetAngle)) < 0.1f)
            {
                Rotator.transform.rotation = Quaternion.Euler(0f, _targetAngle, 0f);
            }
        }

        protected virtual float GetTargetAngle(ControlInput targetDirection)
        {
            return targetDirection switch
            {
                ControlInput.Up => 180f,
                ControlInput.Right => 270f,
                ControlInput.Down => 0f,
                ControlInput.Left => 90f,
                _ => throw new ArgumentOutOfRangeException()
            };
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