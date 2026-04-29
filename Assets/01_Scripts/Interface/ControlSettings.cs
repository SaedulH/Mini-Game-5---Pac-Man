using CoreSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Utilities;

namespace SettingsSystem
{
    public class ControlSettings : SettingsTab
    {
        [field: SerializeField] public PlayerInputActions PlayerInput { get; set; }
        [field: SerializeField] public InputMappingIcons InputMappingIcons { get; set; }
        [field: SerializeField] public Sprite DefaultIcon { get; set; }
        private InputActionMap _playerActions;
        private InputActionMap _menuActions;

        private InputActionRebindingExtensions.RebindingOperation _rebindOperation;

        private VisualElement _inputPopup;
        private Label _currentInputButton;
        private Button _upInput;
        private Button _downInput;
        private Button _leftInput;
        private Button _rightInput;

        public override void InitialiseSettings(VisualElement root)
        {
            if (PlayerInput == null)
            {
                PlayerInput = GameManager.Instance.InputActions;
            }
            _playerActions = PlayerInput.asset.FindActionMap(Constants.PACMAN_ACTION_MAP);
            _menuActions = PlayerInput.asset.FindActionMap(Constants.MENU_ACTION_MAP);

            TabElement = root.Q<Tab>("Controls");

            _upInput = TabElement.Q<Button>("UpInput");
            _upInput.clicked += () => OnPacmanInputChanged(ControlInput.Up);

            _downInput = TabElement.Q<Button>("DownInput");
            _downInput.clicked += () => OnPacmanInputChanged(ControlInput.Down);

            _leftInput = TabElement.Q<Button>("LeftInput");
            _leftInput.clicked += () => OnPacmanInputChanged(ControlInput.Left);

            _rightInput = TabElement.Q<Button>("RightInput");
            _rightInput.clicked += () => OnPacmanInputChanged(ControlInput.Right);

            _inputPopup = root.Q<VisualElement>("InputPopup");
            _currentInputButton = _inputPopup.Q<Label>("InputButtonLabel");

            TabElement.AddToClassList("hide");
            _inputPopup.AddToClassList("hide");

            LoadOverrideBindings();
        }

        protected override void GetSettings()
        {
            SetInputLabel(ControlInput.Up, _playerActions);
            SetInputLabel(ControlInput.Down, _playerActions);
            SetInputLabel(ControlInput.Left, _playerActions);
            SetInputLabel(ControlInput.Right, _playerActions);
        }

        private void OnPacmanInputChanged(ControlInput input, bool playSound = true)
        {
            AudioCollection.Instance.PlaySelectAudio(playSound);
            StartCoroutine(ShowInputPopup(input, _playerActions));
        }

        private void OnMenuInputChanged(bool playSound = true)
        {

        }

        private IEnumerator ShowInputPopup(ControlInput controlInput, InputActionMap actionMap)
        {
            _currentInputButton.text = controlInput.ToString();
            _inputPopup.style.display = DisplayStyle.Flex;

            yield return new WaitForEndOfFrame();
            _inputPopup.RemoveFromClassList("hide");
            yield return new WaitForSeconds(0.1f);

            BeginListeningForInput(controlInput, actionMap);
        }

        private IEnumerator HideInputPopup()
        {
            AudioCollection.Instance.PlayBackAudio();
            _inputPopup.AddToClassList("hide");

            yield return new WaitForSeconds(0.2f);

            _inputPopup.style.display = DisplayStyle.None;
        }

        private void BeginListeningForInput(ControlInput controlInput, InputActionMap actionMap)
        {
            if (actionMap == null) return;

            var (action, part) = GetAction(actionMap, controlInput);
            action.Disable();

            int bindingIndex = GetBindingIndex(action, part);
            if (bindingIndex == -1) return;

            Debug.Log($"Listening for input on action: {action.name}, part: {part}, binding index: {bindingIndex}");

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
                    SaveNewInput(controlInput, actionMap);
                    StartCoroutine(HideInputPopup());
                })
                .OnCancel(operation =>
                {
                    operation.Dispose();
                    _rebindOperation = null;

                    action.Enable();
                    StartCoroutine(HideInputPopup());
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
                PlayerInput.asset.LoadBindingOverridesFromJson(PlayerPrefs.GetString("rebinds"));
            }
        }

        private void SaveNewInput(ControlInput controlInput, InputActionMap actionMap)
        {
            SetInputLabel(controlInput, actionMap);
            PlayerPrefs.SetString("rebinds", PlayerInput.asset.SaveBindingOverridesAsJson());
            PlayerPrefs.Save();
        }

        private int GetBindingIndex(InputAction action, string part)
        {
            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (action.bindings[i].name == part ||
                    (part == null &&
                     !action.bindings[i].isComposite &&
                     !action.bindings[i].isPartOfComposite))
                {
                    return i;
                }
            }

            return -1;
        }

        private void SetInputLabel(ControlInput controlInput, InputActionMap actionMap)
        {
            if (actionMap == null) return;

            Button inputButton = GetControlInputButton(controlInput);
            if (inputButton == null) return;

            var (action, part) = GetAction(actionMap, controlInput);

            int bindingIndex = GetBindingIndex(action, part);

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
                    return _upInput;
                case ControlInput.Down:
                    return _downInput;
                case ControlInput.Left:
                    return _leftInput;
                case ControlInput.Right:
                    return _rightInput;
                default:
                    break;
            }
            return null;
        }

        public override void ResetToDefaults()
        {
            PlayResetAudio();

            PlayerInput.asset.RemoveAllBindingOverrides();

            PlayerPrefs.DeleteKey("rebinds");

            GetSettings();
        }
    }
}