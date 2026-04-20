using UnityEngine;

namespace EventSystem
{
    [CreateAssetMenu(menuName = "Events/IntStringEventChannel")]
    public class IntStringEventChannel : DualEventChannel<int, string> { }
}