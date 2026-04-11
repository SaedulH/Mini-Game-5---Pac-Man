using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    [RequireComponent(typeof(Movement))]

    public class GhostManager : MonoBehaviour
    {
        public enum GhostNodeStateEnum
        {
            Respawning,
            LeftNode,
            RightNode,
            CentreNode,
            StartNode,
            MovingInNodes,
            Scatter,
            Frightened
        }

        public enum GhostName
        {
            Inky,
            Blinky,
            Pinky,
            Clive
        }

        public GhostName ghostName;
        public GhostNodeStateEnum respawnState;
        public GhostNodeStateEnum ghostNodeState;

        [SerializeField] private Animator ghostAnim;
        [SerializeField] private SpriteRenderer body;
        [SerializeField] private GameObject ghostNodeLeft;
        [SerializeField] private GameObject ghostNodeRight;
        [SerializeField] private GameObject ghostNodeCentre;
        [SerializeField] private GameObject ghostNodeStart;

        public GameObject startingNode;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private GhostAnimator animator;

        public Movement movement;
        private Vector3 targetPosition;
        private float speed = 1;
        private Movement playerMovement;
        private Color32 bodyColor;
        private Color32 scaredColor = new Color32(0, 34, 255, 255);

        private Vector3 blinkyCorner = new Vector3(12.5f, 14.5f, 0);
        private Vector3 inkyCorner = new Vector3(12.5f, -13.5f, 0);
        private Vector3 pinkyCorner = new Vector3(-12.5f, 14.5f, 0);
        private Vector3 cliveCorner = new Vector3(-12.5f, -13.5f, 0);

        [SerializeField] private GameObject[] allGhosts;
        [SerializeField] private GameObject trackBlinky;
        [SerializeField] bool isReadyToLeaveHome = false;
        [SerializeField] private bool isTimerPaused = true;
        [SerializeField] private bool commenceRound = true;
        [SerializeField] private bool powerModeActivated = false;
        [SerializeField] private int startingPelletCount;
        [SerializeField] private bool directionChanged = false;

        private float timer = 20f;

        // Start is called before the first frame update
        void Awake()
        {
            ghostAnim = GetComponent<Animator>();
            movement = GetComponent<Movement>();
            gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
            animator = GetComponent<GhostAnimator>();
            playerMovement = gameManager.PacMan.GetComponent<Movement>();
            bodyColor = body.color;

            GetStartPosition();
            transform.position = startingNode.transform.position;
            //movement.CurrentNode = startingNode;
        }


        private void Start()
        {
            startingPelletCount = gameManager.PelletCount;
            if (ghostName.Equals(GhostName.Inky))
            {
                allGhosts = GameObject.FindGameObjectsWithTag("Ghost");
                foreach (GameObject ghost in allGhosts)
                {
                    if (ghost.name.Equals("Blinky"))
                    {
                        trackBlinky = ghost;
                    }
                }
            }
        }
        // Update is called once per frame
        void Update()
        {
            if (gameManager.PelletCount == (startingPelletCount - 10))
            {
                isTimerPaused = false;
                isReadyToLeaveHome = true;
            }

            if (PlayerManager.Instance.isPowerMode)
            {
                if (!powerModeActivated)
                {
                    powerModeActivated = true;
                    StartCoroutine(enterFrightenedMode());
                }
            }


            if (!isTimerPaused)
            {
                AlternateGhostModes();
            }

            if (!PlayerManager.Instance.isAlive)
            {
                if (commenceRound)
                {
                    commenceRound = false;
                    StartCoroutine(resetPosition());
                }
            }
        }

        IEnumerator enterFrightenedMode()
        {
            isReadyToLeaveHome = false;
            isTimerPaused = true;
            ghostNodeState = GhostNodeStateEnum.Frightened;

            yield return new WaitForSeconds(10);

            if (ghostNodeState == GhostNodeStateEnum.Frightened)
            {
                ghostNodeState = GhostNodeStateEnum.MovingInNodes;
            }
            isReadyToLeaveHome = true;
            powerModeActivated = false;
            isTimerPaused = true;
        }



        IEnumerator resetPosition()
        {

            isReadyToLeaveHome = false;
            yield return new WaitForSeconds(2);

            startingPelletCount = gameManager.PelletCount;

            GetStartPosition();
            transform.position = startingNode.transform.position;
            //movement.CurrentNode = startingNode;
            yield return new WaitForSeconds(1);
            commenceRound = true;

        }

        void AlternateGhostModes()
        {
            int wave = 1;
            if (wave < 5)
            {
                timer -= Time.deltaTime;
            }
            if (wave == 1 || wave == 2)
            {
                if (timer <= 0 && ghostNodeState == GhostNodeStateEnum.MovingInNodes)
                {
                    //Debug.Log("Scattering!");
                    ghostNodeState = GhostNodeStateEnum.Scatter;
                    directionChanged = false;
                    timer = 7;
                }
                else if (timer <= 0 && ghostNodeState == GhostNodeStateEnum.Scatter)
                {
                    ghostNodeState = GhostNodeStateEnum.MovingInNodes;
                    timer = 20;
                    wave++;
                }
            }
            else if (wave == 3)
            {
                if (timer <= 0 && ghostNodeState == GhostNodeStateEnum.MovingInNodes)
                {
                    //Debug.Log("Scattering!");
                    ghostNodeState = GhostNodeStateEnum.Scatter;
                    directionChanged = false;
                    timer = 5;
                }
                else if (timer <= 0 && ghostNodeState == GhostNodeStateEnum.Scatter)
                {
                    ghostNodeState = GhostNodeStateEnum.MovingInNodes;
                    timer = 20;
                    wave++;
                }
            }
            else if (wave == 4)
            {
                if (timer <= 0 && ghostNodeState == GhostNodeStateEnum.MovingInNodes)
                {
                    //Debug.Log("Scattering!");
                    ghostNodeState = GhostNodeStateEnum.Scatter;
                    directionChanged = false;
                    timer = 5;
                }
                else if (timer <= 0 && ghostNodeState == GhostNodeStateEnum.Scatter)
                {
                    ghostNodeState = GhostNodeStateEnum.MovingInNodes;
                    wave++;
                }
            }

        }

        private void GetStartPosition()
        {
            body.enabled = true;
            body.color = bodyColor;
            if (ghostName.Equals(GhostName.Blinky))
            {
                ghostNodeState = GhostNodeStateEnum.StartNode;
                respawnState = GhostNodeStateEnum.CentreNode;
                startingNode = ghostNodeStart;
            }
            else if (ghostName.Equals(GhostName.Pinky))
            {
                ghostNodeState = GhostNodeStateEnum.CentreNode;
                respawnState = GhostNodeStateEnum.CentreNode;
                startingNode = ghostNodeCentre;
            }
            else if (ghostName.Equals(GhostName.Inky))
            {
                ghostNodeState = GhostNodeStateEnum.LeftNode;
                respawnState = GhostNodeStateEnum.LeftNode;
                startingNode = ghostNodeLeft;
            }
            else if (ghostName.Equals(GhostName.Clive))
            {
                ghostNodeState = GhostNodeStateEnum.RightNode;
                respawnState = GhostNodeStateEnum.RightNode;
                startingNode = ghostNodeRight;
            }
        }

        public void ReachedNodeCentre(NodeScript nodeScript)
        {
            if (ghostNodeState == GhostNodeStateEnum.MovingInNodes)
            {
                body.color = bodyColor;
                if (ghostName == GhostName.Blinky)
                {
                    DetermineBlinkyDirection();
                }
                else if (ghostName == GhostName.Pinky)
                {
                    DeterminePinkyDirection();
                }
                else if (ghostName == GhostName.Inky)
                {
                    DetermineInkyDirection();
                }
                else if (ghostName == GhostName.Clive)
                {
                    DetermineCliveDirection();
                }
            }
            else if (ghostNodeState == GhostNodeStateEnum.Respawning)
            {
                //determine how to go home
                GoBackToPen();
            }
            else if (ghostNodeState == GhostNodeStateEnum.Scatter)
            {
                body.color = bodyColor;
                if (!directionChanged)
                {
                    GetOppositeDirection();
                    directionChanged = true;
                }
                else
                {
                    ScatterToCorner(ghostName);
                }
                //scatter to corner

            }
            else if (ghostNodeState == GhostNodeStateEnum.Frightened)
            {
                Scramble();
            }
            else
            {
                if (isReadyToLeaveHome)
                {
                    if (ghostNodeState == GhostNodeStateEnum.LeftNode)
                    {
                        ghostNodeState = GhostNodeStateEnum.CentreNode;
                        movement.SetDirection(ControlInput.Right);
                    }
                    else if (ghostNodeState == GhostNodeStateEnum.RightNode)
                    {
                        ghostNodeState = GhostNodeStateEnum.CentreNode;
                        movement.SetDirection(ControlInput.Left);
                    }
                    else if (ghostNodeState == GhostNodeStateEnum.CentreNode)
                    {
                        ghostNodeState = GhostNodeStateEnum.StartNode;
                        movement.SetDirection(ControlInput.Up);
                    }
                    else if (ghostNodeState == GhostNodeStateEnum.StartNode)
                    {
                        ghostNodeState = GhostNodeStateEnum.MovingInNodes;
                        movement.SetDirection(ControlInput.Right);
                    }
                }
            }
        }

        void GetOppositeDirection()
        {
            if (movement.LastMove.Equals(ControlInput.Up))
            {
                movement.SetDirection(ControlInput.Down);
            }
            else if (movement.LastMove.Equals(ControlInput.Down))
            {
                movement.SetDirection(ControlInput.Up);
            }
            else if (movement.LastMove.Equals(ControlInput.Left))
            {
                movement.SetDirection(ControlInput.Right);
            }
            else if (movement.LastMove.Equals(ControlInput.Right))
            {
                movement.SetDirection(ControlInput.Left);
            }
        }

        void DetermineBlinkyDirection()
        {
            ControlInput direction = GetClosestDirection(gameManager.PacMan.transform.position);
            movement.SetDirection(direction);
        }
        void DeterminePinkyDirection()
        {
            //get 4 tiles in front of pacman
            Vector3 tilePosition = GetTilesAhead(4);
            ControlInput direction = GetClosestDirection(tilePosition);
            movement.SetDirection(direction);

        }
        void DetermineInkyDirection()
        {
            //get to twice the distance from blinky to pacman
            Vector3 tilePosition = GetDoubeleDistance();
            ControlInput direction = GetClosestDirection(tilePosition);
            movement.SetDirection(direction);
        }

        void DetermineCliveDirection()
        {
            bool isTooClose = GetDistanceFromPacman();
            if (GetDistanceFromPacman())
            {
                movement.SetDirection(GetClosestDirection(cliveCorner));
            }
            else
            {
                movement.SetDirection(GetClosestDirection(gameManager.PacMan.transform.position));
            }
        }

        void ScatterToCorner(GhostName ghostName)
        {
            Vector3 corner = new Vector3();
            if (ghostName == GhostName.Blinky)
            {
                corner = blinkyCorner;
            }
            else if (ghostName == GhostName.Pinky)
            {
                corner = pinkyCorner;
            }
            else if (ghostName == GhostName.Inky)
            {
                corner = inkyCorner;
            }
            else if (ghostName == GhostName.Clive)
            {
                corner = cliveCorner;
            }

            ControlInput direction = GetClosestDirection(corner);
            movement.SetDirection(direction);
        }

        void Scramble()
        {
            body.color = scaredColor;
            NodeScript nodeScript = movement.CurrentNode.GetComponent<NodeScript>();
            ControlInput lastDirection = movement.LastMove;
            List<ControlInput> canMove = new();
            if (nodeScript.CanMoveLeft && lastDirection != ControlInput.Right)
            {
                canMove.Add(ControlInput.Left);
            }
            else if (nodeScript.CanMoveRight && lastDirection != ControlInput.Left)
            {
                canMove.Add(ControlInput.Right);
            }
            else if (nodeScript.CanMoveUp && lastDirection != ControlInput.Down)
            {
                canMove.Add(ControlInput.Up);
            }
            else if (nodeScript.CanMoveDown && lastDirection != ControlInput.Up)
            {
                canMove.Add(ControlInput.Down);
            }

            ControlInput chosenMove = canMove[UnityEngine.Random.Range(0, canMove.Count)];
            movement.SetDirection(chosenMove);
        }

        private ControlInput GetClosestDirection(Vector2 target)
        {
            float shortDistance = 0;
            ControlInput nextDirection = ControlInput.None;
            ControlInput lastDirection = movement.LastMove;
            NodeScript nodeScript = movement.CurrentNode.GetComponent<NodeScript>();

            if (nodeScript.CanMoveUp && lastDirection != ControlInput.Down)
            {
                GameObject nodeUp = nodeScript.NodeUp;
                float distance = Vector2.Distance(nodeUp.transform.position, target);

                if (distance < shortDistance || shortDistance == 0)
                {
                    shortDistance = distance;
                    nextDirection = ControlInput.Up;
                }
            }

            if (nodeScript.CanMoveDown && lastDirection != ControlInput.Up)
            {
                GameObject nodeDown = nodeScript.NodeDown;
                float distance = Vector2.Distance(nodeDown.transform.position, target);

                if (distance < shortDistance || shortDistance == 0)
                {
                    shortDistance = distance;
                    nextDirection = ControlInput.Down;
                }
            }

            if (nodeScript.CanMoveLeft && lastDirection != ControlInput.Right)
            {
                GameObject nodeLeft = nodeScript.NodeLeft;
                float distance = Vector2.Distance(nodeLeft.transform.position, target);

                if (distance < shortDistance || shortDistance == 0)
                {
                    shortDistance = distance;
                    nextDirection = ControlInput.Left;
                }
            }

            if (nodeScript.CanMoveRight && lastDirection != ControlInput.Left)
            {
                GameObject nodeRight = nodeScript.NodeRight;
                float distance = Vector2.Distance(nodeRight.transform.position, target);

                if (distance < shortDistance || shortDistance == 0)
                {
                    shortDistance = distance;
                    nextDirection = ControlInput.Right;
                }
            }

            return nextDirection;
        }

        private Vector3 GetTilesAhead(int tileCount)
        {
            NodeScript nodeScript = playerMovement.CurrentNode.GetComponent<NodeScript>();
            ControlInput lastDirection = playerMovement.LastMove;
            GameObject chosenNode = null;
            for (int i = 0; i < tileCount; i++)
            {
                List<GameObject> canMove = new List<GameObject>();
                if (nodeScript.CanMoveLeft && lastDirection != ControlInput.Right)
                {
                    canMove.Add(nodeScript.NodeLeft);
                }
                else if (nodeScript.CanMoveRight && lastDirection != ControlInput.Left)
                {
                    canMove.Add(nodeScript.NodeRight);
                }
                else if (nodeScript.CanMoveUp && lastDirection != ControlInput.Up)
                {
                    canMove.Add(nodeScript.NodeUp);
                }
                else if (nodeScript.CanMoveDown && lastDirection != ControlInput.Down)
                {
                    canMove.Add(nodeScript.NodeDown);
                }

                chosenNode = canMove[UnityEngine.Random.Range(0, canMove.Count)];
                nodeScript = chosenNode.GetComponent<NodeScript>();
            }
            return chosenNode.transform.position;

        }

        private Vector3 GetDoubeleDistance()
        {
            //get node 2 tiles ahead
            Vector3 halfPosition = GetTilesAhead(2);

            //get distance from blinky
            float xDistance = 2 * (halfPosition.x - trackBlinky.transform.position.x);
            float yDistance = 2 * (halfPosition.y - trackBlinky.transform.position.y);

            Vector3 targetlocation = new Vector3(xDistance, yDistance, 0);
            return targetlocation;

        }

        private bool GetDistanceFromPacman()
        {
            targetPosition = gameManager.PacMan.transform.position;
            //get 8 tile radius from pacman (tiles are 1x1 so 8m)
            float distance = Vector2.Distance(targetPosition, transform.position);
            if (distance <= 8)
            {
                return true;
            }
            return false;
        }

        void GoBackToPen()
        {
            ControlInput direction = ControlInput.None;
            if (transform.position == ghostNodeStart.transform.position)
            {
                direction = ControlInput.Down;
            }
            else if (transform.position == ghostNodeCentre.transform.position)
            {
                if (respawnState == GhostNodeStateEnum.CentreNode)
                {
                    ghostNodeState = respawnState;
                    body.enabled = true;
                }
                else if (respawnState == GhostNodeStateEnum.LeftNode)
                {
                    direction = ControlInput.Left;
                }
                else if (respawnState == GhostNodeStateEnum.RightNode)
                {
                    direction = ControlInput.Right;
                }

            }
            else if (transform.position == ghostNodeLeft.transform.position || transform.position == ghostNodeRight.transform.position)
            {
                ghostNodeState = respawnState;
                body.enabled = true;
            }
            else
            {
                body.enabled = false;
                direction = GetClosestDirection(ghostNodeStart.transform.position);

            }
            movement.SetDirection(direction);
        }
    }
}
