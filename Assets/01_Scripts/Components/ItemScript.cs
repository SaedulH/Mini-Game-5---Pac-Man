using System;
using UnityEngine;
using Utilities;

namespace CoreSystem
{

    [RequireComponent(typeof(Animator))]
    public class ItemScript : MonoBehaviour
    {
        private static readonly int StateHash = Animator.StringToHash("State");

        [field: SerializeField] public int Score { get; protected set; }
        [field: SerializeField] public NodeType ItemType { get; private set; }
        [field: SerializeField] public bool IsActive { get; protected set; } = true;
        [field: SerializeField] public Animator Anim { get; private set; }
        [field: SerializeField] public Collider Collider { get; private set; }
        [field: SerializeField] public GameObject Body { get; private set; }

        private void Awake()
        {
            Anim = GetComponent<Animator>();
            Collider = GetComponent<Collider>();
        }

        private void Start()
        {
            SetItemScore();
            Anim.SetInteger(StateHash, 0);
            IsActive = true;
        }

        protected virtual void SetItemScore()
        {
            Score = Constants.GetItemScore(ItemType);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                CollectItem(other);
            }
        }

        public void CollectItem(Collider other)
        {
            if (IsActive)
            {
                IsActive = false;
                Anim.SetInteger(StateHash, 1);
                AudioCollection.Instance.PlayCollectAudio(ItemType);
                bool isPellet = ItemType == NodeType.Pellet || ItemType == NodeType.PowerPellet;
                _ = GameManager.Instance.AddScore(Score, isPellet);
                if (ItemType == NodeType.PowerPellet)
                {
                    CameraShake.Instance.ShakeCamera(
                        Constants.CAMERA_SHAKE_INTENSITY_MEDIUM, 
                        Constants.CAMERA_SHAKE_DURATION_MEDIUM);
                    if (other.TryGetComponent(out PlayerManager playerManager))
                    {
                        playerManager.ActivatePowerMode();
                    }
                }
            }
        }

        public void OnCollectedEvent()
        {
            Collider.enabled = false;
            Body.SetActive(false);
            Anim.SetInteger(StateHash, 2);
        }

        public void OnResetEvent()
        {
            Collider.enabled = true;
            Body.SetActive(true);
            Anim.SetInteger(StateHash, 0);
            IsActive = true;
        }
    }
}