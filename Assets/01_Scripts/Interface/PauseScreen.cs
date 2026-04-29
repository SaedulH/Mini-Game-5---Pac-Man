using AudioSystem;
using CoreSystem;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;

namespace UserInterface
{
    public class UIScript : MonoBehaviour
    {

    }

    [RequireComponent(typeof(UIDocument))]
    public class PauseScreen : UIScript
    {
        private VisualElement _root;
        private VisualElement _pauseScreen;

        private Button _resume;
        private Button _restart;
        private Button _settings;
        private Button _quit;

        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
        }

        private void OnEnable()
        {
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
        }

        private void Start()
        {
            AudioCollection.Instance.SetupHoverAudio(_pauseScreen);
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

        }

        private void OnQuitClicked()
        {
            GameManager.Instance.InitialiseMenu();
        }
    }
}