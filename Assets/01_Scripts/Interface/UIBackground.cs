using UnityEngine.UIElements;
using Utilities;

namespace UserInterface
{
    public class UIBackground : UIScript
    {
        private VisualElement _uiBackground;
        private VisualElement _full;
        private VisualElement _overlay;

        public override void Initialise(UIManager uIManager)
        {
            base.Initialise(uIManager);
            _uiBackground = _root.Q<VisualElement>("UIBackground");

            _full = _uiBackground.Q<VisualElement>("Full");
            _overlay = _uiBackground.Q<VisualElement>("Overlay");
        }

        public void EnableBackground(UIState newUIState, bool isOverlay)
        {
            bool enabled = !newUIState.Equals(UIState.None) && !newUIState.Equals(UIState.HUD);
            if (enabled)
            {
                _uiBackground.RemoveFromClassList("hide");
                if (isOverlay)
                {
                    _full.AddToClassList("hide");
                    _overlay.RemoveFromClassList("hide");
                    IsOverlay = true;
                }
                else
                {
                    IsOverlay = false;
                    _full.RemoveFromClassList("hide");
                    _overlay.AddToClassList("hide");
                }
                IsActive = true;
            }
            else
            {
                _uiBackground.AddToClassList("hide");
                _full.AddToClassList("hide");
                _overlay.AddToClassList("hide");
                IsOverlay = false;
                IsActive = false;
            }
        }
    }
}
