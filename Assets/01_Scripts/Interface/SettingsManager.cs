using AudioSystem;
using CoreSystem;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;

namespace SettingsSystem
{
    [RequireComponent(typeof(GameSettings))]
    [RequireComponent(typeof(AudioSettings))]
    [RequireComponent(typeof(ControlSettings))]
    public class SettingsManager : MonoBehaviour
    {
        [field: SerializeField] public SettingsTab CurrentSettingsTab { get; private set; }
        [field: SerializeField] public TabView SettingsTab { get; private set; }
        [field: SerializeField] public GameSettings GameSettings { get; private set; }
        [field: SerializeField] public AudioSettings AudioSettings { get; private set; }
        [field: SerializeField] public ControlSettings ControlSettings { get; private set; }

        [field: SerializeField] public VisualElement Root { get; set; }
        [field: SerializeField] public VisualElement SettingsScreen { get; set; }
        [field: SerializeField] public Button GameButton { get; set; }
        [field: SerializeField] public Button AudioButton { get; set; }
        [field: SerializeField] public Button ControlsButton { get; set; }

        // Footer
        [field: SerializeField] public Button BackButton { get; set; }
        [field: SerializeField] public Button ResetButton { get; set; }
        [field: SerializeField] public Action HideSettingsAction { get; set; }
        [field: SerializeField] public SettingScreenType CurrentScreen { get; private set; }
        [field: SerializeField] public SettingScreenType CachedScreen { get; private set; }

        [field: SerializeField] public float ScreenTransitionTime { get; private set; } = 0.1f;

        private void Awake()
        {
            GameSettings = GetComponent<GameSettings>();
            AudioSettings = GetComponent<AudioSettings>();
            ControlSettings = GetComponent<ControlSettings>();
            CurrentSettingsTab = GameSettings;

            Root = GetComponent<UIDocument>().rootVisualElement;
            //PlayerInput = GetComponent<PlayerInput>();
        }

        private void Start()
        {
            InitialiseSettings();
        }

        private void OnEnable()
        {
            SettingsScreen = Root.Q<VisualElement>("Settings");
            SettingsTab = SettingsScreen.Q<TabView>("SettingsTab");

            GameButton = SettingsScreen.Q<Button>("Game");
            GameButton.clicked += () => OnGameClicked();

            AudioButton = SettingsScreen.Q<Button>("Audio");
            AudioButton.clicked += () => OnAudioClicked();

            ControlsButton = SettingsScreen.Q<Button>("Controls");
            ControlsButton.clicked += () => OnControlsClicked();

            // Footer
            BackButton = SettingsScreen.Q<Button>("Back");
            BackButton.clicked += OnBackClicked;

            ResetButton = SettingsScreen.Q<Button>("Reset");
            ResetButton.clicked += OnResetClicked;
        }

        private void InitialiseSettings()
        {
            CurrentScreen = SettingScreenType.Game;

            GameSettings.InitialiseSettings(SettingsScreen);
            AudioSettings.InitialiseSettings(SettingsScreen);
            ControlSettings.InitialiseSettings(SettingsScreen);

            AudioCollection.Instance.SetupHoverAudio(SettingsScreen);

            SettingsScreen.AddToClassList("hide");
        }

        private void HandleBackAction()
        {
            switch (CurrentScreen)
            {
                case SettingScreenType.Game:
                case SettingScreenType.Audio:
                case SettingScreenType.Controls:
                    OnBackClicked();
                    break;
                case SettingScreenType.InputPopup:
                    //HideInputPopup();
                    break;
            }
        }

        #region Screen Transitions

        private async void HideSettingsScreen()
        {
            CachedScreen = CurrentScreen;
            SettingsScreen.AddToClassList("hide");

            await Task.Delay((int)ScreenTransitionTime);
            SettingsScreen.style.display = DisplayStyle.None;
            HideSettingsAction?.Invoke();
        }

        private void OnGameClicked(bool playSound = true)
        {
            AudioCollection.Instance.PlaySelectAudio(playSound);
            //Debug.Log($"Screen: Game");
            GameButton.SetEnabled(false);
            AudioButton.SetEnabled(true);
            ControlsButton.SetEnabled(true);

            StartCoroutine(GameSettings.ShowSettingsTab(ScreenTransitionTime));
        }

        private void OnAudioClicked(bool playSound = true)
        {
            AudioCollection.Instance.PlaySelectAudio(playSound);
            //Debug.Log($"Screen: Audio");
            GameButton.SetEnabled(true);
            AudioButton.SetEnabled(false);
            ControlsButton.SetEnabled(true);

            StartCoroutine(AudioSettings.ShowSettingsTab(ScreenTransitionTime));
        }

        private void OnControlsClicked(bool playSound = true)
        {
            AudioCollection.Instance.PlaySelectAudio(playSound);
            //Debug.Log($"Screen: Controls");
            GameButton.SetEnabled(true);
            AudioButton.SetEnabled(true);
            ControlsButton.SetEnabled(false);

            StartCoroutine(ControlSettings.ShowSettingsTab(ScreenTransitionTime));
        }

        private void OnBackClicked()
        {
            AudioCollection.Instance.PlayBackAudio();
            HideSettingsScreen();
        }

        private void OnResetClicked()
        {
            CurrentSettingsTab.ResetToDefaults();
        }

        #endregion
    }
}