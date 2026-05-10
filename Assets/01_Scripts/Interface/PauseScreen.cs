using AudioSystem;
using CoreSystem;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;

namespace UserInterface
{
    [RequireComponent(typeof(UIDocument))]
    public class PauseScreen : UIScript
    {
        private VisualElement _pauseScreen;

        private Button _resume;
        private Button _restart;
        private Button _settings;
        private Button _quit;

        public override void Initialise(UIManager uIManager)
        {
            base.Initialise(uIManager);
            _pauseScreen = _root.Q<VisualElement>("PauseScreen");

            _resume = _pauseScreen.Q<Button>("Resume");
            _resume.clicked += OnResumeClicked;

            _restart = _pauseScreen.Q<Button>("Restart");
            _restart.clicked += OnRestartClicked;

            _settings = _pauseScreen.Q<Button>("Settings");
            _settings.clicked += OnSettingsClicked;

            _quit = _pauseScreen.Q<Button>("Quit");
            _quit.clicked += OnQuitClicked;

            _pauseScreen.AddToClassList("hide");

            AudioCollection.Instance.SetupHoverAudio(_pauseScreen);
        }

        public override void Show()
        {
            base.Show();
            if (IsActive) return;

            AudioManager.Instance.CreateAudioBuilder()
                .WithVolume(0.8f)
                .Play(AudioCollection.Instance.PauseAudio);
            _pauseScreen.RemoveFromClassList("hide");
            IsActive = true;
        }

        public override void Hide()
        {
            base.Hide();
            if (!IsActive) return;

            _pauseScreen.AddToClassList("hide");
            IsActive = false;
        }

        private void OnResumeClicked()
        {
            AudioManager.Instance.CreateAudioBuilder()
                .WithVolume(0.8f)
                .Play(AudioCollection.Instance.StartAudio);
            GameManager.Instance.OnResumeEvent();
        }

        private void OnRestartClicked()
        {
            AudioManager.Instance.CreateAudioBuilder()
                .WithVolume(0.8f)
                .Play(AudioCollection.Instance.StartAudio);
            GameManager.Instance.RestartLevel();
        }

        private void OnSettingsClicked()
        {
            Debug.Log("Settings clicked");
            AudioCollection.Instance.PlaySelectAudio();
            _uiManager.OnUIStateChanged(UIState.Settings);
        }

        private void OnQuitClicked()
        {
            GameManager.Instance.InitialiseMenu();
        }
    }
}