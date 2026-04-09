using System.Threading.Tasks;
using AudioSystem;
using UnityEngine;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Setup Step", menuName = "Levels/SetupSteps/SetupMusicStep")]
    public class SetupMusicStepSO : LevelSetupStepSO
    {
        [SerializeField] public AudioData TrackBGM;
        public override async Task Run(LevelContext context)
        {
            MusicManager.Instance.PlayMusic(TrackBGM);

            await Task.CompletedTask;
        }
    }
}

