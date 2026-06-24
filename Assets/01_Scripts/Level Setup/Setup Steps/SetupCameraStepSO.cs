using System.Threading.Tasks;
using UnityEngine;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Setup Camera Step", menuName = "Levels/SetupSteps/SetupCameraStep")]
    public class SetupCameraStepSO : LevelSetupStepSO
    {
        public override async Task Run(LevelContext context)
        {
            string cameraMode = PlayerPrefs.GetString("Camera", "Fixed");
            await CameraZoom.Instance.SetupCameraMode(cameraMode);

            string screenShakeSetting = PlayerPrefs.GetString("ScreenShake", "Low");
            await CameraShake.Instance.SetupScreenShake(screenShakeSetting);

            await Task.CompletedTask;
        }
    }
}