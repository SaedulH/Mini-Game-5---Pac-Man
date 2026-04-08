using AudioSystem;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    public class PelletScript : MonoBehaviour
    {
        [field: SerializeField] private int Score { get; set; }
        [field: SerializeField] private bool IsPowerPellet {  get; set; }
        [field: SerializeField] private bool IsActive { get; set; } = true;
        [field: SerializeField] public SpriteRenderer PelletRenderer { get; private set; }
        [field: SerializeField] public Animator Anim { get; private set; }
        [field: SerializeField] public AudioData CollectAudio { get; private set; }

        private void Awake()
        {
            PelletRenderer = GetComponent<SpriteRenderer>();
            Anim = GetComponent<Animator>();
        }

        private void Start()
        {
            Score = IsPowerPellet ? Constants.POWER_PELLET_SCORE : Constants.PELLET_SCORE;
            Anim.SetTrigger("Active");
            IsActive = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player") && IsActive)
            {
                IsActive = false;
                PelletRenderer.enabled = false;
                Anim.SetTrigger("Collected");
                AudioManager.Instance.CreateAudioBuilder()
                    .WithRandomPitch()
                    .WithPosition(transform.position)
                    .Play(CollectAudio);
                GameManager.Instance.AddScore(Score, true);
                if (IsPowerPellet)
                {
                    if (collision.TryGetComponent(out PlayerManager playerManager)) {
                        playerManager.ActivatePowerMode();
                    }
                }
            }
        }
    }
}