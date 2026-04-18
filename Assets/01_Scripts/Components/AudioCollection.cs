using AudioSystem;
using UnityEngine;
using UnityEngine.UIElements;
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

        public void SetupHoverAudio(VisualElement root)
        {
            root.Query<Button>().ForEach(button =>
            {
                bool hovered = false;
                button.RegisterCallback<PointerEnterEvent>(_ =>
                {
                    if (hovered || !button.enabledSelf)
                        return;

                    hovered = true;

                    PlayHoverAudio();
                });

                button.RegisterCallback<PointerLeaveEvent>(_ =>
                {
                    hovered = false;
                });
            });
        }

        public void PlaySelectAudio(bool playSound = true)
        {
            if (!playSound) return;
            AudioManager.Instance.CreateAudioBuilder()
                .Play(SelectAudio);
        }

        public void PlayBackAudio(bool playSound = true)
        {
            if (!playSound) return;
            AudioManager.Instance.CreateAudioBuilder()
                .Play(BackAudio);
        }

        private void PlayHoverAudio(bool playSound = true)
        {
            if (!playSound) return;
            AudioManager.Instance.CreateAudioBuilder()
                .Play(HoverAudio);
        }

        public void PlayCollectAudio(NodeType nodeType, bool playSound = true)
        {
            if (!playSound) return;
            AudioData audioData = nodeType switch
            {
                NodeType.Pellet => CollectPelletAudio,
                NodeType.PowerPellet => CollectPowerPelletAudio,
                NodeType.Fruit => CollectFruitAudio,
                _ => null
            };
            if (audioData != null)
            {
                AudioManager.Instance.CreateAudioBuilder()
                    .Play(audioData);
            }
        }
    }
}