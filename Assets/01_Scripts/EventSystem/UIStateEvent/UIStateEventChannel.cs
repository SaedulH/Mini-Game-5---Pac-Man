using UnityEngine;
using Utilities;

namespace EventSystem
{
    [CreateAssetMenu(menuName = "Events/UIStateEventChannel")]
    public class UIStateEventChannel : EventChannel<UIState> { }
}