using UnityEngine;
using Utilities;

namespace CoreSystem
{
    public class PlayerAnimator : MonoBehaviour
    {

        [SerializeField] private Animator playerAnim;
        [SerializeField] private Movement movement;

        private ControlInput cachedMove;

        // Start is called before the first frame update
        void Start()
        {
            playerAnim = GetComponent<Animator>();
            movement = GetComponent<Movement>();
            cachedMove = movement.CachedDirection;
            if (!cachedMove.Equals(""))
            {
                playerAnim.SetTrigger(cachedMove.ToString());
            }

        }

        // Update is called once per frame
        void Update()
        {
            GetDirection();
        }


        private void GetDirection()
        {
            if (!movement.CurrentDirection.Equals(cachedMove))
            {
                cachedMove = movement.CurrentDirection;
                //playerAnim.SetTrigger(cachedMove.ToString());
            }
        }
    }
}