using System.Threading.Tasks;
using UnityEngine;

namespace CoreSystem
{
    public abstract class LevelInitStepSO : ScriptableObject, ILevelInitStep
    {
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField, Min(1)] public int Weight { get; private set; } = 1;
        [field: SerializeField, Min(0)] public int StepPriority { get; private set; } = 0;

        public abstract Task Run(LevelContext context);
    }
}

