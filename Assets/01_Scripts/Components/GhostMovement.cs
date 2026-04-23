using UnityEngine;
using Utilities;

namespace CoreSystem
{
    public class GhostMovement : Movement
    {
        public new GhostInputHandler InputHandler { get => (GhostInputHandler)base.InputHandler; protected set => base.InputHandler = value; }
        protected override void Awake()
        {
            base.Awake(); 
        }

        protected override void Update()
        {
            if (!_isPlaying) return;

            Move();
        }

        protected override void Move()
        {
            float speed = InputHandler.CurrentState.Equals(GhostState.Returning) ? Constants.GHOST_RETURN_SPEED : Speed;
            transform.position = Vector3.MoveTowards(transform.position, CurrentNode.transform.position, speed * Time.deltaTime);
            if (!ShouldTeleport() && transform.position == CurrentNode.transform.position)
            {
                SetCachedDirection();
                GetNextDirection();
            }
        }

        private void SetCachedDirection()
        {
            CachedDirection = InputHandler.OnReachedCurrentNode(CurrentNode);

            if (CurrentNode.NodeType.Equals(NodeType.GhostStart)
                && !InputHandler.CurrentState.Equals(GhostState.Returning)
                && CachedDirection.Equals(ControlInput.Down)
                && CurrentNode.NodeDown != null)
            {
                Debug.Log("Avoid Returning To Pen");
                CachedDirection = CurrentDirection.Equals(ControlInput.Right) ? ControlInput.Right : ControlInput.Left;
            }
        }
    }
}