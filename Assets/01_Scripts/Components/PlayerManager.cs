using EventSystem;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Utilities;

namespace CoreSystem
{

    [RequireComponent(typeof(PlayerInputHandler))]
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerAnimator))]
    public class PlayerManager : EntityManager
    {
        public new PlayerInputHandler InputHandler { get => (PlayerInputHandler)base.InputHandler; protected set => base.InputHandler = value; }

        [field: SerializeField] public bool IsPowerMode { get; private set; } = false;
        [field: SerializeField] public int GhostHitCount { get; private set; } = 0;

        private bool _isActive = true;
        private Coroutine _powerModeCoroutine;

        [field: SerializeField] public BoolEventChannel OnPowerMode { get; private set; }
        [field: SerializeField] public EventChannel OnDeath { get; private set; }

        private void Awake()
        {
            InputHandler = GetComponent<PlayerInputHandler>();
            Movement = GetComponent<PlayerMovement>();
            Anim = GetComponent<PlayerAnimator>();
        }

        public void InitialisePlayer(PlayerInputActions inputActions, int levelNumber)
        {
            _isActive = true;
            IsPowerMode = false;

            InputHandler.SetInputActions(inputActions);
            Movement.CurrentDirection = ControlInput.Right;
            //Anim.SetTrigger("Idle");
            Movement.SetSpeed(levelNumber);
        }

        public override void OnGameStateUpdated(GameState gameState)
        {
            _isActive = gameState.Equals(GameState.Playing);
            base.OnGameStateUpdated(gameState);
        }

        protected override async Task ResetPosition()
        {
            Anim.Disappear();
            await Task.Delay(1000);
            Movement.SetStartNode(StartNode);
            Movement.CurrentDirection = ControlInput.Right;
            Anim.Reappear();
        }

        public void SetSpawnpoint(Vector3 position, Quaternion rotation)
        {
            transform.SetPositionAndRotation(position, rotation);
            Collider[] colliders = Physics.OverlapSphere(position, 0.25f, LayerMask.GetMask("Nodes"));
            if (colliders.Length == 0) return;

            foreach (Collider collider in colliders)
            {
                if (collider.TryGetComponent(out NodeScript node))
                {
                    if (node.NodeType == NodeType.PacManStart)
                    {
                        StartNode = node;
                        Movement.SetStartNode(StartNode);
                        break;
                    }
                }
            }
        }

        public void ActivatePowerMode()
        {
            Debug.Log("Start Power Mode");
            if (_powerModeCoroutine != null)
            {
                StopCoroutine(_powerModeCoroutine);
            }

            _powerModeCoroutine = StartCoroutine(PowerMode());
        }

        public IEnumerator PowerMode()
        {
            GhostHitCount = 0;
            IsPowerMode = true;
            Anim.SetPowerMode(true);
            OnPowerMode.Invoke(true);
            yield return new WaitForSeconds(Constants.FRIGHTENED_MODE_DURATION);

            IsPowerMode = false;
            Anim.SetPowerMode(false);
            OnPowerMode.Invoke(false);
            Debug.Log("End Power Mode");
        }
        public int GetHitGhostScore()
        {
            GhostHitCount++;
            return GhostHitCount * Constants.GHOST_SCORE;
        }

        public override void OnHitEvent()
        {
            if (!_isActive) return;

            _isActive = false;
            Anim.SetDeath(true);
            OnHit.Invoke(new Empty());
        }

        protected override void OnDeathEvent()
        {
            OnDeath.Invoke(new Empty());
        }
    }
}