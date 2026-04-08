using System.Threading.Tasks;
using AudioSystem;
using UnityEngine;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Init Step", menuName = "Levels/InitSteps/InitMusicStep")]
    public class InitMusicStepSO : LevelInitStepSO
    {
        [SerializeField] public AudioData TrackBGM;
        public override async Task Run(LevelContext context)
        {
            MusicManager.Instance.PlayMusic(TrackBGM);

            await Task.CompletedTask;
        }
    }
}

