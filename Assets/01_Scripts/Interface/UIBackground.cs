using UnityEngine.UIElements;
using Utilities;

namespace UserInterface
{
    public class UIBackground : UIScript
    {
        private UIDocument _uiDocument;
        private VisualElement _uiBackground;
        private VisualElement _full;
        private VisualElement _overlay;

        public override void Initialise(UIManager uIManager)
        {
            base.Initialise(uIManager);
            _uiDocument = gameObject.GetComponent<UIDocument>();
            _uiBackground = _root.Q<VisualElement>("UIBackground");

            _full = _uiBackground.Q<VisualElement>("Full");
            _overlay = _uiBackground.Q<VisualElement>("Overlay");
        }

        public void EnableBackground(UIState newUIState, bool isOverlay)
        {
            bool enabled = !newUIState.Equals(UIState.None) && !newUIState.Equals(UIState.HUD);
            if (enabled)
            {
                _uiDocument.sortingOrder = newUIState.Equals(UIState.Loading) ? 3 : 1;
                _uiBackground.RemoveFromClassList("hide");
                if (isOverlay)
                {
                    _full.AddToClassList("hide");
                    _overlay.RemoveFromClassList("hide");
                    IsOverlay = true;
                }
                else
                {
                    _full.RemoveFromClassList("hide");
                    _overlay.AddToClassList("hide");
                    IsOverlay = false;
                }
                IsActive = true;
            }
            else
            {
                _uiDocument.sortingOrder = 1;
                _uiBackground.AddToClassList("hide");
                _full.AddToClassList("hide");
                _overlay.AddToClassList("hide");
                IsOverlay = false;
                IsActive = false;
            }
        }
    }
}
