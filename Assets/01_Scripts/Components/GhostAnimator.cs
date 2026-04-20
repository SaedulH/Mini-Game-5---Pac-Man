using UnityEngine;
using Utilities;

namespace CoreSystem
{
    public class GhostAnimator : EntityAnimator
    {
        [SerializeField] public Animator eyesAnim;
        [SerializeField] private Movement movement;

        [SerializeField] private ControlInput cachedMove;
        // Start is called before the first frame update
        void Start()
        {
            movement = GetComponent<Movement>();
            eyesAnim = GetComponentInChildren<Animator>();
            cachedMove = movement.CachedDirection;
            if (!cachedMove.Equals(""))
            {
                //eyesAnim.SetTrigger(cachedMove.ToString());
            }
        }

        // Update is called once per frame
        void Update()
        {

            if (eyesAnim.enabled)
            {
                //eyesAnim.SetBool("scared", false);
                if (!movement.CurrentDirection.Equals(cachedMove))
                {
                    cachedMove = movement.CurrentDirection;
                    //eyesAnim.SetTrigger(cachedMove.ToString());
                }
            }
        }
    }
}