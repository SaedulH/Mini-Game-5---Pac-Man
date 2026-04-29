using UnityEngine;
using Utilities;

namespace EventSystem
{
    [CreateAssetMenu(menuName = "Events/GameStateEventChannel")]
    public class GameStateEventChannel : EventChannel<GameState> { }
}