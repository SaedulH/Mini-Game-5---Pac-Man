using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace CoreSystem
{
    public class AIInputHandler : MonoBehaviour
    {
        private GameObject pacman;
        private Movement movement;
        private Vector3 targetPosition;
        private float speed = 5;
        private string ghostName = "";

        private string blinky = "Blinky";
        private string inky = "Inky";
        private string pinky = "Pinky";
        private string clive = "Clive";

        public GameObject startingNode;

        // Start is called before the first frame update
        void Start()
        {
            movement = GetComponent<Movement>();
            pacman = GameObject.FindGameObjectWithTag("Player");
        }

        // Update is called once per frame
        void Update()
        {
            GetTargetLocation();
            FindBestDirection();
        }

        public void GetTargetLocation()
        {
            if (name.Equals(blinky))
            {
                //target the player
                targetPosition = pacman.transform.position;
            }
            else if (name.Equals(inky))
            {
                //target the position twice the length of  two tiles in front of player 
                //pacman.transform.position;
            }
            else if (name.Equals(pinky))
            {
                //target 4 tiles in front of player
                //targetPosition = pacman.transform.position;
            }
            else if (name.Equals(clive))
            {
                //target player until 8 tile radius then scatter
                //targetPosition = pacman.transform.position;
            }

        }

        public void FindBestDirection()
        {

        }

    }
}