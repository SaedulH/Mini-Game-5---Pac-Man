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
            CheckAvailableMoves();
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
            if (Physics.BoxCast(transform.position, Vector3.one * 0.25f, Vector3.zero, out RaycastHit hit, Quaternion.identity, 0, LayerMask.GetMask("Walls")))
            {
                Debug.LogWarning($"{gameObject.name} is overlapping with walls: {hit.collider.gameObject.name}");
                return false;
            }
            return true;
        }
    }
}