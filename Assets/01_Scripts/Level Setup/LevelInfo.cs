using System.Collections.Generic;
using UnityEngine;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "LevelInfo", menuName = "Levels/LevelInfo")]
    public class LevelInfo : ScriptableObject
    {
        [field: SerializeField] public string LevelName { get; private set; }
        [field: SerializeField] public int LevelIndex { get; private set; }
        [field: SerializeField] public List<LevelSetupStepSO> StepOrder { get; private set; }
    }
}

