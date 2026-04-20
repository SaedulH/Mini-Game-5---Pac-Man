using UnityEngine;

namespace EventSystem
{
    [CreateAssetMenu(menuName = "Events/IntFloatEventChannel")]
    public class IntFloatEventChannel : DualEventChannel<int, float> { }
}