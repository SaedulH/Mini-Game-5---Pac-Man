using UnityEngine;
using Utilities;

namespace CoreSystem
{
    public class GhostInputHandler : MonoBehaviour, IInputHandler
    {
        public ControlInput CachedInput { get; set; }
    }
}