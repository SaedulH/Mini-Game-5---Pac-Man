using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    public class GhostInputHandler : MonoBehaviour, IInputHandler
    {
        public ControlInput CurrentInput { get; set; }
        [field: SerializeField] public GhostType Type { get; private set; }
        [field: SerializeField] public GhostConfig Config { get; private set; }
        [field: SerializeField] public GhostState CurrentState { get; private set; }
        [field: SerializeField] public NodeScript RespawnNode { get; set; }

        [SerializeField] private PlayerManager _pacMan;
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private Vector3 _corner = new();
        [SerializeField] private bool _hasChangedDirection = false;
        [SerializeField] private int _pelletsToExitPen = 0;
        [SerializeField] private bool _canExitPen = false;
        [SerializeField] private float _timerToExitPen = 0f;
        [SerializeField] private bool _isActive = false;

        private void Update()
        {
            if (!_isActive) return;

            if (!_canExitPen && CurrentState.Equals(GhostState.Waiting))
            {
                _timerToExitPen -= Time.deltaTime;
                if (_timerToExitPen < 0f)
                {
                    _canExitPen = true;
                }
            }
        }

        #region Setters
        public void SetTargets(Transform target, PlayerManager pacMan)
        {
            _targetTransform = target;
            _pacMan = pacMan;
        }
        private void SetNewInput(ControlInput newInput)
        {
            if (CurrentInput.Equals(newInput)) return;
            //Debug.Log($"Changing input from: [{CurrentInput}] to: [{newInput}]");
            CurrentInput = newInput;
        }

        public void SetGhostType(GhostType type, GhostConfig config)
        {
            Config = config;
            Type = type;

            _corner = config.Corner;
            _timerToExitPen = config.StartTimer;
            _pelletsToExitPen = config.StartPellets;
            _canExitPen = config.CanExitImmediately;
            SetNewInput(config.InitialInput);
        }

        public void SetEndExitVariables()
        {
            _pelletsToExitPen = Config.EndPellets;
            _timerToExitPen = Config.EndTimer;
            _canExitPen = false;
            SetNewInput(Config.InitialInput);
        }

        public bool AllowExitPenEarly()
        {
            if (CurrentState.Equals(GhostState.Waiting))
            {
                _canExitPen = true;
                return true;
            }

            return false;
        }

        #endregion

        public void SetNewGhostState(GhostState newState)
        {
            switch (newState)
            {
                case GhostState.Chasing:
                    break;
                case GhostState.Scattering:
                    _hasChangedDirection = false;
                    break;
                case GhostState.Frightened:
                    _hasChangedDirection = false;
                    break;
                case GhostState.Waiting:
                    _canExitPen = false;
                    break;
            }
            Debug.Log($"{gameObject.name}: Changing state from: [{CurrentState}] to: [{newState}]");
            CurrentState = newState;
        }

        public void OnGameStateUpdated(GameState gameState)
        {
            _isActive = gameState.Equals(GameState.Playing);
        }

        public void OnLevelStateUpdated(LevelState levelState)
        {
        }

        public ControlInput OnReachedCurrentNode(NodeScript currentNode)
        {
            switch (CurrentState)
            {
                case GhostState.Chasing:
                    DetermineDirection(currentNode);
                    break;
                case GhostState.Scattering:
                    ScatterToCorner(currentNode);
                    break;
                case GhostState.Frightened:
                    Scramble(currentNode);
                    break;
                case GhostState.Returning:
                    ReturnToPen(currentNode);
                    break;
                case GhostState.Waiting:
                    ExitPen(currentNode);
                    break;
            }

            return CurrentInput;
        }

        public bool IsEnoughPelletsToExit(int collectedPellets)
        {
            if (collectedPellets >= _pelletsToExitPen)
            {
                _canExitPen = true;
                return true;
            }

            return false;
        }

        #region Determine Direction Methods

        private void DetermineDirection(NodeScript currentNode)
        {
            switch (Type)
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
                case GhostType.Clive:
                    DetermineCliveDirection(currentNode);
                    break;
            }
        }

        void DetermineBlinkyDirection(NodeScript currentNode)
        {
            //Get direction closest to pacman
            SetNewInput(GetClosestDirection(_targetTransform.position, currentNode));
        }

        void DeterminePinkyDirection(NodeScript currentNode)
        {
            // Get Node 4 Tiles Ahead
            Vector3 tilePosition = GetTilesAheadOfPacMan(4);
            SetNewInput(GetClosestDirection(tilePosition, currentNode));
        }

        void DetermineInkyDirection(NodeScript currentNode)
        {
            // Get to twice the distance from Blinky to Pacman
            // Get Node 2 Tiles Ahead
            Vector3 halfPosition = GetTilesAheadOfPacMan(2);

            //get distance from blinky
            float xDistance = 2 * (halfPosition.x - _targetTransform.transform.position.x);
            float zDistance = 2 * (halfPosition.z - _targetTransform.transform.position.z);

            Vector3 tilePosition = new(xDistance, 1f, zDistance);
            SetNewInput(GetClosestDirection(tilePosition, currentNode));
        }

        void DetermineCliveDirection(NodeScript currentNode)
        {
            float distance = Vector3.Distance(_targetTransform.position, transform.position);
            if (distance <= 8f)
            {
                SetNewInput(GetClosestDirection(_corner, currentNode));
            }
            else
            {
                SetNewInput(GetClosestDirection(_targetTransform.position, currentNode));
            }
        }

        private ControlInput GetClosestDirection(Vector3 target, NodeScript currentNode)
        {
            float shortestDistance = 0f;
            ControlInput nextDirection = ControlInput.None;

            if (currentNode.CanMoveUp && !CurrentInput.Equals(ControlInput.Down))
            {
                float distance = Vector3.Distance(currentNode.NodeUp.transform.position, target);

                if (distance < shortestDistance || shortestDistance == 0f)
                {
                    shortestDistance = distance;
                    nextDirection = ControlInput.Up;
                }
            }

            if (currentNode.CanMoveDown && !CurrentInput.Equals(ControlInput.Up))
            {
                float distance = Vector3.Distance(currentNode.NodeDown.transform.position, target);

                if (distance < shortestDistance || shortestDistance == 0f)
                {
                    shortestDistance = distance;
                    nextDirection = ControlInput.Down;
                }
            }

            if (currentNode.CanMoveLeft && !CurrentInput.Equals(ControlInput.Right))
            {
                float distance = Vector3.Distance(currentNode.NodeLeft.transform.position, target);

                if (distance < shortestDistance || shortestDistance == 0f)
                {
                    shortestDistance = distance;
                    nextDirection = ControlInput.Left;
                }
            }

            if (currentNode.CanMoveRight && !CurrentInput.Equals(ControlInput.Left))
            {
                float distance = Vector3.Distance(currentNode.NodeRight.transform.position, target);

                if (distance < shortestDistance || shortestDistance == 0f)
                {
                    nextDirection = ControlInput.Right;
                }
            }

            return nextDirection;
        }

        private Vector3 GetTilesAheadOfPacMan(int tileCount)
        {
            if (_pacMan == null) return Vector3.zero;

            ControlInput pacManDirection = _pacMan.Movement.CurrentDirection;
            NodeScript currentNode = _pacMan.Movement.CurrentNode;
            NodeScript chosenNode = null;
            for (int i = 0; i < tileCount; i++)
            {
                List<NodeScript> canMove = new List<NodeScript>();
                if (currentNode.CanMoveLeft && !pacManDirection.Equals(ControlInput.Right))
                {
                    canMove.Add(currentNode.NodeLeft);
                }
                if (currentNode.CanMoveRight && !pacManDirection.Equals(ControlInput.Left))
                {
                    canMove.Add(currentNode.NodeRight);
                }
                if (currentNode.CanMoveUp && !pacManDirection.Equals(ControlInput.Down))
                {
                    canMove.Add(currentNode.NodeUp);
                }
                if (currentNode.CanMoveDown && !pacManDirection.Equals(ControlInput.Up))
                {
                    canMove.Add(currentNode.NodeDown);
                }

                if (canMove.Count < 0)
                {
                    chosenNode = canMove[Random.Range(0, canMove.Count)];
                    currentNode = chosenNode.GetComponent<NodeScript>();
                } 
                else
                {
                    return currentNode.transform.position;
                } 
            }
            return chosenNode.transform.position;
        }

        void GetOppositeDirection()
        {
            switch (CurrentInput)
            {
                case ControlInput.Up:
                    SetNewInput(ControlInput.Down);
                    break;
                case ControlInput.Down:
                    SetNewInput(ControlInput.Up);
                    break;
                case ControlInput.Left:
                    SetNewInput(ControlInput.Right);
                    break;
                case ControlInput.Right:
                    SetNewInput(ControlInput.Left);
                    break;
            }
        }

        private void ScatterToCorner(NodeScript currentNode)
        {
            if (!_hasChangedDirection)
            {
                GetOppositeDirection();
                _hasChangedDirection = true;
                return;
            }

            SetNewInput(GetClosestDirection(_corner, currentNode));
        }

        private void ExitPen(NodeScript currentNode)
        {
            if (!_canExitPen) return;

            if (!currentNode.NodeType.Equals(NodeType.GhostStart))
            {
                SetNewGhostState(GhostState.Chasing);
                return;
            }

            if (currentNode.CanMoveUp)
            {
                SetNewInput(ControlInput.Up);
            }
            else if (currentNode.CanMoveLeft)
            {
                SetNewInput(ControlInput.Left);
            }
            else if (currentNode.CanMoveRight)
            {
                SetNewInput(ControlInput.Right);
            }
        }

        protected virtual void Scramble(NodeScript currentNode)
        {
            List<ControlInput> canMove = new();
            if (currentNode.CanMoveLeft && !CurrentInput.Equals(ControlInput.Right))
            {
                canMove.Add(ControlInput.Left);
            }
            if (currentNode.CanMoveRight && !CurrentInput.Equals(ControlInput.Left))
            {
                canMove.Add(ControlInput.Right);
            }
            if (currentNode.CanMoveUp && !CurrentInput.Equals(ControlInput.Down))
            {
                canMove.Add(ControlInput.Up);
            }
            if (currentNode.CanMoveDown && !CurrentInput.Equals(ControlInput.Up))
            {
                canMove.Add(ControlInput.Down);
            }

            if (!_hasChangedDirection || canMove.Count == 0)
            {
                GetOppositeDirection();
                _hasChangedDirection = true;
                return;
            }

            SetNewInput(canMove[Random.Range(0, canMove.Count)]);
        }

        void ReturnToPen(NodeScript currentNode)
        {
            if (currentNode.NodeType.Equals(NodeType.GhostStart))
            {
                if (currentNode == RespawnNode)
                {
                    _timerToExitPen = Constants.RESPAWN_DELAY_DURATION;
                    SetNewGhostState(GhostState.Waiting);
                    return;
                }

                if (currentNode.CanMoveDown)
                {
                    SetNewInput(ControlInput.Down);
                }
                else if (currentNode.CanMoveLeft && currentNode.NodeLeft == RespawnNode)
                {
                    SetNewInput(ControlInput.Left);
                }
                else if (currentNode.CanMoveRight && currentNode.NodeRight == RespawnNode)
                {
                    SetNewInput(ControlInput.Right);
                }
                return;
            }

            SetNewInput(GetClosestDirection(RespawnNode.transform.position, currentNode));
        }
    }

        #endregion
}