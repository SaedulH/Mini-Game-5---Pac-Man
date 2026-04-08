using System.Threading.Tasks;
using UnityEngine;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Init Await Step", menuName = "Levels/InitSteps/InitAwaitStep")]
    public class InitAwaitStepSO : LevelInitStepSO
    {
        [SerializeField] private float WaitTime;

        public override async Task Run(LevelContext context)
        {          
            await Task.Delay((int)(WaitTime * 1000));
        }
    }
}

