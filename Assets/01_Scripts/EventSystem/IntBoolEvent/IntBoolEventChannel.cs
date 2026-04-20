using UnityEngine;

namespace EventSystem
{
    [CreateAssetMenu(menuName = "Events/IntBoolEventChannel")]
    public class IntBoolEventChannel : DualEventChannel<int, bool> { }
}