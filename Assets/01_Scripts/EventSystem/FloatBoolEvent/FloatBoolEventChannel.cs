using UnityEngine;

namespace EventSystem
{
    [CreateAssetMenu(menuName = "Events/FloatBoolEventChannel")]
    public class FloatBoolEventChannel : DualEventChannel<float, bool> { }
}