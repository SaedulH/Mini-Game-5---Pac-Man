using UnityEngine;
using Utilities;

namespace CoreSystem
{
    public class PlayerMovement : Movement
    {
        public override void SetCurrentLevelSpeed(int levelNumber)
        {
            Speed = Constants.BASE_PACMAN_SPEED + (Constants.LEVEL_SPEED_MULTIPLIER * (levelNumber - 1));
        }

        protected override void Move()
        {
            transform.position = Vector3.MoveTowards(transform.position, CurrentNode.transform.position, Speed * Time.deltaTime);
            if (!ShouldTeleport() && (transform.position == CurrentNode.transform.position || AllowQuickDirectionChange()))
            {
                if ((CurrentNode.NodeType == NodeType.GhostStart && CachedDirection == ControlInput.Down))
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

                GetNextDirection();
            }
        }

        private bool AllowQuickDirectionChange()
        {
            if ((CurrentDirection == ControlInput.Right && CachedDirection == ControlInput.Left) ||
                (CurrentDirection == ControlInput.Left && CachedDirection == ControlInput.Right) ||
                (CurrentDirection == ControlInput.Up && CachedDirection == ControlInput.Down) ||
                (CurrentDirection == ControlInput.Down && CachedDirection == ControlInput.Up))
            {
                return true;
            }
            return false;
        }
    }
}