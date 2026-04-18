using UnityEngine;
using Utilities;

namespace CoreSystem
{

    [RequireComponent(typeof(Animator))]
    public class ItemScript : MonoBehaviour
    {
        [field: SerializeField] private int Score { get; set; }
        [field: SerializeField] private NodeType ItemType { get; set; }
        [field: SerializeField] private bool IsActive { get; set; } = true;
        [field: SerializeField] public Animator Anim { get; private set; }

        private void Awake()
        {
            Anim = GetComponent<Animator>();
        }

        private void Start()
        {
            Score = Constants.GetItemScore(ItemType);
            Anim.SetTrigger("Active");
            IsActive = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player") && IsActive)
            {
                IsActive = false;
                Anim.SetTrigger("Collected");
                AudioCollection.Instance.PlayCollectAudio(ItemType);
                GameManager.Instance.AddScore(Score, true);
                if (ItemType == NodeType.PowerPellet)
                {
                    if (collision.TryGetComponent(out PlayerManager playerManager))
                    {
                        playerManager.ActivatePowerMode();
                    }
                }
            }
        }
    }
}