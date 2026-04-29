using UnityEngine;
using Utilities;

namespace EventSystem
{
    [CreateAssetMenu(menuName = "Events/LevelStateEventChannel")]
    public class LevelStateEventChannel : EventChannel<LevelState> { }
}