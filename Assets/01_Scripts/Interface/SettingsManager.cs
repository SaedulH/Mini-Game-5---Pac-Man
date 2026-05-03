using CoreSystem;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UserInterface;
using Utilities;

namespace SettingsSystem
{
    [RequireComponent(typeof(GameSettings))]
    [RequireComponent(typeof(AudioSettings))]
    [RequireComponent(typeof(ControlSettings))]
    public class SettingsManager : UIScript
    {
        [field: SerializeField] public SettingsTab CurrentSettingsTab { get; private set; }
        [field: SerializeField] public GameSettings GameSettings { get; private set; }
        [field: SerializeField] public AudioSettings AudioSettings { get; private set; }
        [field: SerializeField] public ControlSettings ControlSettings { get; private set; }

        private VisualElement _settingsScreen;
        private TabView _settingsTabs;

        // Footer
        private Button _backButton;
        private Button _resetButton;
        [field: SerializeField] public SettingsType CurrentScreen { get; private set; }
        [field: SerializeField] public SettingsType CachedScreen { get; private set; }

        [field: SerializeField] public float ScreenTransitionTime { get; private set; } = 0.1f;

        protected override void Awake()
        {
            base.Awake();

            GameSettings = GetComponent<GameSettings>();
            AudioSettings = GetComponent<AudioSettings>();
            ControlSettings = GetComponent<ControlSettings>();
            CurrentSettingsTab = GameSettings;
        }

        private void OnEnable()
        {
            _settingsScreen = _root.Q<VisualElement>("SettingsScreen");

            _settingsTabs = _settingsScreen.Q<TabView>("SettingsTabs");
            _settingsTabs.activeTabChanged += OnActiveTabChanged;

            // Footer
            _backButton = _settingsScreen.Q<Button>("Back");
            _backButton.clicked += OnBackClicked;

            _resetButton = _settingsScreen.Q<Button>("Reset");
            _resetButton.clicked += OnResetClicked;

            _settingsScreen.AddToClassList("hide");
        }

        private void OnActiveTabChanged(Tab previousTab, Tab newTab)
        {
            Debug.Log($"Settings Tab Changed From {previousTab} To {newTab}");
            if (Enum.TryParse(newTab.name, out SettingsType settingsType))
            {
                switch (settingsType)
                {
                    case SettingsType.Game:
                    default:
                        OnGameClicked();
                        break;
                    case SettingsType.Audio:
                        OnAudioClicked();
                        break;
                    case SettingsType.Controls:
                        OnControlsClicked();
                        break;
                }
            }
            else
            {
                OnGameClicked();
            }
        }

        private void Start()
        {
            InitialiseSettings();
        }

        public override void Show()
        {
            base.Show();
            if (IsActive) return;

            _settingsScreen.RemoveFromClassList("hide");
            IsActive = true;
        }

        public override void Hide()
        {
            base.Hide();
            if (!IsActive) return;

            _settingsScreen.AddToClassList("hide");
            IsActive = false;
        }

        private void InitialiseSettings()
        {
            CurrentScreen = SettingsType.Game;

            GameSettings.InitialiseSettings(_settingsScreen);
            AudioSettings.InitialiseSettings(_settingsScreen);
            ControlSettings.InitialiseSettings(_settingsScreen);

            AudioCollection.Instance.SetupHoverAudio(_settingsScreen);
        }

        private void HandleBackAction()
        {
            switch (CurrentScreen)
            {
                case SettingsType.Game:
                case SettingsType.Audio:
                case SettingsType.Controls:
                    OnBackClicked();
                    break;
                case SettingsType.InputPopup:
                    //HideInputPopup();
                    break;
            }
        }

        #region Screen Transitions

        public async void ShowSettingsScreen()
        {
            _settingsTabs.selectedTabIndex = (int)CachedScreen;
            _settingsScreen.style.display = DisplayStyle.Flex;

            await Task.Delay((int)ScreenTransitionTime);

            _settingsScreen.RemoveFromClassList("hide");
        }

        private async void HideSettingsScreen()
        {
            CachedScreen = CurrentScreen;
            _settingsScreen.AddToClassList("hide");

            await Task.Delay((int)ScreenTransitionTime);
            _settingsScreen.style.display = DisplayStyle.None;
        }

        private void OnGameClicked(bool playSound = true)
        {
            AudioCollection.Instance.PlaySelectAudio(playSound);
            Debug.Log($"Screen: Game");
            CurrentScreen = SettingsType.Game;
            StartCoroutine(GameSettings.ShowSettingsTab(ScreenTransitionTime));
        }

        private void OnAudioClicked(bool playSound = true)
        {
            AudioCollection.Instance.PlaySelectAudio(playSound);
            Debug.Log($"Screen: Audio");
            CurrentScreen = SettingsType.Audio;
            StartCoroutine(AudioSettings.ShowSettingsTab(ScreenTransitionTime));
        }

        private void OnControlsClicked(bool playSound = true)
        {
            AudioCollection.Instance.PlaySelectAudio(playSound);
            Debug.Log($"Screen: Controls");
            CurrentScreen = SettingsType.Controls;
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