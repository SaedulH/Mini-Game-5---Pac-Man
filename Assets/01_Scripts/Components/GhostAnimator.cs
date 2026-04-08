using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreSystem
{
    public class GhostAnimator : MonoBehaviour
    {

        [SerializeField] public Animator eyesAnim;
        [SerializeField] private GhostManager ghostManager;
        [SerializeField] private Movement movement;

        [SerializeField] private string cachedMove;
        // Start is called before the first frame update
        void Start()
        {
            ghostManager = GetComponent<GhostManager>();
            movement = GetComponent<Movement>();
            eyesAnim = GetComponentInChildren<Animator>();
            cachedMove = movement.cachedMove;
            if (!cachedMove.Equals(""))
            {
                eyesAnim.SetTrigger(cachedMove);
            }
        }

        // Update is called once per frame
        void Update()
        {

            if (eyesAnim.enabled)
            {
                eyesAnim.SetBool("scared", false);
                if (!movement.lastMove.Equals(cachedMove))
                {
                    cachedMove = movement.lastMove;
                    eyesAnim.SetTrigger(cachedMove);
                }
            }
        }
    }
}