using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;


namespace CoreSystem
{
    public class Movement : MonoBehaviour
    {

        [SerializeField] private float speed;
        [SerializeField] Rigidbody2D body;
        public GameObject currentNode;
        [SerializeField] private GameObject leftTeleportNode;
        [SerializeField] private GameObject rightTeleportNode;
        [SerializeField] private NodeScript nodeScript;

        private string UP = "up";
        private string DOWN = "down";
        private string LEFT = "left";
        private string RIGHT = "right";

        [SerializeReference] private float xPosition;
        [SerializeReference] private float yPosition;

        [SerializeField] private bool isGhost = false;
        [SerializeField] public string lastMove = "";
        [SerializeField] public string cachedMove = "";

        // Start is called before the first frame update
        void Start()
        {
            body = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            nodeScript = currentNode.GetComponent<NodeScript>();
            if (PlayerManager.Instance.isAlive)
            {
                Move();
            }

        }

        public void SetDirection(string direction)
        {
            if (!direction.Equals(lastMove))
            {
                cachedMove = direction;
            }
        }

        private void Move()
        {
            transform.position = Vector2.MoveTowards(transform.position, currentNode.transform.position, speed * Time.deltaTime);
            if (transform.position == currentNode.transform.position)
            {

                if ((nodeScript.isGhostStartingNode && cachedMove == DOWN)
                    && (!isGhost || GetComponent<GhostManager>().ghostNodeState != GhostManager.GhostNodeStateEnum.Respawning))
                {
                    if (lastMove == RIGHT)
                    {
                        currentNode = nodeScript.nodeRight;
                    }
                    else
                    {
                        currentNode = nodeScript.nodeLeft;
                    }
                }

                if (isGhost)
                {
                    GetComponent<GhostManager>().ReachedNodeCentre(nodeScript);
                }

                if (cachedMove.Equals(UP) && nodeScript.canMoveUp)
                {
                    lastMove = cachedMove;
                    currentNode = nodeScript.nodeUp;
                }
                else if (cachedMove.Equals(DOWN) && nodeScript.canMoveDown)
                {
                    lastMove = cachedMove;
                    currentNode = nodeScript.nodeDown;
                }
                else if (cachedMove.Equals(LEFT) && nodeScript.canMoveLeft)
                {
                    lastMove = cachedMove;
                    currentNode = nodeScript.nodeLeft;
                }
                else if (cachedMove.Equals(RIGHT) && nodeScript.canMoveRight)
                {
                    lastMove = cachedMove;
                    currentNode = nodeScript.nodeRight;
                }
                else
                {
                    //keep moving in lasst move direction until cachedMove condition reached
                    MaintainCurrentDirection();
                }

            }

        }
        private void MaintainCurrentDirection()
        {
            if (lastMove.Equals(UP) && nodeScript.canMoveUp)
            {

                currentNode = nodeScript.nodeUp;
            }
            else if (lastMove.Equals(DOWN) && nodeScript.canMoveDown)
            {

                currentNode = nodeScript.nodeDown;
            }
            else if (lastMove.Equals(LEFT) && nodeScript.canMoveLeft)
            {

                currentNode = nodeScript.nodeLeft;
            }
            else if (lastMove.Equals(RIGHT) && nodeScript.canMoveRight)
            {

                currentNode = nodeScript.nodeRight;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("LeftTeleport"))
            {
                transform.position = new Vector3(15f, 2f, 0f);
                currentNode = rightTeleportNode;
            }
            else if (collision.gameObject.CompareTag("RightTeleport"))
            {
                transform.position = new Vector3(-14f, 2f, 0f);
                currentNode = leftTeleportNode;
            }
        }
    }
}