using System.Threading.Tasks;
using UnityEngine;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Setup Await Step", menuName = "Levels/SetupSteps/SetupAwaitStep")]
    public class SetupAwaitStepSO : LevelSetupStepSO
    {
        [SerializeField] private float WaitTime;

        public override async Task Run(LevelContext context)
        {          
            await Task.Delay((int)(WaitTime * 1000));
        }
    }
}

