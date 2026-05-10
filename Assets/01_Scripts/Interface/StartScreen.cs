using AudioSystem;
using CoreSystem;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;

namespace UserInterface
{
    [RequireComponent(typeof(UIDocument))]
    public class StartScreen : UIScript
    {
        private VisualElement _startScreen;

        private Button _start;
        private Button _settings;
        private Button _quit;

        public override void Initialise(UIManager uIManager)
        {
            base.Initialise(uIManager);
            _startScreen = _root.Q<VisualElement>("StartScreen");

            _start = _root.Q<Button>("Start");
            _start.clicked += OnStartClicked;

            _settings = _root.Q<Button>("Settings");
            _settings.clicked += OnSettingsClicked;

            _quit = _root.Q<Button>("Quit");
            _quit.clicked += OnQuitClicked;
            AudioCollection.Instance.SetupHoverAudio(_startScreen);
        }

        public override void Show()
        {
            base.Show();
            if (IsActive) return;

            _startScreen.RemoveFromClassList("hide");
            IsActive = true;
        }

        public override void Hide()
        {
            base.Hide();
            if (!IsActive) return;

            _startScreen.AddToClassList("hide");
            IsActive = false;
        }

        private void OnStartClicked()
        {
            Debug.Log("Start clicked");
            AudioManager.Instance.CreateAudioBuilder()
                .WithVolume(0.8f)
                .Play(AudioCollection.Instance.StartAudio);
            // Load scene, start game, etc.
            GameManager.Instance.InitialiseLevel();
        }

        private void OnSettingsClicked()
        {
            Debug.Log("Settings clicked");
            AudioCollection.Instance.PlaySelectAudio();
            _uiManager.OnUIStateChanged(UIState.Settings);
        }

        public void OnQuitClicked()
        {
            AudioCollection.Instance.PlayBackAudio();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}
