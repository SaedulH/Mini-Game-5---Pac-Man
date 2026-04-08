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
        [field: SerializeField] public List<LevelInitStepSO> SceneEssentials { get; private set; }
        [field: SerializeField] public List<LevelInitStepSO> ResetComponents { get; private set; }
        [field: SerializeField] public List<LevelInitStepSO> UIElements { get; private set; }
        [field: SerializeField] public List<LevelInitStepSO> GameplayObjects { get; private set; }
        [field: SerializeField] public List<LevelInitStepSO> FinaliseSceneState { get; private set; }
    }
}

