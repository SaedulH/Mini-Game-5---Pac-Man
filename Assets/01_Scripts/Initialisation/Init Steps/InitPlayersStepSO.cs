using System.Threading.Tasks;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Init Players Step", menuName = "Levels/InitSteps/InitPlayersStep")]
    public class InitPlayersStepSO : LevelInitStepSO
    {
        public override async Task Run(LevelContext context)
        {
            // Setup Player One
            //await GameManager.Instance.ConfigurePlayer(1, context.VehicleOne);

            // Setup Player Two
            if (context.PlayerCount == 2)
            {
                //await GameManager.Instance.ConfigurePlayer(2, context.VehicleTwo);
            }
            else if (context.GameMode == GameMode.Race)
            {
                string aiDifficulty = PlayerPrefs.GetString("Difficulty");
                //await GameManager.Instance.ConfigureAI(context.VehicleTwo, aiDifficulty);
            }
        }
    }
}

