using System.Collections;
using UnityEngine;
using Utilities;

namespace CoreSystem
{

    [RequireComponent(typeof(Movement))]
    [RequireComponent(typeof(Animator))]
    public class PlayerManager : NonPersistentSingleton<PlayerManager>
    {
        public int remainingLives = 4;
        public bool isAlive = true;
        public bool isPowerMode = false;
        [field: SerializeField] public Movement Movement { get; private set; }
        [field: SerializeField] public Animator Anim { get; private set; }

        private Vector3 startPosition = new Vector3(-3, -7, 0);
        [SerializeField] private GameObject startNode;
        [SerializeField] private SpriteRenderer body;

        protected override void Awake()
        {
            base.Awake();
            Movement = GetComponent<Movement>();
            Anim = GetComponent<Animator>();
        }

        private void Start()
        {
            transform.position = startPosition;
            Movement.currentNode = startNode;
            Movement.lastMove = "right";
        }

        public void ActivatePowerMode()
        {

        }

        public void DeactivatePowerMode()
        {

        }

        public IEnumerator PowerMode()
        {
            Debug.Log("Start powermode");
            isPowerMode = true;
            body.color = new Color32(255, 206, 206, 255);
            yield return new WaitForSeconds(10);

            body.color = new Color32(255, 255, 255, 255);
            isPowerMode = false;
            Debug.Log("end powermode");
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ghost"))
            {
                if (!isPowerMode)
                {
                    isAlive = false;
                    StartCoroutine(RespawnPacman());
                }
                else
                {
                    GhostManager ghostManager = collision.gameObject.GetComponent<GhostManager>();
                    if (ghostManager.ghostNodeState != GhostManager.GhostNodeStateEnum.Respawning)
                    {
                        GameManager.Instance.AddScore(800, false);
                        ghostManager.ghostNodeState = GhostManager.GhostNodeStateEnum.Respawning;
                    }
                }
            }
        }

        private IEnumerator RespawnPacman()
        {
            if (remainingLives > 1)
            {
                remainingLives--;
                yield return new WaitForSeconds(2);
                transform.position = startPosition;
                Movement.currentNode = startNode;
                Movement.lastMove = "right";
                yield return new WaitForSeconds(1);
                isAlive = true;


            }
            else
            {
                //gameOver screen
                yield return new WaitForSeconds(1);
            }
        }
    }
}