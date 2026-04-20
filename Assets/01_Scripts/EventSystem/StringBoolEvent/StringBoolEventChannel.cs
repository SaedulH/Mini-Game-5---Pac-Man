using UnityEngine;
using Utilities;

namespace EventSystem
{
    [CreateAssetMenu(menuName = "Events/StringBoolEventChannel")]
    public class StringBoolEventChannel : DualEventChannel<string, bool> { }
}