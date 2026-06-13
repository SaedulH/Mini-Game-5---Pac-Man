using CoreSystem;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;

namespace SettingsSystem
{
    public class GameSettings : SettingsTab
    {
        private Button _pacmanMapToggle;
        private Button _msPacmanMapToggle;
        private Button _randomGenMapToggle;

        private Button _fixedCameraToggle;
        private Button _dynamicCameraToggle;
        private Button _followCameraToggle;

        private Button _screenShakeOffToggle;
        private Button _screenShakeLowToggle;
        private Button _screenShakeHighToggle;

        public void InitialiseSettings(VisualElement root)
        {
            TabElement = root.Q<Tab>("Game");

            _pacmanMapToggle = TabElement.Q<Button>("Pacman");
            _pacmanMapToggle.clicked += () => OnMapNameChanged(MapName.Pacman);

            _msPacmanMapToggle = TabElement.Q<Button>("MsPacman");
            _msPacmanMapToggle.clicked += () => OnMapNameChanged(MapName.MsPacman);

            _randomGenMapToggle = TabElement.Q<Button>("Random");
            _randomGenMapToggle.clicked += () => OnMapNameChanged(MapName.Random);

            _fixedCameraToggle = TabElement.Q<Button>("Fixed");
            _fixedCameraToggle.clicked += () => OnCameraModeChanged(CameraMode.Fixed);

            _dynamicCameraToggle = TabElement.Q<Button>("Dynamic");
            _dynamicCameraToggle.clicked += () => OnCameraModeChanged(CameraMode.Dynamic);

            _followCameraToggle = TabElement.Q<Button>("Follow");
            _followCameraToggle.clicked += () => OnCameraModeChanged(CameraMode.Follow);

            _screenShakeOffToggle = TabElement.Q<Button>("Off");
            _screenShakeOffToggle.clicked += () => OnScreenShakeChanged(ScreenShake.Off);

            _screenShakeLowToggle = TabElement.Q<Button>("Low");
            _screenShakeLowToggle.clicked += () => OnScreenShakeChanged(ScreenShake.Low);

            _screenShakeHighToggle = TabElement.Q<Button>("High");
            _screenShakeHighToggle.clicked += () => OnScreenShakeChanged(ScreenShake.High);

            GetSettings();
            TabElement.RemoveFromClassList("hide");
        }

        private void OnMapNameChanged(MapName setting, bool playSound = true)
        {
            AudioCollection.Instance.PlaySelectAudio(playSound);
            SetMapNameSetting(setting);

            ApplyMapNameUI(setting);
        }

        private void OnScreenShakeChanged(ScreenShake setting, bool playSound = true)
        {
            AudioCollection.Instance.PlaySelectAudio(playSound);
            SetScreenShakeSetting(setting);

            ApplyScreenShakeUI(setting);
        }

        private void OnCameraModeChanged(CameraMode setting, bool playSound = true)
        {
            AudioCollection.Instance.PlaySelectAudio(playSound);
            SetCameraModeSetting(setting);

            ApplyCameraUI(setting);
        }

        protected override void GetSettings()
        {
            var mapName = GetMapNameSetting() switch
            {
                "Pacman" => MapName.Pacman,
                "MsPacman" => MapName.MsPacman,
                "Random" => MapName.Random,
                _ => MapName.Pacman
            };

            var camera = GetCameraSetting() switch
            {
                "Fixed" => CameraMode.Fixed,
                "Dynamic" => CameraMode.Dynamic,
                "Follow" => CameraMode.Follow,
                _ => CameraMode.Fixed
            };

            var shake = GetScreenShakeSetting() switch
            {
                "Off" => ScreenShake.Off,
                "Low" => ScreenShake.Low,
                "High" => ScreenShake.High,
                _ => ScreenShake.Low
            };

            ApplyMapNameUI(mapName);
            ApplyCameraUI(camera);
            ApplyScreenShakeUI(shake);
        }

        private static void UpdateSelected(Button button, bool selected)
        {
            if (selected)
            {
                button.AddToClassList("selectedButton");
                button.RemoveFromClassList("selectionButton");
            }
            else
            {
                button.AddToClassList("selectionButton");
                button.RemoveFromClassList("selectedButton");
            }
        }

        private void ApplyMapNameUI(MapName name)
        {
            UpdateSelected(_pacmanMapToggle, name == MapName.Pacman);
            UpdateSelected(_msPacmanMapToggle, name == MapName.MsPacman);
            UpdateSelected(_randomGenMapToggle, name == MapName.Random);
        }

        private void ApplyCameraUI(CameraMode mode)
        {
            UpdateSelected(_fixedCameraToggle, mode == CameraMode.Fixed);
            UpdateSelected(_dynamicCameraToggle, mode == CameraMode.Dynamic);
            UpdateSelected(_followCameraToggle, mode == CameraMode.Follow);
        }

        private void ApplyScreenShakeUI(ScreenShake setting)
        {
            UpdateSelected(_screenShakeOffToggle, setting == ScreenShake.Off);
            UpdateSelected(_screenShakeLowToggle, setting == ScreenShake.Low);
            UpdateSelected(_screenShakeHighToggle, setting == ScreenShake.High);
        }

        private void SetMapNameSetting(MapName mapName)
        {
            PlayerPrefs.SetString("MapName", mapName.ToString());
        }

        private string GetMapNameSetting()
        {
            return PlayerPrefs.GetString("MapName", "Pacman");
        }

        private void SetCameraModeSetting(CameraMode cameraMode)
        {
            PlayerPrefs.SetString("Camera", cameraMode.ToString());
        }

        private string GetCameraSetting()
        {
            return PlayerPrefs.GetString("Camera", "Fixed");
        }

        private void SetScreenShakeSetting(ScreenShake screenShake)
        {
            PlayerPrefs.SetString("ScreenShake", screenShake.ToString());
        }

        private string GetScreenShakeSetting()
        {
            return PlayerPrefs.GetString("ScreenShake", "Low");
        }

        public override void ResetToDefaults()
        {
            PlayResetAudio();
            OnMapNameChanged(MapName.Pacman, false);
            OnCameraModeChanged(CameraMode.Fixed, false);
            OnScreenShakeChanged(ScreenShake.Low, false);
            GetSettings();
        }
    }
}