using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreSystem
{
    public class InputHandler : MonoBehaviour
    {
        private Movement movement;
        private string UP = "up";
        private string DOWN = "down";
        private string LEFT = "left";
        private string RIGHT = "right";

        private void Awake()
        {
            movement = GetComponent<Movement>();
            movement.cachedMove = RIGHT;
        }
        void Update()
        {
            DetectInput();
        }

        void DetectInput()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                movement.SetDirection(UP);

            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                movement.SetDirection(DOWN);

            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                movement.SetDirection(RIGHT);

            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                movement.SetDirection(LEFT);

            }
        }
    }
}