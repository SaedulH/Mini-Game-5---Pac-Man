using System.Threading.Tasks;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Enter UI State Step", menuName = "Levels/SetupSteps/EnterUIStateStep")]
    public class EnterUIStateStepSO : LevelSetupStepSO
    {
        [SerializeField] private UIState NewUIState;

        public override async Task Run(LevelContext context)
        {
            UIManager.Instance.OnUIStateChanged(NewUIState);

            await Task.Yield();
        }
    }
}

