using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreSystem
{
    public class PlayerAnimator : MonoBehaviour
    {

        [SerializeField] private Animator playerAnim;
        [SerializeField] private Movement movement;

        private string cachedMove;

        // Start is called before the first frame update
        void Start()
        {
            playerAnim = GetComponent<Animator>();
            movement = GetComponent<Movement>();
            cachedMove = movement.cachedMove;
            if (!cachedMove.Equals(""))
            {
                playerAnim.SetTrigger(cachedMove);
            }

        }

        // Update is called once per frame
        void Update()
        {
            GetDirection();
        }


        private void GetDirection()
        {
            if (!movement.lastMove.Equals(cachedMove))
            {
                cachedMove = movement.lastMove;
                playerAnim.SetTrigger(cachedMove);
            }
        }
    }
}