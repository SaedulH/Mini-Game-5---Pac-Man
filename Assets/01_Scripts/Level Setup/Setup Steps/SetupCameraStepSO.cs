using System;
using System.Threading.Tasks;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Setup Camera Step", menuName = "Levels/SetupSteps/SetupCameraStep")]
    public class SetupCameraStepSO : LevelSetupStepSO
    {
        public override async Task Run(LevelContext context)
        {
            string cameraMode = PlayerPrefs.GetString("Camera");
            await CameraZoom.Instance.SetupCameraMode(context, cameraMode);

            string screenShakeSetting = PlayerPrefs.GetString("ScreenShake");
            await CameraShake.Instance.SetupScreenShake(screenShakeSetting);
        }
    }
}

