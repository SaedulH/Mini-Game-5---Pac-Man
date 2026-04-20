using EventSystem;
using System.Collections;
using UnityEngine;
using Utilities;

namespace CoreSystem
{

    [RequireComponent(typeof(PlayerInputHandler))]
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerAnimator))]
    [RequireComponent(typeof(SkinHandler))]
    public class PlayerManager : EntityManager
    {
        public new PlayerInputHandler InputHandler { get => (PlayerInputHandler)base.InputHandler; protected set => base.InputHandler = value; }

        [field: SerializeField] public int RemainingLives { get; private set; } = 4;
        [field: SerializeField] public bool IsAlive { get; private set; } = true;
        [field: SerializeField] public bool IsPowerMode { get; private set; } = false;
        [field: SerializeField] public int GhostKillCount { get; private set; } = 0;

        private Coroutine _powerModeCoroutine;
        [field: SerializeField] public BoolEventChannel OnPowerMode { get; private set; }

        private void Awake()
        {
            InputHandler = GetComponent<PlayerInputHandler>();
            Movement = GetComponent<PlayerMovement>();
            Anim = GetComponent<PlayerAnimator>();
            Skin = GetComponent<SkinHandler>();
        }

        public void InitialisePlayer(PlayerInputActions inputActions, int remainingLives, int skinIndex)
        {
            this.RemainingLives = remainingLives;
            this.IsAlive = true;
            this.IsPowerMode = false;

            InputHandler.SetInputActions(inputActions);
            Movement.CurrentDirection = ControlInput.Right;
            //Anim.SetTrigger("Idle");
            Skin.AssignSkin(skinIndex);
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
            GhostKillCount = 0;
            IsPowerMode = true;
            Anim.SetPowerMode(true);
            OnPowerMode.Invoke(true);
            yield return new WaitForSeconds(Constants.FRIGHTENED_MODE_DURATION);

            IsPowerMode = false;
            Anim.SetPowerMode(false);
            OnPowerMode.Invoke(false);
            Debug.Log("End Power Mode");
        }

        public void KillPacMan()
        {
            if (!IsAlive) return;
            IsAlive = false;
            Anim.SetDeath(true);
            StartCoroutine(RespawnPacman());
        }

        private IEnumerator RespawnPacman()
        {
            if (RemainingLives > 1)
            {
                RemainingLives--;
                yield return new WaitForSeconds(2);
                //transform.position = startPosition;
                //Movement.CurrentNode = startNode;
                Movement.CurrentDirection = ControlInput.Right;
                yield return new WaitForSeconds(1);
                IsAlive = true;
            }
            else
            {
                //gameOver screen
                yield return new WaitForSeconds(1);
            }
        }

        public int GetKillGhostScore()
        {
            GhostKillCount++;
            return GhostKillCount * Constants.GHOST_SCORE;
        }
    }
}