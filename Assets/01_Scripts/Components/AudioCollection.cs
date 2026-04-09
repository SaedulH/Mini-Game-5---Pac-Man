using AudioSystem;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    public class AudioCollection : NonPersistentSingleton<AudioCollection>
    {
        [field: Header("UI")]
        [field: SerializeField] public AudioData StartAudio { get; set; }
        [field: SerializeField] public AudioData PauseAudio { get; set; }
        [field: SerializeField] public AudioData SelectAudio { get; set; }
        [field: SerializeField] public AudioData CountdownAudio { get; set; }
        [field: SerializeField] public AudioData BeginAudio { get; set; }
        [field: SerializeField] public AudioData BackAudio { get; set; }
        [field: SerializeField] public AudioData HoverAudio { get; set; }
        [field: SerializeField] public AudioData ResetAudio { get; set; }
        [field: SerializeField] public AudioData GameOverAudio { get; set; }
        [field: SerializeField] public AudioData LevelCompleteAudio { get; set; }

        [field: Header("Effects")]
        [field: SerializeField] public AudioData PlayerMoveAudio { get; set; }
        [field: SerializeField] public AudioData GhostMoveAudio { get; set; }
        [field: SerializeField] public AudioData CollectPelletAudio { get; set; }
        [field: SerializeField] public AudioData CollectPowerPelletAudio { get; set; }
        [field: SerializeField] public AudioData CollectFruitAudio { get; set; }
        [field: SerializeField] public AudioData TeleportAudio { get; set; }
        [field: SerializeField] public AudioData KillGhostAudio { get; set; }
        [field: SerializeField] public AudioData KillPlayerAudio { get; set; }
    }
}