using UnityEngine;
using Utilities;

namespace CoreSystem
{
    public interface IInputHandler
    {
        ControlInput CachedInput { get; set; }

        void SetInputActions(PlayerInputActions inputActions);
    }
}