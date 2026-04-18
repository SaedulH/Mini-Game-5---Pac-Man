using System;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    public class NodeScript : MonoBehaviour
    {
        [field: Header("Available Moves")]
        [field: SerializeField] public bool CanMoveUp { get; private set; } = false;
        [field: SerializeField] public bool CanMoveDown { get; private set; } = false;
        [field: SerializeField] public bool CanMoveLeft { get; private set; } = false;
        [field: SerializeField] public bool CanMoveRight { get; private set; } = false;

        [field: Header("Connected Nodes")]
        [field: SerializeField] public NodeScript NodeUp { get; private set; }
        [field: SerializeField] public NodeScript NodeDown { get; private set; }
        [field: SerializeField] public NodeScript NodeLeft { get; private set; }
        [field: SerializeField] public NodeScript NodeRight { get; private set; }
        [field: SerializeField] public int TeleportIndex { get; private set; } = -1;

        [field: Header("Node Type")]
        [field: SerializeField] public NodeType NodeType { get; private set; }
        [field: SerializeField] public ItemScript PelletPrefab { get; private set; }
        [field: SerializeField] public ItemScript PowerPelletPrefab { get; private set; }
        [field: SerializeField] public ItemScript FruitPrefab { get; private set; }

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

        [ContextMenu("CheckAvailableMoves")]
        public void CheckAvailableMovesContextMenu()
        {
            CheckAvailableMoves(1f);
        }

        public void CheckAvailableMoves(float nodeDistance = 1f)
        {
            if (Physics.Raycast(transform.position, Vector3.forward, out RaycastHit hitUp, nodeDistance, LayerMask.GetMask("Nodes")))
            {
                CanMoveUp = true;
                NodeUp = hitUp.collider.GetComponent<NodeScript>();
            }

            if (Physics.Raycast(transform.position, Vector3.back, out RaycastHit hitDown, nodeDistance, LayerMask.GetMask("Nodes")))
            {
                CanMoveDown = true;
                NodeDown = hitDown.collider.GetComponent<NodeScript>();
            }

            if (Physics.Raycast(transform.position, Vector3.right, out RaycastHit hitRight, nodeDistance, LayerMask.GetMask("Nodes")))
            {
                CanMoveRight = true;
                NodeRight = hitRight.collider.GetComponent<NodeScript>();
            }

            if (Physics.Raycast(transform.position, Vector3.left, out RaycastHit hitLeft, nodeDistance, LayerMask.GetMask("Nodes")))
            {
                CanMoveLeft = true;
                NodeLeft = hitLeft.collider.GetComponent<NodeScript>();
            }

            //if (isGhostStartingNode)
            //{
            //    CanMoveDown = true;
            //    NodeDown = GameManager.Instance.nodeCentre;
            //}
        }

        [ContextMenu("ValidateNodePosition")]
        public bool ValidateNodePosition()
        {
            Collider[] walls = new Collider[10];
            int wallCount = Physics.OverlapSphereNonAlloc(transform.position, 0.25f, walls, LayerMask.GetMask("Walls"));
            if (wallCount > 0)
            {
                string wallNames = string.Join(", ", System.Array.ConvertAll(walls, wall => wall.gameObject.name));
                Debug.LogError($"Node '{gameObject.name}' is overlapping with wall(s): {wallNames}. Please adjust the node's position.");
                return false;   
            }
            return true;
        }


        [ContextMenu("SetNodeType")]
        public void SetNodeTypeContextMenu()
        {
            SetNodeType(NodeType);
        }

        public void SetNodeType(NodeType nodeType)
        {
            NodeType = nodeType;
            switch(nodeType)
            {
                case NodeType.Path:
                    break;
                case NodeType.Pellet:
                    SpawnItem(PelletPrefab);
                    break;
                case NodeType.PowerPellet:
                    SpawnItem(PowerPelletPrefab);
                    break;
                case NodeType.Fruit:
                    SpawnItem(FruitPrefab);
                    break;
                case NodeType.Teleport:
                    break;

                case NodeType.GhostStart:
                    break;
            }
        }

        private void SpawnItem(ItemScript item)
        {
            ItemScript[] existingItems = gameObject.GetComponentsInChildren<ItemScript>();
            foreach (ItemScript existingItem in existingItems)
            {
#if UNITY_EDITOR
                DestroyImmediate(existingItem.gameObject);
#endif
                Destroy(existingItem.gameObject);
            }
            Instantiate(item, transform.position, Quaternion.identity, transform);
        }
    }
}