using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreSystem
{
    public class NodeScript : MonoBehaviour
    {

        public bool canMoveUp = false;
        public bool canMoveDown = false;
        public bool canMoveLeft = false;
        public bool canMoveRight = false;

        public GameObject nodeUp;
        public GameObject nodeDown;
        public GameObject nodeLeft;
        public GameObject nodeRight;

        public bool isGhostStartingNode = false;
        // Start is called before the first frame update
        void Start()
        {
            CheckAvailableMoves();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void CheckAvailableMoves()
        {
            RaycastHit2D hitUp = Physics2D.Raycast(transform.position, Vector2.up, 1f, ~LayerMask.GetMask("Ignore Raycast"));
            if (hitUp.collider != null && (hitUp.collider.gameObject.CompareTag("PathwayNode")))
            {
                canMoveUp = true;
                nodeUp = hitUp.collider.gameObject;
            }

            RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, 1f, ~LayerMask.GetMask("Ignore Raycast"));
            if (hitDown.collider != null && (hitDown.collider.gameObject.CompareTag("PathwayNode")))
            {
                canMoveDown = true;
                nodeDown = hitDown.collider.gameObject;
            }

            RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, 1f, ~LayerMask.GetMask("Ignore Raycast"));
            if (hitRight.collider != null && (hitRight.collider.gameObject.CompareTag("PathwayNode")))
            {
                canMoveRight = true;
                nodeRight = hitRight.collider.gameObject;
            }

            RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, 1f, ~LayerMask.GetMask("Ignore Raycast"));
            if (hitLeft.collider != null && (hitLeft.collider.gameObject.CompareTag("PathwayNode")))
            {
                canMoveLeft = true;
                nodeLeft = hitLeft.collider.gameObject;
            }

            if (isGhostStartingNode)
            {
                canMoveDown = true;
                nodeDown = GameManager.Instance.nodeCentre;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Walls"))
            {
                Destroy(gameObject);
            }
        }
    }
}