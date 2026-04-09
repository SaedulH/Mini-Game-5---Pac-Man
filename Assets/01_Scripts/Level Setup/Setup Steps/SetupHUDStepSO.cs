using System.Threading.Tasks;
using UnityEngine;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Setup HUD Step", menuName = "Levels/SetupSteps/SetupHUDStep")]
    public class SetupHUDStepSO : LevelSetupStepSO
    {

        public override async Task Run(LevelContext context)
        {
            string difficulty = PlayerPrefs.GetString("Difficulty");

            await HUDManager.Instance.SetupHUD(context);
        }

    }
}

