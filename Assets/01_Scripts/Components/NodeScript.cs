using UnityEngine;

namespace CoreSystem
{
    public class NodeScript : MonoBehaviour
    {
        [field: SerializeField] public bool CanMoveUp { get; private set; } = false;
        [field: SerializeField] public bool CanMoveDown { get; private set; } = false;
        [field: SerializeField] public bool CanMoveLeft { get; private set; } = false;
        [field: SerializeField] public bool CanMoveRight { get; private set; } = false;

        [field: SerializeField] public GameObject NodeUp { get; private set; }
        [field: SerializeField] public GameObject NodeDown { get; private set; }
        [field: SerializeField] public GameObject NodeLeft { get; private set; }
        [field: SerializeField] public GameObject NodeRight { get; private set; }

        public GameObject pelletPrefab;
        public GameObject powerPelletPrefab;
        public GameObject fruitPrefab;

        public bool isGhostStartingNode = false;

        void Start()
        {
            //CheckAvailableMoves();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, Vector3.forward);

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, Vector3.back);

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, Vector3.right);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, Vector3.left);
        }

        public void CheckAvailableMoves(float nodeDistance = 1f)
        {
            if (Physics.Raycast(transform.position, Vector3.forward, out RaycastHit hitUp, nodeDistance, LayerMask.GetMask("Nodes")))
            {
                CanMoveUp = true;
                NodeUp = hitUp.collider.gameObject;
            }

            if (Physics.Raycast(transform.position, Vector3.back, out RaycastHit hitDown, nodeDistance, LayerMask.GetMask("Nodes")))
            {
                CanMoveDown = true;
                NodeDown = hitDown.collider.gameObject;
            }

            if (Physics.Raycast(transform.position, Vector3.right, out RaycastHit hitRight, nodeDistance, LayerMask.GetMask("Nodes")))
            {
                CanMoveRight = true;
                NodeRight = hitRight.collider.gameObject;
            }

            if (Physics.Raycast(transform.position, Vector3.left, out RaycastHit hitLeft, nodeDistance, LayerMask.GetMask("Nodes")))
            {
                CanMoveLeft = true;
                NodeLeft = hitLeft.collider.gameObject;
            }

            if (isGhostStartingNode)
            {
                CanMoveDown = true;
                NodeDown = GameManager.Instance.nodeCentre;
            }
        }
            
        public bool ValidateNodePosition()
        {
            Collider[] walls = Physics.OverlapSphere(transform.position, 0.25f, LayerMask.GetMask("Walls"));
            if (walls.Length > 0)
            {
                string wallNames = string.Join(", ", System.Array.ConvertAll(walls, wall => wall.gameObject.name));
                Debug.LogError($"Node '{gameObject.name}' is overlapping with wall(s): {wallNames}. Please adjust the node's position.");
                return false;   
            }
            return true;
        }
    }
}