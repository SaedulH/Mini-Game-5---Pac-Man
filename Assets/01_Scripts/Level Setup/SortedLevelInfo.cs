using System.Collections.Generic;
using UnityEngine;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "SortedLevelInfo", menuName = "Levels/SortedLevelInfo")]
    public class SortedLevelInfo : ScriptableObject
    {
        [field: SerializeField] public string LevelName { get; private set; }
        [field: SerializeField] public int LevelIndex { get; private set; }

        [field: Header("Step Order")]
        [field: SerializeField] public List<LevelSetupStepSO> SceneEssentials { get; private set; }
        [field: SerializeField] public List<LevelSetupStepSO> ResetComponents { get; private set; }
        [field: SerializeField] public List<LevelSetupStepSO> UIElements { get; private set; }
        [field: SerializeField] public List<LevelSetupStepSO> GameplayObjects { get; private set; }
        [field: SerializeField] public List<LevelSetupStepSO> FinaliseSceneState { get; private set; }
    }
}

