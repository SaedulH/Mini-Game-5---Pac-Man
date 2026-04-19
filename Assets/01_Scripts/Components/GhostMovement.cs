using UnityEngine;
using Utilities;

namespace CoreSystem
{
    public class GhostMovement : Movement
    {
        [field: SerializeField] public GhostManager GhostManager { get; private set; }
        protected override void Awake()
        {
            base.Awake(); 
            GhostManager = GetComponent<GhostManager>();
        }

        protected override void Move()
        {
            transform.position = Vector3.MoveTowards(transform.position, CurrentNode.transform.position, Speed * Time.deltaTime);
            if (!ShouldTeleport() && transform.position == CurrentNode.transform.position)
            {
                if ((CurrentNode.NodeType == NodeType.GhostStart && CachedDirection == ControlInput.Down)
                    && (GhostManager.ghostNodeState != GhostNodeState.Respawning))
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

                GhostManager.ReachedNodeCentre(CurrentNode);

                GetNextDirection();
            }
        }

    }
}