using UnityEngine;
using Utilities;

namespace CoreSystem
{
    public class PlayerMovement : Movement
    {
        protected override void Move()
        {
            transform.position = Vector3.MoveTowards(transform.position, CurrentNode.transform.position, Speed * Time.deltaTime);
            if (transform.position == CurrentNode.transform.position || AllowQuickDirectionChange())
            {
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