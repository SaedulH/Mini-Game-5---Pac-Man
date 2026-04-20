using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    public class GhostInputHandler : MonoBehaviour, IInputHandler
    {
        public ControlInput CachedInput { get; set; }
        [field: SerializeField] public GhostType GhostType { get; private set; }
        [field: SerializeField] public GhostState CurrentState { get; private set; }
        [field: SerializeField] public NodeScript StartNode { get; private set; }

        private PlayerManager _pacMan;
        private Transform _targetTransform;
        private Vector3 _corner = new();
        private bool _hasChangedDirection = false;
        private bool _canExit = false;

        #region Setters
        public void SetTargets(Transform target, PlayerManager pacMan)
        {
            _targetTransform = target;
            _pacMan = pacMan;
        }

        public void SetStartNode(NodeScript startNode)
        {
            StartNode = startNode;
        }

        public void SetGhostType(GhostType ghostType)
        {
            GhostType = ghostType;
            switch (ghostType)
            {
                case GhostType.Blinky:
                    _corner = Constants.BLINKY_CORNER_POSITION;
                    break;
                case GhostType.Pinky:
                    _corner = Constants.PINKY_CORNER_POSITION;
                    break;
                case GhostType.Inky:
                    _corner = Constants.INKY_CORNER_POSITION;
                    break;
                case GhostType.Clyde:
                    _corner = Constants.CLYDE_CORNER_POSITION;
                    break;
            }
        }

        #endregion

        public void SetNewGhostState(GhostState newState)
        {
            CurrentState = newState;
            switch (newState)
            {
                case GhostState.Chasing:
                    _canExit = true;
                    break;
                case GhostState.Scatter:
                    _canExit = true;
                    _hasChangedDirection = false;
                    break;
                case GhostState.Frightened:
                    _canExit = false;
                    break;
            }
        }

        public void SetFrightenedState(bool isFrightened)
        {
            CurrentState = isFrightened
                ? GhostState.Frightened : CurrentState.Equals(GhostState.Frightened)
                ? GhostState.Frightened : CurrentState;
            _canExit = !isFrightened;
        }

        public void OnReachedNodeCentre(NodeScript currentNode)
        {
            switch (CurrentState)
            {
                case GhostState.Chasing:
                    DetermineDirection(currentNode);
                    break;
                case GhostState.Scatter:
                    ScatterToCorner(currentNode);
                    break;
                case GhostState.Frightened:
                    Scramble(currentNode);
                    break;
                case GhostState.Respawning:
                    GoBackToPen(currentNode);
                    break;
                case GhostState.Standby:
                    ExitPen(currentNode);
                    break;
            }
        }

        #region Determine Direction Methods

        private void DetermineDirection(NodeScript currentNode)
        {
            switch (GhostType)
            {
                case GhostType.Blinky:
                    DetermineBlinkyDirection(currentNode);
                    break;
                case GhostType.Pinky:
                    DeterminePinkyDirection(currentNode);
                    break;
                case GhostType.Inky:
                    DetermineInkyDirection(currentNode);
                    break;
                case GhostType.Clyde:
                    DetermineCliveDirection(currentNode);
                    break;
            }
        }

        void DetermineBlinkyDirection(NodeScript currentNode)
        {
            //Get direction closest to pacman
            CachedInput = GetClosestDirection(_targetTransform.position, currentNode);
        }

        void DeterminePinkyDirection(NodeScript currentNode)
        {
            //Get 4 tiles in front of pacman
            Vector3 tilePosition = GetTilesAheadOfPacMan(4);
            CachedInput = GetClosestDirection(tilePosition, currentNode);
        }

        void DetermineInkyDirection(NodeScript currentNode)
        {
            //Get to twice the distance from blinky to pacman
            Vector3 tilePosition = GetDoubleDistanceFromPacMan();
            CachedInput = GetClosestDirection(tilePosition, currentNode);
        }

        void DetermineCliveDirection(NodeScript currentNode)
        {
            if (GetDistanceFromTarget())
            {
                CachedInput = GetClosestDirection(_corner, currentNode);
            }
            else
            {
                CachedInput = GetClosestDirection(_targetTransform.position, currentNode);
            }
        }

        private ControlInput GetClosestDirection(Vector3 target, NodeScript currentNode)
        {
            float shortDistance = 0;
            ControlInput nextDirection = ControlInput.None;
            NodeScript nodeScript = currentNode;

            if (nodeScript.CanMoveUp && !CachedInput.Equals(ControlInput.Down))
            {
                NodeScript nodeUp = nodeScript.NodeUp;
                float distance = Vector3.Distance(nodeUp.transform.position, target);

                if (distance < shortDistance || shortDistance == 0)
                {
                    shortDistance = distance;
                    nextDirection = ControlInput.Up;
                }
            }

            if (nodeScript.CanMoveDown && !CachedInput.Equals(ControlInput.Up))
            {
                NodeScript nodeDown = nodeScript.NodeDown;
                float distance = Vector2.Distance(nodeDown.transform.position, target);

                if (distance < shortDistance || shortDistance == 0)
                {
                    shortDistance = distance;
                    nextDirection = ControlInput.Down;
                }
            }

            if (nodeScript.CanMoveLeft && !CachedInput.Equals(ControlInput.Right))
            {
                NodeScript nodeLeft = nodeScript.NodeLeft;
                float distance = Vector2.Distance(nodeLeft.transform.position, target);

                if (distance < shortDistance || shortDistance == 0)
                {
                    shortDistance = distance;
                    nextDirection = ControlInput.Left;
                }
            }

            if (nodeScript.CanMoveRight && !CachedInput.Equals(ControlInput.Left))
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

        private Vector3 GetTilesAheadOfPacMan(int tileCount)
        {
            if (_pacMan == null) return Vector3.zero;

            NodeScript currentNode = _pacMan.Movement.CurrentNode;
            NodeScript chosenNode = null;
            for (int i = 0; i < tileCount; i++)
            {
                List<NodeScript> canMove = new List<NodeScript>();
                if (currentNode.CanMoveLeft && !CachedInput.Equals(ControlInput.Right))
                {
                    canMove.Add(currentNode.NodeLeft);
                }
                else if (currentNode.CanMoveRight && !CachedInput.Equals(ControlInput.Left))
                {
                    canMove.Add(currentNode.NodeRight);
                }
                else if (currentNode.CanMoveUp && !CachedInput.Equals(ControlInput.Down))
                {
                    canMove.Add(currentNode.NodeUp);
                }
                else if (currentNode.CanMoveDown && !CachedInput.Equals(ControlInput.Up))
                {
                    canMove.Add(currentNode.NodeDown);
                }

                chosenNode = canMove[Random.Range(0, canMove.Count)];
                currentNode = chosenNode.GetComponent<NodeScript>();
            }
            return chosenNode.transform.position;
        }

        private Vector3 GetDoubleDistanceFromPacMan()
        {
            //get node 2 tiles ahead
            Vector3 halfPosition = GetTilesAheadOfPacMan(2);

            //get distance from blinky
            float xDistance = 2 * (halfPosition.x - _targetTransform.transform.position.x);
            float yDistance = 2 * (halfPosition.y - _targetTransform.transform.position.y);

            Vector3 targetlocation = new Vector3(xDistance, yDistance, 0);
            return targetlocation;

        }

        private bool GetDistanceFromTarget()
        {
            //get 8 tile radius from pacman (tiles are 1x1 so 8m)
            float distance = Vector2.Distance(_targetTransform.position, transform.position);
            if (distance <= 8)
            {
                return true;
            }
            return false;
        }

        #endregion

        void GetOppositeDirection()
        {
            switch (CachedInput)
            {
                case ControlInput.Up:
                    CachedInput = ControlInput.Down;
                    break;
                case ControlInput.Down:
                    CachedInput = ControlInput.Up;
                    break;
                case ControlInput.Left:
                    CachedInput = ControlInput.Right;
                    break;
                case ControlInput.Right:
                    CachedInput = ControlInput.Left;
                    break;
            }
        }

        private void ScatterToCorner(NodeScript currentNode)
        {
            if (!_hasChangedDirection)
            {
                GetOppositeDirection();
                _hasChangedDirection = true;
            }
            else
            {
                CachedInput = GetClosestDirection(_corner, currentNode);
            }
        }

        private void ExitPen(NodeScript currentNode)
        {
            if (!_canExit) return;

            if (currentNode.CanMoveUp)
            {
                CachedInput = ControlInput.Up;
            }
            else if (currentNode.CanMoveLeft)
            {
                CachedInput = ControlInput.Left;

            }
            else if (currentNode.CanMoveRight)
            {
                CachedInput = ControlInput.Right;
            }
        }

        protected virtual void Scramble(NodeScript currentNode)
        {
            List<ControlInput> canMove = new();
            if (currentNode.CanMoveLeft && !CachedInput.Equals(ControlInput.Right))
            {
                canMove.Add(ControlInput.Left);
            }
            else if (currentNode.CanMoveRight && !CachedInput.Equals(ControlInput.Left))
            {
                canMove.Add(ControlInput.Right);
            }
            else if (currentNode.CanMoveUp && !CachedInput.Equals(ControlInput.Down))
            {
                canMove.Add(ControlInput.Up);
            }
            else if (currentNode.CanMoveDown && !CachedInput.Equals(ControlInput.Up))
            {
                canMove.Add(ControlInput.Down);
            }

            CachedInput = canMove[Random.Range(0, canMove.Count)];
        }

        void GoBackToPen(NodeScript currentNode)
        {
            CachedInput = GetClosestDirection(StartNode.transform.position, currentNode);


            //if (transform.position == StartNode.transform.position)
            //{
            //    CachedInput = ControlInput.Down;
            //}
            //else if (transform.position == ghostNodeCentre.transform.position)
            //{
            //    if (StartPenNode.Equals(GhostPenNode.CentreNode))
            //    {
            //        CurrentState = GhostState.Standby;
            //    }
            //    else if (StartPenNode.Equals(GhostPenNode.LeftNode))
            //    {
            //        CachedInput = ControlInput.Left;
            //    }
            //    else if (StartPenNode.Equals(GhostPenNode.RightNode))
            //    {
            //        CachedInput = ControlInput.Right;
            //    }

            //}
            //else if (transform.position == ghostNodeLeft.transform.position || transform.position == ghostNodeRight.transform.position)
            //{
            //    ghostNodeState = respawnState;
            //}
            //else
            //{
            //    CachedInput = GetClosestDirection(StartNode.transform.position, currentNode);
            //}
        }
    }
}