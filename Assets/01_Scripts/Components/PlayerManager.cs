using System.Collections;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    [RequireComponent(typeof(PlayerInputHandler))]
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerAnimator))]
    [RequireComponent(typeof(SkinHandler))]
    public class PlayerManager : NonPersistentSingleton<PlayerManager>
    {
        public int remainingLives = 4;
        public bool isAlive = true;
        public bool isPowerMode = false;
        [field: SerializeField] public PlayerInputHandler InputHandler { get; private set; }
        [field: SerializeField] public PlayerMovement Movement { get; private set; }
        [field: SerializeField] public PlayerAnimator Anim { get; private set; }
        [field: SerializeField] public SkinHandler Skin { get; private set; }

        [field: SerializeField] public NodeScript StartNode { get; private set; }

        protected override void Awake()
        {
            base.Awake();
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
            Collider[] results = new Collider[10];
            int nodeCount = Physics.OverlapSphereNonAlloc(position, 0.5f, results, LayerMask.GetMask("Nodes"));
            if (nodeCount > 0)
            {
                StartNode = results[0].GetComponent<NodeScript>();
                Movement.SetStartNode(StartNode);
            }
        }

        public void ActivatePowerMode()
        {

        }

        public void DeactivatePowerMode()
        {

        }

        //public IEnumerator PowerMode()
        //{
        //    //Debug.Log("Start powermode");
        //    //isPowerMode = true;
        //    //body.color = new Color32(255, 206, 206, 255);
        //    //yield return new WaitForSeconds(10);

        //    //body.color = new Color32(255, 255, 255, 255);
        //    //isPowerMode = false;
        //    //Debug.Log("End powermode");
        //}

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