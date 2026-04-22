using EventSystem;
using System.Threading.Tasks;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    [RequireComponent(typeof(GhostInputHandler))]
    [RequireComponent(typeof(GhostMovement))]
    [RequireComponent(typeof(GhostAnimator))]
    public class GhostManager : EntityManager
    {
        public new GhostInputHandler InputHandler { get => (GhostInputHandler)base.InputHandler; protected set => base.InputHandler = value; }

        [SerializeField] private int _currentWave = 1;
        [SerializeField] private int _collectedPellets = 0;
        [SerializeField] private float _timer = 0f;
        [SerializeField] private bool _isTimerPaused = true;

        void Awake()
        {
            InputHandler = GetComponent<GhostInputHandler>();
            Movement = GetComponent<GhostMovement>();
            Anim = GetComponent<GhostAnimator>();
        }

        void Update()
        {
            if (_isTimerPaused) return;

            AlternateGhostModes();
        }

        public void InitialiseGhost(GhostType ghostType, GhostConfig ghostConfig, int levelNumber)
        {
            _currentWave = 1;
            _collectedPellets = 0;
            _timer = Constants.CHASE_MODE_DURATION;
            //Anim.SetTrigger("Idle");
            Movement.SetSpeed(levelNumber);
            SetGhostType(ghostType, ghostConfig);
        }

        public override void OnGameStateUpdated(GameState gameState)
        {
            _isTimerPaused = !gameState.Equals(GameState.Playing);
            base.OnGameStateUpdated(gameState);
        }

        protected override async Task ResetPosition()
        {
            Debug.Log($"Resetting {gameObject.name}");
            Anim.Disappear();
            InputHandler.SetEndExitVariables();
            await Task.Delay(1000);
            Movement.SetStartNode(StartNode);
            Anim.Reappear();
        }

        private void SetGhostType(GhostType ghostType, GhostConfig ghostConfig)
        {
            InputHandler.SetGhostType(ghostType, ghostConfig);
            if (ghostType == GhostType.Blinky)
            {
                SetNewGhostState(GhostState.Chasing);
                _isTimerPaused = false;
            }
            else
            {
                SetNewGhostState(GhostState.Waiting);
                _isTimerPaused = true;
            }
        }

        public void SetTargets(Transform target, PlayerManager pacMan)
        {
            InputHandler.SetTargets(target, pacMan);
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
                    if (node.NodeType == NodeType.GhostStart)
                    {
                        StartNode = node;
                        InputHandler.RespawnNode = StartNode;
                        Movement.SetStartNode(StartNode);
                        break;
                    }
                }
            }
        }

        private void AlternateGhostModes()
        {
            if (_currentWave >= 5) return;

            _timer -= Time.deltaTime;
            if (_timer > 0) return;

            switch (InputHandler.CurrentState)
            {
                case GhostState.Chasing:
                    SetNewGhostState(GhostState.Scattering);
                    _timer = GetScatterDuration(_currentWave);
                    break;

                case GhostState.Scattering:
                    SetNewGhostState(GhostState.Chasing);
                    _timer = Constants.CHASE_MODE_DURATION;
                    _currentWave++;
                    break;

                default:
                    break;
            }
        }

        private float GetScatterDuration(int wave)
        {
            return wave switch
            {
                1 or 2 => Constants.EARLY_SCATTER_MODE_DURATION,
                _ => Constants.LATE_SCATTER_MODE_DURATION,
            };
        }

        private void OnTriggerEnter(Collider other)
        {
            if (InputHandler.CurrentState.Equals(GhostState.Returning)) return;

            if (other.gameObject.CompareTag("Player"))
            {
                if (other.TryGetComponent(out PlayerManager playerManager))
                {
                    if (!playerManager.IsPowerMode)
                    {
                        Debug.Log("PacMan Hit");
                        playerManager.OnHitEvent();
                    }
                    else
                    {
                        _ = GameManager.Instance.AddScore(playerManager.GetHitGhostScore(), false);
                        OnHitEvent();
                    }
                }
            }
        }

        private void SetNewGhostState(GhostState newState)
        {
            InputHandler.SetNewGhostState(newState);
        }

        public void OnPowerModeEvent(bool enabled)
        {
            Debug.Log($"Power Mode: {enabled}");
            _isTimerPaused = enabled;
            if (enabled)
            {
                if (CanEnterFrightenedState(InputHandler.CurrentState))
                {
                    SetNewGhostState(GhostState.Frightened);
                }
            }
            else
            {
                if (InputHandler.CurrentState.Equals(GhostState.Frightened))
                {
                    SetNewGhostState(GhostState.Chasing);
                }
            }
        }

        private bool CanEnterFrightenedState(GhostState ghostState)
        {
            return InputHandler.CurrentState.Equals(GhostState.Chasing) ||
                InputHandler.CurrentState.Equals(GhostState.Scattering);
        }

        public void OnCollectPelletEvent()
        {
            _collectedPellets++;
            if (InputHandler.IsEnoughPelletsToExit(_collectedPellets))
            {
                _isTimerPaused = false;
                _collectedPellets = 0;
            }
        }

        public override void OnHitEvent()
        {
            Anim.SetDeath(true);
            OnHit.Invoke(new Empty());
        }

        protected override void OnDeathEvent()
        {
            SetNewGhostState(GhostState.Returning);
        }
    }
}
