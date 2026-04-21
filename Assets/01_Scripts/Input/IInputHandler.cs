using UnityEngine;
using Utilities;

namespace CoreSystem
{
    public interface IInputHandler
    {
        ControlInput CurrentInput { get; set; }

        void OnGameStateUpdated(GameState gameState);
    }
}