using UnityEngine;
using Utilities;

namespace EventSystem
{
    [CreateAssetMenu(menuName = "Events/StringArrayEventChannel")]
    public class StringArrayEventChannel : EventChannel<string[]> { }
}