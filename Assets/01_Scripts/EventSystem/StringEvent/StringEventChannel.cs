using UnityEngine;
using Utilities;

namespace EventSystem
{
    [CreateAssetMenu(menuName = "Events/StringEventChannel")]
    public class StringEventChannel : EventChannel<string> { }
}