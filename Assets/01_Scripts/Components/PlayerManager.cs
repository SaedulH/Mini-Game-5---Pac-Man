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
        public int remainingLives = 4;
        public bool isAlive = true;
        public bool isPowerMode = false;

        private Coroutine _powerModeCoroutine;

        private void Awake()
        {
            InputHandler = GetComponent<PlayerInputHandler>();
            Movement = GetComponent<PlayerMovement>();
            Anim = GetComponent<PlayerAnimator>();
            Skin = GetComponent<SkinHandler>();
        }

        public void InitialisePlayer(PlayerInputActions inputActions, int remainingLives, int skinIndex)
        {
            this.remainingLives = remainingLives;
            this.isAlive = true;
            this.isPowerMode = false;

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
                if(collider.TryGetComponent(out NodeScript node))
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
            if(_powerModeCoroutine != null)
            {
                StopCoroutine(_powerModeCoroutine);
            }

            _powerModeCoroutine = StartCoroutine(PowerMode());
        }

        public IEnumerator PowerMode()
        {
            isPowerMode = true;
            Anim.SetPowerMode(true);
            yield return new WaitForSeconds(10);

            isPowerMode = false;
            Anim.SetPowerMode(false);
            Debug.Log("End Power Mode");
        }

        private void OnCollisionEnter(Collision collision)
        {
            //if (collision.gameObject.CompareTag("Ghost"))
            //{
            //    if (!isPowerMode)
            //    {
            //        isAlive = false;
            //        StartCoroutine(RespawnPacman());
            //    }
            //    else
            //    {
            //        GhostManager ghostManager = collision.gameObject.GetComponent<GhostManager>();
            //        if (ghostManager.ghostNodeState != GhostManager.GhostNodeStateEnum.Respawning)
            //        {
            //            GameManager.Instance.AddScore(800, false);
            //            ghostManager.ghostNodeState = GhostManager.GhostNodeStateEnum.Respawning;
            //        }
            //    }
            //}
        }

        private IEnumerator RespawnPacman()
        {
            if (remainingLives > 1)
            {
                remainingLives--;
                yield return new WaitForSeconds(2);
                //transform.position = startPosition;
                //Movement.CurrentNode = startNode;
                Movement.CurrentDirection = ControlInput.Right;
                yield return new WaitForSeconds(1);
                isAlive = true;
            }
            else
            {
                //gameOver screen
                yield return new WaitForSeconds(1);
            }
        }
    }
}