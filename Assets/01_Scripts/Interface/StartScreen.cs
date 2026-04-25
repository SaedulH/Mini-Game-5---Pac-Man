using AudioSystem;
using CoreSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace UserInterface
{
    [RequireComponent(typeof(UIDocument))]
    public class StartScreen : MonoBehaviour
    {
        private VisualElement _startScreen;

        private Button _start;
        private Button _settings;
        private Button _quit;

        private void Awake()
        {
            VisualElement _root = GetComponent<UIDocument>().rootVisualElement;
            _startScreen = _root.Q<VisualElement>("StartScreen");

            _start = _root.Q<Button>("Start");
            _start.clicked += OnStartClicked;

            _settings = _root.Q<Button>("Settings");
            _settings.clicked += OnSettingsClicked;

            _quit = _root.Q<Button>("Quit");
            _quit.clicked += OnQuitClicked;
        }

        private void Start()
        {
            AudioCollection.Instance.SetupHoverAudio(_startScreen);
        }

        private void OnDisable()
        {
            _start.clicked -= OnStartClicked;
            _settings.clicked -= OnSettingsClicked;
            _quit.clicked -= OnQuitClicked;
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
