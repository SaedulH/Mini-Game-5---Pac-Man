using UnityEngine;
using UnityEngine.UIElements;
using Utilities;

namespace UserInterface
{
    [RequireComponent(typeof(UIDocument))]
    public abstract class UIScript : MonoBehaviour
    {
        [field: SerializeField] public bool IsActive { get; protected set; } = false;
        [field: SerializeField] public bool IsOverlay { get; protected set; } = false;

        protected VisualElement _root;

        protected virtual void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
        }

        public virtual void Show()
        {
        }

        public virtual void Hide()
        {
        }
    }
}