using UnityEngine;
using Utilities;

namespace CoreSystem
{
    public class Movement : MonoBehaviour
    {
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public Rigidbody Body { get; private set; }

        [field: SerializeField] public GameObject CurrentNode { get; private set; }
        [field: SerializeField] public NodeScript NodeScript { get; private set; }
        [field: SerializeField] public GameObject LeftTeleportNode { get; private set; }
        [field: SerializeField] public GameObject RightTeleportNode { get; private set; }

        [field: SerializeField] public bool IsGhost { get; private set; } = false;
        [field: SerializeField] public GhostManager GhostManager { get; private set; }

        [field: SerializeField] public ControlInput LastMove = ControlInput.None;
        [field: SerializeField] public ControlInput CachedMove = ControlInput.None;

        void Awake()
        {
            Body = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            NodeScript = CurrentNode.GetComponent<NodeScript>();
            if (PlayerManager.Instance.isAlive)
            {
                Move();
            }

        }

        public void SetDirection(ControlInput direction)
        {
            if (!direction.Equals(LastMove))
            {
                CachedMove = direction;
            }
        }

        private void Move()
        {
            transform.position = Vector2.MoveTowards(transform.position, CurrentNode.transform.position, Speed * Time.deltaTime);
            if (transform.position == CurrentNode.transform.position)
            {

                if ((NodeScript.isGhostStartingNode && CachedMove == ControlInput.Down)
                    && (!IsGhost || GetComponent<GhostManager>().ghostNodeState != GhostManager.GhostNodeStateEnum.Respawning))
                {
                    if (LastMove == ControlInput.Right)
                    {
                        CurrentNode = NodeScript.NodeRight;
                    }
                    else
                    {
                        CurrentNode = NodeScript.NodeLeft;
                    }
                }

                if (IsGhost)
                {
                    GetComponent<GhostManager>().ReachedNodeCentre(NodeScript);
                }

                if (CachedMove.Equals(ControlInput.Up) && NodeScript.CanMoveUp)
                {
                    LastMove = CachedMove;
                    CurrentNode = NodeScript.NodeUp;
                }
                else if (CachedMove.Equals(ControlInput.Down) && NodeScript.CanMoveDown)
                {
                    LastMove = CachedMove;
                    CurrentNode = NodeScript.NodeDown;
                }
                else if (CachedMove.Equals(ControlInput.Left) && NodeScript.CanMoveLeft)
                {
                    LastMove = CachedMove;
                    CurrentNode = NodeScript.NodeLeft;
                }
                else if (CachedMove.Equals(ControlInput.Right) && NodeScript.CanMoveRight)
                {
                    LastMove = CachedMove;
                    CurrentNode = NodeScript.NodeRight;
                }
                else
                {
                    //keep moving in lasst move direction until CachedMove condition reached
                    MaintainCurrentDirection();
                }

            }

        }
        private void MaintainCurrentDirection()
        {
            if (LastMove.Equals(ControlInput.Up) && NodeScript.CanMoveUp)
            {
                CurrentNode = NodeScript.NodeUp;
            }
            else if (LastMove.Equals(ControlInput.Down) && NodeScript.CanMoveDown)
            {
                CurrentNode = NodeScript.NodeDown;
            }
            else if (LastMove.Equals(ControlInput.Left) && NodeScript.CanMoveLeft)
            {
                CurrentNode = NodeScript.NodeLeft;
            }
            else if (LastMove.Equals(ControlInput.Right) && NodeScript.CanMoveRight)
            {
                CurrentNode = NodeScript.NodeRight;
            }
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.CompareTag("LeftTeleport"))
            {
                transform.position = new Vector3(15f, 2f, 0f);
                CurrentNode = RightTeleportNode;
            }
            else if (collision.gameObject.CompareTag("RightTeleport"))
            {
                transform.position = new Vector3(-14f, 2f, 0f);
                CurrentNode = LeftTeleportNode;
            }
        }
    }
}