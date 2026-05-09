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
        [field: SerializeField] public GameSettings GameSettings { get; private set; }
        [field: SerializeField] public AudioSettings AudioSettings { get; private set; }
        [field: SerializeField] public ControlSettings ControlSettings { get; private set; }

        [field: Space]
        [field: SerializeField] public SettingsTab CurrentSettingsTab { get; private set; }
        [field: SerializeField] public SettingsType CurrentScreen { get; private set; }
        [field: SerializeField] public SettingsType CachedScreen { get; private set; }

        [field: SerializeField] public float ScreenTransitionTime { get; private set; } = 0.1f;

        private VisualElement _settingsScreen;
        private TabView _settingsTabs;
        private Button _backButton;
        private Button _resetButton;

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

        private void SetCurrentScreen(SettingsType currentScreen)
        {
            CurrentScreen = currentScreen;
            CurrentSettingsTab = currentScreen switch
            {
                SettingsType.Game => GameSettings,
                SettingsType.Audio => AudioSettings,
                SettingsType.Controls => ControlSettings,
                _ => GameSettings
            };
        }

        private void OnActiveTabChanged(Tab previousTab, Tab newTab)
        {
            Debug.Log($"Settings Tab Changed From {previousTab.name} To {newTab.name}");
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

            ShowSettingsScreen();
            IsActive = true;
        }

        public override void Hide()
        {
            base.Hide();
            if (!IsActive) return;

            HideSettingsScreen();
            IsActive = false;
        }

        private void InitialiseSettings()
        {
            SetCurrentScreen(SettingsType.Game);

            GameSettings.InitialiseSettings(_settingsScreen);
            AudioSettings.InitialiseSettings(_settingsScreen);
            ControlSettings.InitialiseSettings(_settingsScreen);

            AudioCollection.Instance.SetupHoverAudio(_settingsScreen);
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
            SetCurrentScreen(SettingsType.Game);
            StartCoroutine(GameSettings.ShowSettingsTab(ScreenTransitionTime));
        }

        private void OnAudioClicked(bool playSound = true)
        {
            AudioCollection.Instance.PlaySelectAudio(playSound);
            Debug.Log($"Screen: Audio");
            SetCurrentScreen(SettingsType.Audio);
            StartCoroutine(AudioSettings.ShowSettingsTab(ScreenTransitionTime));
        }

        private void OnControlsClicked(bool playSound = true)
        {
            AudioCollection.Instance.PlaySelectAudio(playSound);
            Debug.Log($"Screen: Controls");
            SetCurrentScreen(SettingsType.Controls);
            StartCoroutine(ControlSettings.ShowSettingsTab(ScreenTransitionTime));
        }

        public override void OnBackClicked()
        {
            bool returnFromSettings = CurrentSettingsTab.OnBackClicked(CurrentScreen);
            if (returnFromSettings)
            {
                AudioCollection.Instance.PlayBackAudio();
                _uiManager.ReturnToPreviousUIState();
            }
        }

        private void OnResetClicked()
        {
            CurrentSettingsTab.ResetToDefaults();
        }

        #endregion
    }
}