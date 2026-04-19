using System.Threading.Tasks;
using AudioSystem;
using UnityEngine;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Setup Components Step", menuName = "Levels/SetupSteps/SetupComponentsStep")]
    public class SetupComponentsStepSO : LevelSetupStepSO
    {
        public override async Task Run(LevelContext context)
        {

            //await AudioManager.Instance.ResetComponent();
            await MazeGenerator.Instance.ValidateNodes();

            await MazeGenerator.Instance.SetTotalPelletCount();

            await Task.CompletedTask;
        }
    }
}

