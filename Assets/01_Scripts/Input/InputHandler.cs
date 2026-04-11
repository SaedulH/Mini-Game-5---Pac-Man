using UnityEngine;
using Utilities;

namespace CoreSystem
{
    public class InputHandler : MonoBehaviour
    {
        private Movement movement;

        private void Awake()
        {
            movement = GetComponent<Movement>();
            movement.CachedMove = ControlInput.Right;
        }

        void Update()
        {
            DetectInput();
        }

        void DetectInput()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                movement.SetDirection(ControlInput.Up);

            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                movement.SetDirection(ControlInput.Down);

            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                movement.SetDirection(ControlInput.Right);

            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                movement.SetDirection(ControlInput.Left);

            }
        }
    }
}