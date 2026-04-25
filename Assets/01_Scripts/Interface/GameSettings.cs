using CoreSystem;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;

namespace SettingsSystem
{
    public class GameSettings : SettingsTab
    {
        private Toggle _fixedCameraToggle;
        private Toggle _dynamicCameraToggle;

        private Toggle _screenShakeOffToggle;
        private Toggle _screenShakeLowToggle;
        private Toggle _screenShakeHighToggle;

        public override void InitialiseSettings(VisualElement root)
        {
            SettingsScreen = root.Q<Button>("GameSettings");

            _fixedCameraToggle = SettingsScreen.Q<Toggle>("Fixed");
            _fixedCameraToggle.RegisterValueChangedCallback(e =>
            {
                if (!e.newValue) return;
                OnCameraModeChanged(CameraMode.Fixed);
            });

            _dynamicCameraToggle = SettingsScreen.Q<Toggle>("Dynamic");
            _dynamicCameraToggle.RegisterValueChangedCallback(e =>
            {
                if (!e.newValue) return;
                OnCameraModeChanged(CameraMode.Dynamic);
            });

            _screenShakeOffToggle = SettingsScreen.Q<Toggle>("Off");
            _screenShakeOffToggle.RegisterValueChangedCallback(e =>
            {
                if (!e.newValue) return;
                OnScreenShakeChanged(ScreenShake.Off);
            });

            _screenShakeLowToggle = SettingsScreen.Q<Toggle>("Low");
            _screenShakeLowToggle.RegisterValueChangedCallback(e =>
            {
                if (!e.newValue) return;
                OnScreenShakeChanged(ScreenShake.Low);
            });

            _screenShakeHighToggle = SettingsScreen.Q<Toggle>("High");
            _screenShakeHighToggle.RegisterValueChangedCallback(e =>
            {
                if (!e.newValue) return;
                OnScreenShakeChanged(ScreenShake.High);
            });

            SettingsScreen.RemoveFromClassList("hide");
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
                "Dynamic" => CameraMode.Dynamic,
                _ => CameraMode.Fixed
            };

            var shake = GetScreenShakeSetting() switch
            {
                "Off" => ScreenShake.Off,
                "High" => ScreenShake.High,
                _ => ScreenShake.Low
            };

            ApplyCameraUI(camera);
            ApplyScreenShakeUI(shake);
        }

        private void ApplyCameraUI(CameraMode mode)
        {
            _fixedCameraToggle.SetValueWithoutNotify(mode == CameraMode.Fixed);
            _dynamicCameraToggle.SetValueWithoutNotify(mode == CameraMode.Dynamic);
        }
        private void ApplyScreenShakeUI(ScreenShake setting)
        {
            _screenShakeOffToggle.SetValueWithoutNotify(setting == ScreenShake.Off);
            _screenShakeLowToggle.SetValueWithoutNotify(setting == ScreenShake.Low);
            _screenShakeHighToggle.SetValueWithoutNotify(setting == ScreenShake.High);
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
