using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    [RequireComponent(typeof(Movement))]
    public class GhostManager : EntityManager
    {
        [field: SerializeField] public PlayerManager PacMan { get; protected set; }

        public GhostType GhostType;
        public GhostNodeState respawnState;
        public GhostNodeState ghostNodeState;

        [SerializeField] private GameObject ghostNodeLeft;
        [SerializeField] private GameObject ghostNodeRight;
        [SerializeField] private GameObject ghostNodeCentre;
        [SerializeField] private GameObject ghostNodeStart;

        private Vector3 targetPosition;
        private float speed = 1;
        private Color32 bodyColor;
        private Color32 scaredColor = new Color32(0, 34, 255, 255);

        private Vector3 blinkyCorner = new Vector3(12.5f, 14.5f, 0);
        private Vector3 inkyCorner = new Vector3(12.5f, -13.5f, 0);
        private Vector3 pinkyCorner = new Vector3(-12.5f, 14.5f, 0);
        private Vector3 clydeCorner = new Vector3(-12.5f, -13.5f, 0);

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
            InputHandler = GetComponent<GhostInputHandler>();
            Movement = GetComponent<GhostMovement>();
            Anim = GetComponent<GhostAnimator>();
            Skin = GetComponent<SkinHandler>();

            GetStartPosition();
        }

        public void InitialiseGhost(GhostType ghostType, int skinIndex, int totalPelletCount, PlayerManager pacMan)
        {
            GhostType = ghostType;
            //Anim.SetTrigger("Idle");
            Skin.AssignSkin(skinIndex);
            startingPelletCount = totalPelletCount;
            PacMan = pacMan;
        }

        private void Start()
        {
            if (GhostType.Equals(GhostType.Inky))
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
            //if (gameManager.TotalPelletCount == (startingPelletCount - 10))
            //{
            //    isTimerPaused = false;
            //    isReadyToLeaveHome = true;
            //}

            //if (PlayerManager.Instance.isPowerMode)
            //{
            //    if (!powerModeActivated)
            //    {
            //        powerModeActivated = true;
            //        StartCoroutine(enterFrightenedMode());
            //    }
            //}


            if (!isTimerPaused)
            {
                AlternateGhostModes();
            }

            if (!PacMan.isAlive)
            {
                if (commenceRound)
                {
                    commenceRound = false;
                    StartCoroutine(resetPosition());
                }
            }
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
                        Movement.SetStartNode(StartNode);
                        break;
                    }
                }
            }
        }

        IEnumerator enterFrightenedMode()
        {
            isReadyToLeaveHome = false;
            isTimerPaused = true;
            ghostNodeState = GhostNodeState.Frightened;

            yield return new WaitForSeconds(10);

            if (ghostNodeState == GhostNodeState.Frightened)
            {
                ghostNodeState = GhostNodeState.MovingInNodes;
            }
            isReadyToLeaveHome = true;
            powerModeActivated = false;
            isTimerPaused = true;
        }

        IEnumerator resetPosition()
        {
            isReadyToLeaveHome = false;
            yield return new WaitForSeconds(2);

            //startingPelletCount = gameManager.TotalPelletCount;

            GetStartPosition();
            transform.position = StartNode.transform.position;
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
                if (timer <= 0 && ghostNodeState == GhostNodeState.MovingInNodes)
                {
                    //Debug.Log("Scattering!");
                    ghostNodeState = GhostNodeState.Scatter;
                    directionChanged = false;
                    timer = 7;
                }
                else if (timer <= 0 && ghostNodeState == GhostNodeState.Scatter)
                {
                    ghostNodeState = GhostNodeState.MovingInNodes;
                    timer = 20;
                    wave++;
                }
            }
            else if (wave == 3)
            {
                if (timer <= 0 && ghostNodeState == GhostNodeState.MovingInNodes)
                {
                    //Debug.Log("Scattering!");
                    ghostNodeState = GhostNodeState.Scatter;
                    directionChanged = false;
                    timer = 5;
                }
                else if (timer <= 0 && ghostNodeState == GhostNodeState.Scatter)
                {
                    ghostNodeState = GhostNodeState.MovingInNodes;
                    timer = 20;
                    wave++;
                }
            }
            else if (wave == 4)
            {
                if (timer <= 0 && ghostNodeState == GhostNodeState.MovingInNodes)
                {
                    //Debug.Log("Scattering!");
                    ghostNodeState = GhostNodeState.Scatter;
                    directionChanged = false;
                    timer = 5;
                }
                else if (timer <= 0 && ghostNodeState == GhostNodeState.Scatter)
                {
                    ghostNodeState = GhostNodeState.MovingInNodes;
                    wave++;
                }
            }

        }

        private void GetStartPosition()
        {
            if (GhostType.Equals(GhostType.Blinky))
            {
                ghostNodeState = GhostNodeState.StartNode;
                respawnState = GhostNodeState.CentreNode;
            }
            else if (GhostType.Equals(GhostType.Pinky))
            {
                ghostNodeState = GhostNodeState.CentreNode;
                respawnState = GhostNodeState.CentreNode;
            }
            else if (GhostType.Equals(GhostType.Inky))
            {
                ghostNodeState = GhostNodeState.LeftNode;
                respawnState = GhostNodeState.LeftNode;
            }
            else if (GhostType.Equals(GhostType.Clyde))
            {
                ghostNodeState = GhostNodeState.RightNode;
                respawnState = GhostNodeState.RightNode;
            }
        }

        public void ReachedNodeCentre(NodeScript nodeScript)
        {
            if (ghostNodeState == GhostNodeState.MovingInNodes)
            {
                if (GhostType == GhostType.Blinky)
                {
                    DetermineBlinkyDirection();
                }
                else if (GhostType == GhostType.Pinky)
                {
                    DeterminePinkyDirection();
                }
                else if (GhostType == GhostType.Inky)
                {
                    DetermineInkyDirection();
                }
                else if (GhostType == GhostType.Clyde)
                {
                    DetermineClydeDirection();
                }
            }
            else if (ghostNodeState == GhostNodeState.Respawning)
            {
                //determine how to go home
                GoBackToPen();
            }
            else if (ghostNodeState == GhostNodeState.Scatter)
            {
                if (!directionChanged)
                {
                    GetOppositeDirection();
                    directionChanged = true;
                }
                else
                {
                    ScatterToCorner(GhostType);
                }
                //scatter to corner

            }
            else if (ghostNodeState == GhostNodeState.Frightened)
            {
                Scramble();
            }
            else
            {
                if (isReadyToLeaveHome)
                {
                    if (ghostNodeState == GhostNodeState.LeftNode)
                    {
                        ghostNodeState = GhostNodeState.CentreNode;
                        //movement.SetDirection(ControlInput.Right);
                    }
                    else if (ghostNodeState == GhostNodeState.RightNode)
                    {
                        ghostNodeState = GhostNodeState.CentreNode;
                        //movement.SetDirection(ControlInput.Left);
                    }
                    else if (ghostNodeState == GhostNodeState.CentreNode)
                    {
                        ghostNodeState = GhostNodeState.StartNode;
                        //movement.SetDirection(ControlInput.Up);
                    }
                    else if (ghostNodeState == GhostNodeState.StartNode)
                    {
                        ghostNodeState = GhostNodeState.MovingInNodes;
                        //movement.SetDirection(ControlInput.Right);
                    }
                }
            }
        }

        void GetOppositeDirection()
        {
            if (Movement.CurrentDirection.Equals(ControlInput.Up))
            {
                //movement.SetDirection(ControlInput.Down);
            }
            else if (Movement.CurrentDirection.Equals(ControlInput.Down))
            {
                //movement.SetDirection(ControlInput.Up);
            }
            else if (Movement.CurrentDirection.Equals(ControlInput.Left))
            {
                //movement.SetDirection(ControlInput.Right);
            }
            else if (Movement.CurrentDirection.Equals(ControlInput.Right))
            {
                //movement.SetDirection(ControlInput.Left);
            }
        }

        void DetermineBlinkyDirection()
        {
            ControlInput direction = GetClosestDirection(PacMan.transform.position);
            //movement.SetDirection(direction);
        }
        void DeterminePinkyDirection()
        {
            //get 4 tiles in front of pacman
            Vector3 tilePosition = GetTilesAhead(4);
            ControlInput direction = GetClosestDirection(tilePosition);
            //movement.SetDirection(direction);

        }
        void DetermineInkyDirection()
        {
            //get to twice the distance from blinky to pacman
            Vector3 tilePosition = GetDoubleDistance();
            ControlInput direction = GetClosestDirection(tilePosition);
            //movement.SetDirection(direction);
        }

        void DetermineClydeDirection()
        {
            bool isTooClose = GetDistanceFromPacman();
            if (GetDistanceFromPacman())
            {
                //movement.SetDirection(GetClosestDirection(cliveCorner));
            }
            else
            {
                //movement.SetDirection(GetClosestDirection(gameManager.PacMan.transform.position));
            }
        }

        void ScatterToCorner(GhostType GhostType)
        {
            Vector3 corner = new Vector3();
            if (GhostType == GhostType.Blinky)
            {
                corner = blinkyCorner;
            }
            else if (GhostType == GhostType.Pinky)
            {
                corner = pinkyCorner;
            }
            else if (GhostType == GhostType.Inky)
            {
                corner = inkyCorner;
            }
            else if (GhostType == GhostType.Clyde)
            {
                corner = clydeCorner;
            }

            ControlInput direction = GetClosestDirection(corner);
            //movement.SetDirection(direction);
        }

        void Scramble()
        {
            NodeScript nodeScript = Movement.CurrentNode.GetComponent<NodeScript>();
            ControlInput lastDirection = Movement.CurrentDirection;
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
            //movement.SetDirection(chosenMove);
        }

        private ControlInput GetClosestDirection(Vector2 target)
        {
            float shortDistance = 0;
            ControlInput nextDirection = ControlInput.None;
            ControlInput lastDirection = Movement.CurrentDirection;
            NodeScript nodeScript = Movement.CurrentNode;

            if (nodeScript.CanMoveUp && lastDirection != ControlInput.Down)
            {
                NodeScript nodeUp = nodeScript.NodeUp;
                float distance = Vector2.Distance(nodeUp.transform.position, target);

                if (distance < shortDistance || shortDistance == 0)
                {
                    shortDistance = distance;
                    nextDirection = ControlInput.Up;
                }
            }

            if (nodeScript.CanMoveDown && lastDirection != ControlInput.Up)
            {
                NodeScript nodeDown = nodeScript.NodeDown;
                float distance = Vector2.Distance(nodeDown.transform.position, target);

                if (distance < shortDistance || shortDistance == 0)
                {
                    shortDistance = distance;
                    nextDirection = ControlInput.Down;
                }
            }

            if (nodeScript.CanMoveLeft && lastDirection != ControlInput.Right)
            {
                NodeScript nodeLeft = nodeScript.NodeLeft;
                float distance = Vector2.Distance(nodeLeft.transform.position, target);

                if (distance < shortDistance || shortDistance == 0)
                {
                    shortDistance = distance;
                    nextDirection = ControlInput.Left;
                }
            }

            if (nodeScript.CanMoveRight && lastDirection != ControlInput.Left)
            {
                NodeScript nodeRight = nodeScript.NodeRight;
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
            NodeScript nodeScript = PacMan.Movement.CurrentNode.GetComponent<NodeScript>();
            ControlInput lastDirection = PacMan.Movement.CurrentDirection;
            NodeScript chosenNode = null;
            for (int i = 0; i < tileCount; i++)
            {
                List<NodeScript> canMove = new List<NodeScript>();
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

        private Vector3 GetDoubleDistance()
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
            targetPosition = PacMan.transform.position;
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
                if (respawnState == GhostNodeState.CentreNode)
                {
                    ghostNodeState = respawnState;
                }
                else if (respawnState == GhostNodeState.LeftNode)
                {
                    direction = ControlInput.Left;
                }
                else if (respawnState == GhostNodeState.RightNode)
                {
                    direction = ControlInput.Right;
                }

            }
            else if (transform.position == ghostNodeLeft.transform.position || transform.position == ghostNodeRight.transform.position)
            {
                ghostNodeState = respawnState;
                //body.enabled = true;
            }
            else
            {
                // body.enabled = false;
                direction = GetClosestDirection(ghostNodeStart.transform.position);

            }
            //movement.SetDirection(direction);
        }
    }
}
