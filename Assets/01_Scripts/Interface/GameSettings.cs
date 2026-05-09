using CoreSystem;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;

namespace SettingsSystem
{
    public class GameSettings : SettingsTab
    {
        private Button _fixedCameraToggle;
        private Button _dynamicCameraToggle;
        private Button _followCameraToggle;

        private Button _screenShakeOffToggle;
        private Button _screenShakeLowToggle;
        private Button _screenShakeHighToggle;

        public override void InitialiseSettings(VisualElement root)
        {
            TabElement = root.Q<Tab>("Game");

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

            TabElement.RemoveFromClassList("hide");
        }

        private void OnScreenShakeChanged(ScreenShake setting, bool playSound = true)
        {
            AudioCollection.Instance.PlaySelectAudio(playSound);
            SetScreenShakeSetting(setting);
        }

        private void OnCameraModeChanged(CameraMode setting, bool playSound = true)
        {
            AudioCollection.Instance.PlaySelectAudio(playSound);
            SetCameraModeSetting(setting);
        }

        protected override void GetSettings()
        {
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

            ApplyCameraUI(camera);
            ApplyScreenShakeUI(shake);
        }

        private void ApplyCameraUI(CameraMode mode)
        {
            _fixedCameraToggle.SetEnabled(mode == CameraMode.Fixed);
            _dynamicCameraToggle.SetEnabled(mode == CameraMode.Dynamic);
            _followCameraToggle.SetEnabled(mode == CameraMode.Follow);
        }
        private void ApplyScreenShakeUI(ScreenShake setting)
        {
            _screenShakeOffToggle.SetEnabled(setting == ScreenShake.Off);
            _screenShakeLowToggle.SetEnabled(setting == ScreenShake.Low);
            _screenShakeHighToggle.SetEnabled(setting == ScreenShake.High);
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
            OnCameraModeChanged(CameraMode.Fixed, false);
            OnScreenShakeChanged(ScreenShake.Low, false);
            GetSettings();
        }
    }
}