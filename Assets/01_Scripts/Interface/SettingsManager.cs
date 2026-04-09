using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AudioSystem;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Utilities;

namespace CoreSystem
{
    public class SettingsManager : MonoBehaviour
    {
        [field: SerializeField] public VisualElement Root { get; set; }
        [field: SerializeField] public VisualElement SettingsScreen { get; set; }
        [field: SerializeField] public Button GameButton { get; set; }
        [field: SerializeField] public Button AudioButton { get; set; }
        [field: SerializeField] public Button ControlsButton { get; set; }

        // Game Settings
        [field: SerializeField] public VisualElement GameSettings { get; set; }
        [field: SerializeField] public Button FixedButton { get; set; }
        [field: SerializeField] public Button DynamicButton { get; set; }
        [field: SerializeField] public Button OffButton { get; set; }
        [field: SerializeField] public Button LowButton { get; set; }
        [field: SerializeField] public Button HighButton { get; set; }

        // Audio Settings
        [field: SerializeField] public VisualElement AudioSettings { get; set; }
        [field: SerializeField] public SliderInt MasterVolumeSlider { get; set; }
        [field: SerializeField] public SliderInt MusicVolumeSlider { get; set; }
        [field: SerializeField] public SliderInt UIVolumeSlider { get; set; }
        [field: SerializeField] public SliderInt EffectsVolumeSlider { get; set; }
        [field: SerializeField] public List<SliderInt> VolumeSliders { get; set; }
        [field: SerializeField] public AudioMixer AudioMixer { get; set; }

        // Control Settings
        [field: SerializeField] public VisualElement ControlsSettings { get; set; }
        [field: SerializeField] public VisualElement InputPopup { get; set; }
        [field: SerializeField] public Label InputPlayerLabel { get; set; }
        [field: SerializeField] public Label InputButtonLabel { get; set; }
        [field: SerializeField] public InputMappingIcons InputMappingIcons { get; set; }
        [field: SerializeField] public PlayerInput PlayerInput { get; set; }
        [field: SerializeField] public Sprite DefaultIcon { get; set; }
        private InputActionRebindingExtensions.RebindingOperation _rebindOperation;

        // Player One
        [field: SerializeField] public Button UpInput { get; set; }
        [field: SerializeField] public Button DownInput { get; set; }
        [field: SerializeField] public Button LeftInput { get; set; }
        [field: SerializeField] public Button RightInput { get; set; }

        // Footer
        [field: SerializeField] public Button BackButton { get; set; }
        [field: SerializeField] public Button ResetButton { get; set; }
        [field: SerializeField] public Action HideSettingsAction { get; set; }
        [field: SerializeField] public SettingScreenType CurrentScreen { get; private set; }
        [field: SerializeField] public SettingScreenType CachedScreen { get; private set; }

        [field: SerializeField] public float ScreenTransitionTime { get; private set; } = 0.1f;
        [field: SerializeField] public float SliderLerpSpeed { get; private set; } = 50f;

        private void Awake()
        {
            Root = GetComponent<UIDocument>().rootVisualElement;
            PlayerInput = GetComponent<PlayerInput>();
        }

        private void Start()
        {
            InitialiseSettings();
        }

        private void OnEnable()
        {
            SettingsScreen = Root.Q<VisualElement>("Settings");

            //Settings Screen
            GameSettings = SettingsScreen.Q<VisualElement>("GameSettings");
            AudioSettings = SettingsScreen.Q<VisualElement>("AudioSettings");
            ControlsSettings = SettingsScreen.Q<VisualElement>("ControlSettings");

            GameButton = SettingsScreen.Q<Button>("Game");
            GameButton.clicked += () => OnGameClicked();

            AudioButton = SettingsScreen.Q<Button>("Audio");
            AudioButton.clicked += () => OnAudioClicked();

            ControlsButton = SettingsScreen.Q<Button>("Controls");
            ControlsButton.clicked += () => OnControlsClicked();        

            //Game Settings
            FixedButton = GameSettings.Q<Button>("Fixed");
            FixedButton.clicked += () => OnFixedClicked();

            DynamicButton = GameSettings.Q<Button>("Dynamic");
            DynamicButton.clicked += () => OnDynamicClicked();

            OffButton = GameSettings.Q<Button>("Off");
            OffButton.clicked += () => OnOffClicked();

            LowButton = GameSettings.Q<Button>("Low");
            LowButton.clicked += () => OnLowClicked();

            HighButton = GameSettings.Q<Button>("High");
            HighButton.clicked += () => OnHighClicked();

            //Audio Settings
            MasterVolumeSlider = AudioSettings.Q<SliderInt>("MasterVolume");
            MasterVolumeSlider.RegisterValueChangedCallback((e) => OnMasterVolumeChanged(e.newValue));

            MusicVolumeSlider = AudioSettings.Q<SliderInt>("MusicVolume");
            MusicVolumeSlider.RegisterValueChangedCallback((e) => OnMusicVolumeChanged(e.newValue));

            UIVolumeSlider = AudioSettings.Q<SliderInt>("UIVolume");
            UIVolumeSlider.RegisterValueChangedCallback((e) => OnUIVolumeChanged(e.newValue));

            EffectsVolumeSlider = AudioSettings.Q<SliderInt>("EffectsVolume");
            EffectsVolumeSlider.RegisterValueChangedCallback((e) => OnEffectsVolumeChanged(e.newValue));

            VolumeSliders = AudioSettings.Query<SliderInt>().ToList();
            SetupSliders(VolumeSliders);

            //Control Settings
            UpInput = ControlsSettings.Q<Button>("UpInput");
            UpInput.clicked += () => OnUpInputChanged();

            DownInput = ControlsSettings.Q<Button>("DownInput");
            DownInput.clicked += () => OnDownInputChanged();

            LeftInput = ControlsSettings.Q<Button>("LeftInput");
            LeftInput.clicked += () => OnLeftInputChanged();

            RightInput = ControlsSettings.Q<Button>("RightInput");
            RightInput.clicked += () => OnRightInputChanged();

            InputPopup = SettingsScreen.Q<VisualElement>("InputPopup");
            InputPlayerLabel = InputPopup.Q<Label>("InputPlayerLabel");
            InputButtonLabel = InputPopup.Q<Label>("InputButtonLabel");

            // Footer
            BackButton = SettingsScreen.Q<Button>("Back");
            BackButton.clicked += OnBackClicked;

            ResetButton = SettingsScreen.Q<Button>("Reset");
            ResetButton.clicked += OnResetClicked;
        }

        private void OnDisable()
        {
            GameButton.clicked -= () => OnGameClicked();
            AudioButton.clicked -= () => OnAudioClicked();
            ControlsButton.clicked -= () => OnControlsClicked();
            FixedButton.clicked -= () => OnFixedClicked();
            DynamicButton.clicked -= () => OnDynamicClicked();
            OffButton.clicked -= () => OnOffClicked();
            LowButton.clicked -= () => OnLowClicked();
            HighButton.clicked -= () => OnHighClicked();

            MasterVolumeSlider.UnregisterValueChangedCallback((e) => OnMasterVolumeChanged(e.newValue));
            MusicVolumeSlider.UnregisterValueChangedCallback((e) => OnMusicVolumeChanged(e.newValue));
            UIVolumeSlider.UnregisterValueChangedCallback((e) => OnUIVolumeChanged(e.newValue));
            EffectsVolumeSlider.UnregisterValueChangedCallback((e) => OnEffectsVolumeChanged(e.newValue));

            UpInput.clicked -= () => OnUpInputChanged();
            DownInput.clicked -= () => OnDownInputChanged();
            LeftInput.clicked -= () => OnLeftInputChanged();
            RightInput.clicked -= () => OnRightInputChanged();

            BackButton.clicked -= OnBackClicked;
            ResetButton.clicked -= OnResetClicked;

        }

        private void InitialiseSettings()
        {
            CurrentScreen = SettingScreenType.Game;

            SettingsScreen.AddToClassList("hide");
            GameSettings.RemoveFromClassList("hide");
            AudioSettings.AddToClassList("hide");
            ControlsSettings.AddToClassList("hide");
            InputPopup.AddToClassList("hide");

            LoadOverrideBindings();

            SetupHoverAudio();
            SetupAudioMixer();
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
                    HideInputPopup();
                    break;
            }
        }

        #region Audio 

        private void SetupAudioMixer()
        {
            SetMasterVolumeSetting(GetMasterVolumeSetting());
            SetMusicVolumeSetting(GetMusicVolumeSetting());
            SetUIVolumeSetting(GetUIVolumeSetting());
            SetEffectsVolumeSetting(GetEffectsVolumeSetting());
        }

        private void SetupHoverAudio()
        {
            Root.Query<Button>().ForEach(button =>
            {
                bool hovered = false;
                button.RegisterCallback<PointerEnterEvent>(_ =>
                {
                    if (hovered || !button.enabledSelf)
                        return;

                    hovered = true;

                    PlayHoverAudio();
                });

                button.RegisterCallback<PointerLeaveEvent>(_ =>
                {
                    hovered = false;
                });
            });
        }

        private void PlayBackAudio(bool playSound = true)
        {
            if (!playSound) return;
            AudioManager.Instance.CreateAudioBuilder()
                .Play(AudioCollection.Instance.BackAudio);
        }

        private void PlaySelectAudio(bool playSound = true)
        {
            if (!playSound) return;
            AudioManager.Instance.CreateAudioBuilder()
                .Play(AudioCollection.Instance.SelectAudio);
        }

        private void PlayHoverAudio(bool playSound = true)
        {
            if (!playSound) return;
            AudioManager.Instance.CreateAudioBuilder()
                .Play(AudioCollection.Instance.HoverAudio);
        }

        private void PlayResetAudio(bool playSound = true)
        {
            if (!playSound) return;
            AudioManager.Instance.CreateAudioBuilder()
                .Play(AudioCollection.Instance.ResetAudio);
        }

        #endregion

        #region Screen Transitions

        private IEnumerator ShowGameSettingsScreen()
        {
            CurrentScreen = SettingScreenType.Game;
            GetPlayerGameSettings();

            AudioSettings.AddToClassList("hide");
            ControlsSettings.AddToClassList("hide");
            yield return new WaitForSeconds(ScreenTransitionTime);
            GameSettings.SetEnabled(true);
            GameSettings.RemoveFromClassList("hide");
        }

        private IEnumerator ShowAudioSettingsScreen()
        {
            CurrentScreen = SettingScreenType.Audio;
            GetPlayerAudioSettings();

            GameSettings.AddToClassList("hide");
            ControlsSettings.AddToClassList("hide");
            yield return new WaitForSeconds(ScreenTransitionTime);
            AudioSettings.SetEnabled(true);
            AudioSettings.RemoveFromClassList("hide");
        }

        private IEnumerator ShowControlsSettingsScreen()
        {
            CurrentScreen = SettingScreenType.Controls;
            GetPlayerControlsSettings();

            AudioSettings.AddToClassList("hide");
            GameSettings.AddToClassList("hide");
            yield return new WaitForSeconds(ScreenTransitionTime);
            ControlsSettings.SetEnabled(true);
            ControlsSettings.RemoveFromClassList("hide");
        }

        private async void ShowSettingsScreen()
        {
            SettingsScreen.style.display = DisplayStyle.Flex;
            await Task.Yield();
            SettingsScreen.RemoveFromClassList("hide");

            switch (CachedScreen)
            {
                case SettingScreenType.Game:
                    OnGameClicked(false);
                    break;
                case SettingScreenType.Audio:
                    OnAudioClicked(false);
                    break;
                case SettingScreenType.Controls:
                    OnControlsClicked(false);
                    break;
            }

            await Task.Delay(200);
        }

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
            PlaySelectAudio(playSound);
            //Debug.Log($"Screen: Game");
            GameButton.SetEnabled(false);
            AudioButton.SetEnabled(true);
            ControlsButton.SetEnabled(true);

            StartCoroutine(ShowGameSettingsScreen());
        }

        private void OnAudioClicked(bool playSound = true)
        {
            PlaySelectAudio(playSound);
            //Debug.Log($"Screen: Audio");
            GameButton.SetEnabled(true);
            AudioButton.SetEnabled(false);
            ControlsButton.SetEnabled(true);

            StartCoroutine(ShowAudioSettingsScreen());
        }

        private void OnControlsClicked(bool playSound = true)
        {
            PlaySelectAudio(playSound);
            //Debug.Log($"Screen: Controls");
            GameButton.SetEnabled(true);
            AudioButton.SetEnabled(true);
            ControlsButton.SetEnabled(false);

            StartCoroutine(ShowControlsSettingsScreen());
        }

        private void OnBackClicked()
        {
            PlayBackAudio();
            HideSettingsScreen();
        }

        private void OnResetClicked()
        {
            switch (CurrentScreen)
            {
                case SettingScreenType.Game:
                    ResetGameSettingsToDefault();
                    break;
                case SettingScreenType.Audio:
                    ResetAudioSettingsToDefault();
                    break;
                case SettingScreenType.Controls:
                    ResetControlsSettingsToDefault();
                    break;
            }
        }

        private void ResetGameSettingsToDefault()
        {
            PlayResetAudio();
            OnFixedClicked(false);
            OnLowClicked(false);
        }

        private void ResetAudioSettingsToDefault()
        {
            PlayResetAudio();

            OnMasterVolumeChanged(50);
            OnMusicVolumeChanged(50);
            OnUIVolumeChanged(50);
            OnEffectsVolumeChanged(50);

            GetPlayerAudioSettings();
        }

        private void ResetControlsSettingsToDefault()
        {
            PlayResetAudio();

            PlayerInput.actions.RemoveAllBindingOverrides();

            PlayerPrefs.DeleteKey("rebinds");

            GetPlayerControlsSettings();
        }

        #endregion

        #region Game Settings

        private void GetPlayerGameSettings()
        {
            switch (GetCameraSetting())
            {
                case "Fixed":
                default:
                    OnFixedClicked(false);
                    break;
                case "Dynamic":
                    OnDynamicClicked(false);
                    break;

            }
            switch (GetScreenShakeSetting())
            {
                case "Off":
                    OnOffClicked(false);
                    break;
                case "Low":
                default:
                    OnLowClicked(false);
                    break;
                case "High":
                    OnHighClicked(false);
                    break;
            }
        }

        private void OnFixedClicked(bool playSound = true)
        {
            PlaySelectAudio(playSound);
            FixedButton.SetEnabled(false);
            DynamicButton.SetEnabled(true);
            SetCameraSetting("Fixed");
        }

        private void OnDynamicClicked(bool playSound = true)
        {
            PlaySelectAudio(playSound);
            FixedButton.SetEnabled(true);
            DynamicButton.SetEnabled(false);
            SetCameraSetting("Dynamic");
        }

        private void OnOffClicked(bool playSound = true)
        {
            PlaySelectAudio(playSound);
            OffButton.SetEnabled(false);
            LowButton.SetEnabled(true);
            HighButton.SetEnabled(true);
            SetScreenShakeSetting("Off");
        }

        private void OnLowClicked(bool playSound = true)
        {
            PlaySelectAudio(playSound);
            OffButton.SetEnabled(true);
            LowButton.SetEnabled(false);
            HighButton.SetEnabled(true);
            SetScreenShakeSetting("Low");
        }

        private void OnHighClicked(bool playSound = true)
        {
            PlaySelectAudio(playSound);
            OffButton.SetEnabled(true);
            LowButton.SetEnabled(true);
            HighButton.SetEnabled(false);
            SetScreenShakeSetting("High");
        }

        private void SetDifficultySetting(string difficultySetting)
        {
            PlayerPrefs.SetString("Difficulty", difficultySetting);
        }

        private string GetDifficultySetting()
        {
            return PlayerPrefs.GetString("Difficulty", "Easy");
        }

        private void SetCameraSetting(string cameraSetting)
        {
            PlayerPrefs.SetString("Camera", cameraSetting);
        }

        private string GetCameraSetting()
        {
            return PlayerPrefs.GetString("Camera", "Fixed");
        }

        private void SetScreenShakeSetting(string screenShakeSetting)
        {
            PlayerPrefs.SetString("ScreenShake", screenShakeSetting);
        }

        private string GetScreenShakeSetting()
        {
            return PlayerPrefs.GetString("ScreenShake", "Low");
        }

        #endregion

        #region Audio Settings

        private void GetPlayerAudioSettings()
        {
            StartCoroutine(SetSliderValue(VolumeSliders[0], GetMasterVolumeSetting()));
            StartCoroutine(SetSliderValue(VolumeSliders[1], GetMusicVolumeSetting()));
            StartCoroutine(SetSliderValue(VolumeSliders[2], GetUIVolumeSetting()));
            StartCoroutine(SetSliderValue(VolumeSliders[3], GetEffectsVolumeSetting()));
        }

        private void SetupSliders(List<SliderInt> sliders)
        {
            List<SliderInt> orderedSliders = new(new SliderInt[4]);
            foreach (SliderInt slider in sliders)
            {
                slider.AddToClassList("audioSlider");
                slider.SetEnabled(true);
                slider.style.opacity = 1f;
                if (slider.name.Equals("MasterVolume"))
                {
                    orderedSliders[0] = slider;
                }
                if (slider.name.Equals("MusicVolume"))
                {
                    orderedSliders[1] = slider;
                }
                if (slider.name.Equals("UIVolume"))
                {
                    orderedSliders[2] = slider;
                }
                if (slider.name.Equals("EffectsVolume"))
                {
                    orderedSliders[3] = slider;
                }
            }

            VolumeSliders = orderedSliders;
        }

        private void OnMasterVolumeChanged(int newValue)
        {
            SetMasterVolumeSetting(newValue);
        }

        private void OnMusicVolumeChanged(int newValue)
        {
            SetMusicVolumeSetting(newValue);
        }

        private void OnUIVolumeChanged(int newValue)
        {
            SetUIVolumeSetting(newValue);
        }

        private void OnEffectsVolumeChanged(int newValue)
        {
            SetEffectsVolumeSetting(newValue);
        }

        private void SetMasterVolumeSetting(int masterVolume)
        {
            PlayerPrefs.SetInt(Constants.MASTER_AUDIO_MIXER, masterVolume);
            SetMixerVolume(Constants.MASTER_AUDIO_MIXER, masterVolume);
        }

        private int GetMasterVolumeSetting()
        {
            return PlayerPrefs.GetInt(Constants.MASTER_AUDIO_MIXER, 50);
        }

        private void SetMusicVolumeSetting(int musicVolume)
        {
            PlayerPrefs.SetInt(Constants.MUSIC_AUDIO_MIXER, musicVolume);
            SetMixerVolume(Constants.MUSIC_AUDIO_MIXER, musicVolume);
        }

        private int GetMusicVolumeSetting()
        {
            return PlayerPrefs.GetInt(Constants.MUSIC_AUDIO_MIXER, 50);
        }

        private void SetUIVolumeSetting(int uiVolume)
        {
            PlayerPrefs.SetInt(Constants.UI_AUDIO_MIXER, uiVolume);
            SetMixerVolume(Constants.UI_AUDIO_MIXER, uiVolume);
        }

        private int GetUIVolumeSetting()
        {
            return PlayerPrefs.GetInt(Constants.UI_AUDIO_MIXER, 50);
        }

        private void SetEffectsVolumeSetting(int effectsVolume)
        {
            PlayerPrefs.SetInt(Constants.EFFECTS_AUDIO_MIXER, effectsVolume);
            SetMixerVolume(Constants.EFFECTS_AUDIO_MIXER, effectsVolume);
        }

        private int GetEffectsVolumeSetting()
        {
            return PlayerPrefs.GetInt(Constants.EFFECTS_AUDIO_MIXER, 50);
        }

        private void SetMixerVolume(string mixerGroup, int channelVolume)
        {
            if (AudioMixer == null) return;

            float channel = channelVolume / 100f;

            if (channel <= 0.0001f)
            {
                AudioMixer.SetFloat(mixerGroup, -80f);
                return;
            }

            float db = Mathf.Log10(channel) * 20f;
            AudioMixer.SetFloat(mixerGroup, db);
        }

        private IEnumerator SetSliderValue(SliderInt slider, int value)
        {
            float current = slider.value;

            while (!Mathf.Approximately(current, value))
            {
                current = Mathf.Lerp(current, value, Time.deltaTime * SliderLerpSpeed);
                slider.SetValueWithoutNotify(Mathf.RoundToInt(current));
                yield return null;
            }

            slider.SetValueWithoutNotify(value);
        }

        #endregion

        #region Controls Settings

        private void GetPlayerControlsSettings()
        {
            SetInputLabel(ControlInput.Up);
            SetInputLabel(ControlInput.Down);
            SetInputLabel(ControlInput.Left);
            SetInputLabel(ControlInput.Right);
        }

        private void OnUpInputChanged(bool playSound = true)
        {
            ShowInputPopup(ControlInput.Up);
        }

        private void OnDownInputChanged(bool playSound = true)
        {
            ShowInputPopup(ControlInput.Down);
        }

        private void OnLeftInputChanged(bool playSound = true)
        {
            ShowInputPopup(ControlInput.Left);
        }

        private void OnRightInputChanged(bool playSound = true)
        {
            ShowInputPopup(ControlInput.Right);
        }

        private async void ShowInputPopup(ControlInput controlInput)
        {
            CurrentScreen = SettingScreenType.InputPopup;
            InputButtonLabel.text = controlInput.ToString();
            InputPopup.style.display = DisplayStyle.Flex;

            await Task.Yield();
            InputPopup.RemoveFromClassList("hide");
            await Task.Delay(100);
            BeginListeningForInput(controlInput);
        }

        private async void HideInputPopup()
        {
            PlayBackAudio();
            InputPopup.AddToClassList("hide");
            await Task.Delay(200);
            CurrentScreen = SettingScreenType.Controls;
            InputPopup.style.display = DisplayStyle.None;
        }

        private void BeginListeningForInput(ControlInput controlInput)
        {
            InputActionMap actionMap = PlayerInput.actions.FindActionMap("Player");
            if (actionMap == null) return;

            var actionResult = GetAction(actionMap, controlInput);
            InputAction action = actionResult.action;
            action.Disable();

            int bindingIndex = -1;

            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (action.bindings[i].name == actionResult.part ||
                    (actionResult.part == null && !action.bindings[i].isComposite && !action.bindings[i].isPartOfComposite))
                {
                    bindingIndex = i;
                    break;
                }
            }
            if (bindingIndex == -1) return;
            Debug.Log($"Listening for input on action: {action.name}, part: {actionResult.part}, binding index: {bindingIndex}");

            _rebindOperation = action
                .PerformInteractiveRebinding(bindingIndex)
                .WithCancelingThrough("<Keyboard>/escape")
                // Mouse
                .WithControlsExcluding("Mouse/position")
                .WithControlsExcluding("Mouse/position")
                .WithControlsExcluding("Mouse/delta")
                .WithControlsExcluding("Mouse/scroll")

                // Pen
                .WithControlsExcluding("Pen/position")
                .WithControlsExcluding("Pen/delta")
                .WithControlsExcluding("Pen/scroll")

                // Touch
                .WithControlsExcluding("Touch/position")
                .WithControlsExcluding("Touch/delta")
                .WithControlsExcluding("Touch/scroll")

                // Function keys
                .WithControlsExcluding("<Keyboard>/f1")
                .WithControlsExcluding("<Keyboard>/f2")
                .WithControlsExcluding("<Keyboard>/f3")
                .WithControlsExcluding("<Keyboard>/f4")
                .WithControlsExcluding("<Keyboard>/f5")
                .WithControlsExcluding("<Keyboard>/f6")
                .WithControlsExcluding("<Keyboard>/f7")
                .WithControlsExcluding("<Keyboard>/f8")
                .WithControlsExcluding("<Keyboard>/f9")
                .WithControlsExcluding("<Keyboard>/f10")
                .WithControlsExcluding("<Keyboard>/f11")
                .WithControlsExcluding("<Keyboard>/f12")

                // Special keys
                .WithControlsExcluding("<Keyboard>/leftMeta")
                .WithControlsExcluding("<Keyboard>/numLock")
                .WithControlsExcluding("<Keyboard>/scrollLock")
                .WithControlsExcluding("<Keyboard>/home")
                .WithControlsExcluding("<Keyboard>/pageUp")
                .WithControlsExcluding("<Keyboard>/pageDown")
                .WithControlsExcluding("<keyboard>/anyKey")
                .OnComplete(operation =>
                {
                    operation.Dispose();
                    _rebindOperation = null;

                    action.Enable();
                    SaveNewInput(controlInput);
                    HideInputPopup();
                })
                .OnCancel(operation =>
                {
                    operation.Dispose();
                    _rebindOperation = null;

                    action.Enable();
                    HideInputPopup();
                });

            _rebindOperation.Start();
        }

        private (InputAction action, string part) GetAction(InputActionMap map, ControlInput controlInput)
        {
            switch (controlInput)
            {
                case ControlInput.Up:
                    return (map.FindAction("Vertical"), "positive");
                case ControlInput.Down:
                    return (map.FindAction("Vertical"), "negative");
                case ControlInput.Left:
                    return (map.FindAction("Horizontal"), "negative");
                case ControlInput.Right:
                    return (map.FindAction("Horizontal"), "positive");
                default:
                    break;
            }

            return (null, null);
        }

        private void LoadOverrideBindings()
        {
            if (PlayerPrefs.HasKey("rebinds"))
            {
                PlayerInput.actions.LoadBindingOverridesFromJson(PlayerPrefs.GetString("rebinds"));
            }
        }

        private void SaveNewInput(ControlInput controlInput)
        {
            SetInputLabel(controlInput);
            PlayerPrefs.SetString("rebinds", PlayerInput.actions.SaveBindingOverridesAsJson());
            PlayerPrefs.Save();
        }

        private void SetInputLabel(ControlInput controlInput)
        {
            Button inputButton = GetControlInputButton(controlInput);
            if (inputButton == null) return;

            InputActionMap map = PlayerInput.actions.FindActionMap("Player");

            var actionResult = GetAction(map, controlInput);
            InputAction action = actionResult.action;

            int bindingIndex = -1;

            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (action.bindings[i].name == actionResult.part ||
                   (actionResult.part == null && !action.bindings[i].isComposite && !action.bindings[i].isPartOfComposite))
                {
                    bindingIndex = i;
                    break;
                }
            }

            if (bindingIndex == -1) return;

            string path = action.bindings[bindingIndex].effectivePath;

            string readable = InputControlPath.ToHumanReadableString(
                path,
                InputControlPath.HumanReadableStringOptions.OmitDevice
            );
            //Debug.Log($"Setting input label for player {playerIndex} control {controlInput} with path: {path}, readable: {readable}");
            InputKeyIconMap inputMap = InputMappingIcons.GetInputMapForInputKey(readable);
            Sprite displayIcon = inputMap != null && inputMap.InputIcon != null ? inputMap.InputIcon : null;
            string displayText = inputMap != null && inputMap.InputString.Length > 0 ? inputMap.InputString : readable;

            SetInputKeyDisplayValue(inputButton, displayIcon, displayText);
        }

        private void SetInputKeyDisplayValue(Button inputButton, Sprite icon, string value)
        {
            //string addToClass = icon != null ? "controlInputIcon" : "controlInput";
            //string removeFromClass = icon != null ? "controlInput" : "controlInputIcon";
            inputButton.text = icon != null ? "" : value;
            inputButton.style.backgroundImage = new StyleBackground(icon != null ? icon : DefaultIcon);
            //inputButton.AddToClassList(addToClass);
            //inputButton.RemoveFromClassList(removeFromClass);
        }

        private Button GetControlInputButton(ControlInput controlInput)
        {
            switch (controlInput)
            {
                case ControlInput.Up:
                    return UpInput;
                case ControlInput.Down:
                    return DownInput;
                case ControlInput.Left:
                    return LeftInput;
                case ControlInput.Right:
                    return RightInput;
                default:
                    break;
            }
            return null;
        }
    }

    #endregion
}