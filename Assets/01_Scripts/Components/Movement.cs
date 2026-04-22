using System;
using UnityEngine;
using Utilities;

namespace CoreSystem
{

    [RequireComponent(typeof(Rigidbody))]
    public class Movement : MonoBehaviour
    {
        [field: SerializeField] public IInputHandler InputHandler { get; protected set; }
        [field: SerializeField] public Rigidbody RB { get; private set; }

        [field: SerializeField] public NodeScript CurrentNode { get; protected set; }

        [field: SerializeField] public ControlInput CurrentDirection = ControlInput.None;
        [field: SerializeField] public ControlInput CachedDirection = ControlInput.None;

        [field: SerializeField] public float Speed { get; private set; }

        protected bool _isPlaying = false;

        protected virtual void Awake()
        {
            InputHandler = GetComponent<IInputHandler>();
            RB = GetComponent<Rigidbody>();
        }

        protected virtual void Update()
        {
            if (!_isPlaying) return;

            ReadInput();
            Move();
        }

        public virtual void OnGameStateUpdated(GameState gameState)
        { 
            _isPlaying = gameState.Equals(GameState.Playing);
        }

        private void ReadInput()
        {
            if (InputHandler.CurrentInput.Equals(ControlInput.None)) return;

            if (!InputHandler.CurrentInput.Equals(CachedDirection))
            {
                CachedDirection = InputHandler.CurrentInput;
            }
        }

        public void SetStartNode(NodeScript startNode)
        {
            CurrentNode = startNode;
            transform.position = startNode.transform.position;
        }

        public void SetSpeed(int levelNumber)
        {
            Speed = Constants.BASE_SPEED + (Constants.SPEED_MULTIPLIER * (levelNumber - 1));
        }

        protected virtual void Move()
        {
            transform.position = Vector3.MoveTowards(transform.position, CurrentNode.transform.position, Speed * Time.deltaTime);
            if (transform.position == CurrentNode.transform.position)
            {
                GetNextDirection();
            }
        }

        protected void GetNextDirection()
        {
            if (CachedDirection.Equals(ControlInput.Up) && CurrentNode.CanMoveUp)
            {
                CurrentDirection = CachedDirection;
                CurrentNode = CurrentNode.NodeUp;
            }
            else if (CachedDirection.Equals(ControlInput.Down) && CurrentNode.CanMoveDown)
            {
                CurrentDirection = CachedDirection;
                CurrentNode = CurrentNode.NodeDown;
            }
            else if (CachedDirection.Equals(ControlInput.Left) && CurrentNode.CanMoveLeft)
            {
                CurrentDirection = CachedDirection;
                CurrentNode = CurrentNode.NodeLeft;
            }
            else if (CachedDirection.Equals(ControlInput.Right) && CurrentNode.CanMoveRight)
            {
                CurrentDirection = CachedDirection;
                CurrentNode = CurrentNode.NodeRight;
            }
            else
            {
                MaintainCurrentDirection();
            }
        }

        private void MaintainCurrentDirection()
        {
            switch (CurrentDirection)
            {
                case ControlInput.Up:
                    if (CurrentNode.CanMoveUp)
                    {
                        CurrentNode = CurrentNode.NodeUp;
                    }
                    break;
                case ControlInput.Down:
                    if (CurrentNode.CanMoveDown)
                    {
                        CurrentNode = CurrentNode.NodeDown;
                    }
                    break;
                case ControlInput.Left:
                    if (CurrentNode.CanMoveLeft)
                    {
                        CurrentNode = CurrentNode.NodeLeft;
                    }
                    break;
                case ControlInput.Right:
                    if (CurrentNode.CanMoveRight)
                    {
                        CurrentNode = CurrentNode.NodeRight;
                    }
                    break;
            }
        }

        protected virtual bool ShouldTeleport()
        {
            if (CurrentNode.NodeType == NodeType.Teleport && transform.position == CurrentNode.transform.position)
            {
                if (CurrentDirection == ControlInput.Left && CurrentNode.TeleportNodeRight != null)
                {
                    transform.position = CurrentNode.TeleportNodeRight.transform.position;
                    CurrentNode = CurrentNode.TeleportNodeRight;
                    return true;
                }
                else if (CurrentDirection == ControlInput.Right && CurrentNode.TeleportNodeLeft != null)
                {
                    transform.position = CurrentNode.TeleportNodeLeft.transform.position;
                    CurrentNode = CurrentNode.TeleportNodeLeft;
                    return true;
                }
            }

            return false;
        }
    }
}