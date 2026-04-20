using System;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    public class EntityAnimator : MonoBehaviour
    {
        [field: SerializeField] public Animator Anim { get; protected set; }
        [field: SerializeField] public Movement Movement { get; protected set; }
        [field: SerializeField] public GameObject Body { get; protected set; }
        [field: SerializeField] public ControlInput CurrentDirection { get; protected set; }

        public virtual void SetPowerMode(bool enabled)
        {

        }

        public virtual void SetDeath(bool enabled)
        {

        }
    }
}