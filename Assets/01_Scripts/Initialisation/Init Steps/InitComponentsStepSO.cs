using System.Threading.Tasks;
using AudioSystem;
using UnityEngine;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Init Components Step", menuName = "Levels/InitSteps/InitComponentsStep")]
    public class InitComponentsStepSO : LevelInitStepSO
    {
        public override async Task Run(LevelContext context)
        {

            //await AudioManager.Instance.ResetComponent();

            await Task.CompletedTask;
        }
    }
}

