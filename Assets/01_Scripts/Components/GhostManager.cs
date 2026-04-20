using System.Collections;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    [RequireComponent(typeof(GhostInputHandler))]
    [RequireComponent(typeof(GhostMovement))]
    [RequireComponent(typeof(GhostAnimator))]
    [RequireComponent(typeof(SkinHandler))]
    public class GhostManager : EntityManager
    {
        public new GhostInputHandler InputHandler { get => (GhostInputHandler)base.InputHandler; protected set => base.InputHandler = value; }

        public GhostState CurrentGhostState;

        [SerializeField] private int _currentWave = 1;
        [SerializeField] private int _collectedPellets = 0;
        [SerializeField] private float _timer = 0f;
        [SerializeField] private bool _isTimerPaused = true;

        void Awake()
        {
            InputHandler = GetComponent<GhostInputHandler>();
            Movement = GetComponent<GhostMovement>();
            Anim = GetComponent<GhostAnimator>();
            Skin = GetComponent<SkinHandler>();
        }

        void Update()
        {
            if (_isTimerPaused) return;

            AlternateGhostModes();
        }

        public void InitialiseGhost(GhostType ghostType, int skinIndex, PlayerManager pacMan)
        {
            _currentWave = 1;
            _collectedPellets = 0;
            _timer = Constants.CHASE_MODE_DURATION;
            //Anim.SetTrigger("Idle");
            Skin.AssignSkin(skinIndex);

            SetGhostType(ghostType);
        }

        private void SetGhostType(GhostType ghostType)
        {
            InputHandler.SetGhostType(ghostType);
            if (ghostType == GhostType.Blinky)
            {
                SetNewGhostState(GhostState.Chasing);
                _isTimerPaused = false;
            }
            else
            {
                SetNewGhostState(GhostState.Standby);
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
                        InputHandler.SetStartNode(StartNode);
                        Movement.SetStartNode(StartNode);
                        break;
                    }
                }
            }
        }

        protected virtual IEnumerator ResetPosition()
        {
            _collectedPellets = 0;
            _timer = Constants.CHASE_MODE_DURATION;
            yield return new WaitForSeconds(2);

            transform.position = StartNode.transform.position;
            //Movement.CurrentNode = StartNode;
        }

        private void AlternateGhostModes()
        {
            if (_currentWave >= 5) return;

            _timer -= Time.deltaTime;
            if (_timer > 0) return;

            switch (CurrentGhostState)
            {
                case GhostState.Chasing:
                    SetNewGhostState(GhostState.Scatter);
                    _timer = GetScatterDuration(_currentWave);
                    break;

                case GhostState.Scatter:
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
            if (CurrentGhostState.Equals(GhostState.Respawning)) return;

            if (other.gameObject.CompareTag("Player"))
            {
                if (other.TryGetComponent(out PlayerManager playerManager))
                {
                    if (!playerManager.IsPowerMode)
                    {
                        playerManager.KillPacMan();
                    }
                    else
                    {
                        _ = GameManager.Instance.AddScore(playerManager.GetKillGhostScore(), false);
                        SetNewGhostState(GhostState.Respawning);
                    }
                }
            }
        }

        private void SetNewGhostState(GhostState newState)
        {
            CurrentGhostState = newState;
            InputHandler.SetNewGhostState(newState);
        }

        public void OnPowerModeEvent(bool enabled)
        {
            if (enabled)
            {
                _isTimerPaused = true;
                if (CurrentGhostState != GhostState.Respawning)
                {
                    SetNewGhostState(GhostState.Frightened);
                }
            }
            else
            {
                _isTimerPaused = false;
                if (CurrentGhostState == GhostState.Frightened)
                {
                    SetNewGhostState(GhostState.Chasing);
                }
            }
        }

        public void OnCollectPelletEvent()
        {
            _collectedPellets++;
            if (_collectedPellets == 10)
            {
                SetNewGhostState(GhostState.Chasing);
                _isTimerPaused = false;
            }
        }
    }
}
