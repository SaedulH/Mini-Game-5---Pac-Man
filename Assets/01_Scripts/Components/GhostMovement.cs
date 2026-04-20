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

        protected override void Move()
        {
            transform.position = Vector3.MoveTowards(transform.position, CurrentNode.transform.position, Speed * Time.deltaTime);
            if (!ShouldTeleport() && transform.position == CurrentNode.transform.position)
            {
                if ((CurrentNode.NodeType == NodeType.GhostStart && CachedDirection == ControlInput.Down)
                    && (!InputHandler.CurrentState.Equals(GhostState.Respawning)))
                {
                    if (CurrentDirection == ControlInput.Right)
                    {
                        CurrentNode = CurrentNode.NodeRight;
                    }
                    else
                    {
                        CurrentNode = CurrentNode.NodeLeft;
                    }
                }

                InputHandler.OnReachedNodeCentre(CurrentNode);
                GetNextDirection();
            }
        }

    }
}