using NUnit.Framework.Interfaces;
using UnityEngine;

namespace CoreSystem
{
    public class WallScript : MonoBehaviour {

        [field: Header("Connected Wall Nodes")]
        [field: SerializeField] public WallScript WallUp { get; private set; }
        [field: SerializeField] public WallScript WallDown { get; private set; }
        [field: SerializeField] public WallScript WallLeft { get; private set; }
        [field: SerializeField] public WallScript WallRight { get; private set; }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = WallUp != null ? Color.blue : Color.red;
            Gizmos.DrawRay(transform.position, Vector3.forward);

            Gizmos.color = WallDown != null ? Color.blue : Color.red;
            Gizmos.DrawRay(transform.position, Vector3.back);

            Gizmos.color = WallRight != null ? Color.blue : Color.red;
            Gizmos.DrawRay(transform.position, Vector3.right);

            Gizmos.color = WallLeft != null ? Color.blue : Color.red;
            Gizmos.DrawRay(transform.position, Vector3.left);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, 0.2f);
        }

        [ContextMenu("CheckSurroundingWalls")]
        public void CheckSurroundingWallsContextMenu()
        {
            CheckSurroundingWalls(1f);
        }

        public void CheckSurroundingWalls(float nodeDistance = 1f)
        {
            WallUp = GetWallAtOffset(Vector3.forward, nodeDistance);
            WallDown = GetWallAtOffset(Vector3.back, nodeDistance);
            WallRight = GetWallAtOffset(Vector3.right, nodeDistance);
            WallLeft = GetWallAtOffset(Vector3.left, nodeDistance);
        }

        private WallScript GetWallAtOffset(Vector3 offset, float nodeDistance)
        {
            Vector3 checkPosition = transform.position + offset * nodeDistance;

            Collider[] results = new Collider[5];

            int count = Physics.OverlapSphereNonAlloc(
                checkPosition,
                0.2f,
                results,
                LayerMask.GetMask("Walls"));

            for (int i = 0; i < count; i++)
            {
                WallScript wall = results[i].GetComponent<WallScript>();

                if (wall != null && wall != this)
                    return wall;
            }

            return null;
        }

        [ContextMenu("ValidateWallPosition")]
        public bool ValidateWallPosition()
        {
            Collider[] nodes = new Collider[10];
            int nodeCount = Physics.OverlapSphereNonAlloc(transform.position, 0.25f, nodes, LayerMask.GetMask("Nodes"));
            if (nodeCount > 0)
            {
                string nodeNames = string.Join(", ", System.Array.ConvertAll(nodes, node => node.gameObject.name));
                Debug.LogError($"Wall '{gameObject.name}' is overlapping with node(s): {nodeNames}. Please adjust the wall's position.");
                return false;
            }
            return true;
        }

        [ContextMenu("SetWallType")]
        public void SetWallType()
        {
            
        }
    }
}