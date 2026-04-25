using AudioSystem;
using CoreSystem;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;

namespace UserInterface
{
    [RequireComponent(typeof(UIDocument))]
    public class PauseScreen : MonoBehaviour
    {
        private VisualElement _pauseScreen;
        private Button _resume;
        private Button _settings;
        private Button _quit;

        private void Awake()
        {
            VisualElement _root = GetComponent<UIDocument>().rootVisualElement;
            _pauseScreen = _root.Q<VisualElement>("PauseScreen");

            _resume = _pauseScreen.Q<Button>("Resume");
            _resume.clicked += OnResumeClicked;

            _settings = _pauseScreen.Q<Button>("Settings");
            _settings.clicked += OnSettingsClicked;

            _quit = _pauseScreen.Q<Button>("Quit");
            _quit.clicked += OnQuitClicked;
        }

        private void Start()
        {
            AudioCollection.Instance.SetupHoverAudio(_pauseScreen);
        }

        private void OnDisable()
        {
            _resume.clicked -= OnResumeClicked;
            _settings.clicked -= OnSettingsClicked;
            _quit.clicked -= OnQuitClicked;
        }


        public void OnGameStateUpdated(GameState gameState)
        {
            bool enabled = gameState.Equals(GameState.Paused);
            ShowPauseMenu(enabled);
        }

        public void ShowPauseMenu(bool enabled)
        {
            if (enabled)
            {
                _pauseScreen.RemoveFromClassList("hide");
            }
            else
            {
                _pauseScreen.AddToClassList("hide");
            }
        }
        private void OnResumeClicked()
        {
            AudioManager.Instance.CreateAudioBuilder()
                .WithVolume(0.8f)
                .Play(AudioCollection.Instance.StartAudio);
            GameManager.Instance.OnPauseEvent();
        }

        private void OnSettingsClicked()
        {
            Debug.Log("Settings clicked");
            AudioCollection.Instance.PlaySelectAudio();
        }

        private void OnQuitClicked()
        {
            GameManager.Instance.InitialiseMenu();
        }
    }
}