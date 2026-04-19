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
        [field: SerializeField] public NodeScript TeleportNodeLeft { get; private set; }
        [field: SerializeField] public NodeScript TeleportNodeRight { get; private set; }


        [field: Header("Node Type")]
        [field: SerializeField] public NodeType NodeType { get; private set; }
        [field: SerializeField] public ItemScript CurrentItem { get; private set; }
        [field: SerializeField] public ItemScript PelletPrefab { get; private set; }
        [field: SerializeField] public ItemScript PowerPelletPrefab { get; private set; }
        [field: SerializeField] public FruitItemScript FruitPrefab { get; private set; }
        [field: SerializeField] public TeleportScript TeleportPrefab { get; private set; }

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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.1f);
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
        public void SetNodeType()
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
                    SpawnFruit(1);
                    break;
                case NodeType.Teleport:
                    SetupTeleport();
                    break;
                case NodeType.PacManStart:
                    break;
                case NodeType.GhostStart:
                    break;
            }
        }

        private void SetupTeleport()
        {
            TeleportScript[] teleportScripts = gameObject.GetComponentsInChildren<TeleportScript>(true);
            foreach (TeleportScript teleportScript in teleportScripts)
            {
                DestroyImmediate(teleportScript.gameObject);
            }
            TeleportScript teleport = Instantiate(TeleportPrefab, transform.position, Quaternion.identity, transform);
            teleport.SetParentNode(this);
            bool isLeftTeleport = transform.position.x < 0;
            Vector3 rayDirection = isLeftTeleport ? Vector3.right : Vector3.left;
            if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, 50f, LayerMask.GetMask("Teleport")))
            {
                TeleportScript otherTeleport = hit.collider.TryGetComponent(out TeleportScript teleportScript) ? teleportScript : null;
                if (otherTeleport == null || otherTeleport.ParentNode == null) return;

                if (isLeftTeleport)
                {
                    TeleportNodeRight = otherTeleport.ParentNode;
                    otherTeleport.ParentNode.TeleportNodeLeft = this;
                } 
                else
                {
                    TeleportNodeLeft = otherTeleport.ParentNode;
                    otherTeleport.ParentNode.TeleportNodeRight = this;
                }
            }
        }

        private ItemScript SpawnItem(ItemScript item)
        {
            ItemScript[] existingItems = gameObject.GetComponentsInChildren<ItemScript>(true);
            foreach (ItemScript existingItem in existingItems)
            {
                if (existingItem.ItemType == item.ItemType)
                {
                    DestroyImmediate(existingItem.gameObject);
                } 
                else
                {
                    if (item.ItemType == NodeType.Fruit && IsCurrentlyActivePellet(existingItem))
                    {
                        _ = GameManager.Instance.EatPellet();
                    }
                    existingItem.gameObject.SetActive(false);
                }
            }
            CurrentItem = Instantiate(item, transform.position, Quaternion.identity, transform);
            return CurrentItem;
        }

        public void SpawnFruit(int currentLevel)
        {
            FruitItemScript fruit = SpawnItem(FruitPrefab) as FruitItemScript;
            fruit.SetFruitType(currentLevel);
            Debug.Log($"Spawned fruit of type {fruit.FruitType} at node '{gameObject.name}' for level {currentLevel}.");
        }

        private bool IsCurrentlyActivePellet(ItemScript item)
        {
            return CurrentItem != null && CurrentItem == item && CurrentItem.IsActive && 
                (CurrentItem.ItemType == NodeType.Pellet || CurrentItem.ItemType == NodeType.PowerPellet);
        }
    }
}