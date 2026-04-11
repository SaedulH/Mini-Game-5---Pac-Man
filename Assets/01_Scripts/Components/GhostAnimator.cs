using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    public class GhostAnimator : MonoBehaviour
    {

        [SerializeField] public Animator eyesAnim;
        [SerializeField] private GhostManager ghostManager;
        [SerializeField] private Movement movement;

        [SerializeField] private ControlInput cachedMove;
        // Start is called before the first frame update
        void Start()
        {
            ghostManager = GetComponent<GhostManager>();
            movement = GetComponent<Movement>();
            eyesAnim = GetComponentInChildren<Animator>();
            cachedMove = movement.CachedMove;
            if (!cachedMove.Equals(""))
            {
                eyesAnim.SetTrigger(cachedMove.ToString());
            }
        }

        // Update is called once per frame
        void Update()
        {

            if (eyesAnim.enabled)
            {
                eyesAnim.SetBool("scared", false);
                if (!movement.LastMove.Equals(cachedMove))
                {
                    cachedMove = movement.LastMove;
                    eyesAnim.SetTrigger(cachedMove.ToString());
                }
            }
        }
    }
}