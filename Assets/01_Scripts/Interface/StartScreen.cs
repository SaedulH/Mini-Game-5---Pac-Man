using AudioSystem;
using CoreSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace UserInterface
{
    [RequireComponent(typeof(UIDocument))]
    public class StartScreen : MonoBehaviour
    {
        private VisualElement _root;
        private Button _startButton;
        private Button _settingsButton;
        private Button _quitButton;

        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;

            _startButton = _root.Q<Button>("Start");
            _settingsButton = _root.Q<Button>("Settings");
            _quitButton = _root.Q<Button>("Quit");

            _startButton.clicked += OnStartClicked;
            _settingsButton.clicked += OnSettingsClicked;
            _quitButton.clicked += OnQuitClicked;
        }

        private void Start()
        {
            AudioCollection.Instance.SetupHoverAudio(_root);
        }

        private void OnDisable()
        {
            _startButton.clicked -= OnStartClicked;
            _settingsButton.clicked -= OnSettingsClicked;
            _quitButton.clicked -= OnQuitClicked;
        }

        private void OnStartClicked()
        {
            Debug.Log("Start clicked");
            AudioManager.Instance.CreateAudioBuilder()
                .WithVolume(0.8f)
                .Play(AudioCollection.Instance.StartAudio);
            // Load scene, start game, etc.
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
