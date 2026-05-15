using UnityEngine;
using Utilities;

namespace CoreSystem
{
    public class WallScript : MonoBehaviour
    {
        [field: SerializeField] public WallNodeType WallNodeType { get; private set; }

        [field: Header("Connected Wall Nodes")]
        [field: SerializeField] public WallScript WallTop { get; private set; }
        [field: SerializeField] public WallScript WallTopLeft { get; private set; }
        [field: SerializeField] public WallScript WallTopRight { get; private set; }
        [field: SerializeField] public WallScript WallBottom { get; private set; }
        [field: SerializeField] public WallScript WallBottomLeft { get; private set; }
        [field: SerializeField] public WallScript WallBottomRight { get; private set; }
        [field: SerializeField] public WallScript WallLeft { get; private set; }
        [field: SerializeField] public WallScript WallRight { get; private set; }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = WallTop != null ? Color.green : Color.red;
            Gizmos.DrawRay(transform.position, Vector3.forward);

            Gizmos.color = WallTopLeft != null ? Color.green : Color.red;
            Gizmos.DrawRay(transform.position, (Vector3.forward + Vector3.left).normalized);

            Gizmos.color = WallTopRight != null ? Color.green : Color.red;
            Gizmos.DrawRay(transform.position, (Vector3.forward + Vector3.right).normalized);

            Gizmos.color = WallBottom != null ? Color.green : Color.red;
            Gizmos.DrawRay(transform.position, Vector3.back);

            Gizmos.color = WallBottomLeft != null ? Color.green : Color.red;
            Gizmos.DrawRay(transform.position, (Vector3.back + Vector3.left).normalized);

            Gizmos.color = WallBottomRight != null ? Color.green : Color.red;
            Gizmos.DrawRay(transform.position, (Vector3.back + Vector3.right).normalized);

            Gizmos.color = WallRight != null ? Color.green : Color.red;
            Gizmos.DrawRay(transform.position, Vector3.right);

            Gizmos.color = WallLeft != null ? Color.green : Color.red;
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
            WallTop = GetWallAtOffset(Vector3.forward, nodeDistance);
            WallTopLeft = GetWallAtOffset((Vector3.forward + Vector3.left).normalized, nodeDistance);
            WallTopRight = GetWallAtOffset((Vector3.forward + Vector3.right).normalized, nodeDistance);
            WallBottom = GetWallAtOffset(Vector3.back, nodeDistance);
            WallBottomLeft = GetWallAtOffset((Vector3.back + Vector3.left).normalized, nodeDistance);
            WallBottomRight = GetWallAtOffset((Vector3.back + Vector3.right).normalized, nodeDistance);
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
            SetWallType(MazeGenerator.Instance.WallTypes);
        }
            
        public void SetWallType(WallType[] wallTypes, bool isBoundary = false)
        {
            bool hasTop = WallTop != null;
            bool hasTopLeft = WallTopLeft != null;
            bool hasTopRight = WallTopRight != null;
            bool hasBottom = WallBottom != null;
            bool hasBottomLeft = WallBottomLeft != null;
            bool hasBottomRight = WallBottomRight != null;
            bool hasLeft = WallLeft != null;
            bool hasRight = WallRight != null;

            WallType result = null;
            foreach (WallType type in wallTypes)
            {
                if (type.Matches(
                    isBoundary,
                    hasTop,
                    hasTopLeft,
                    hasTopRight,
                    hasBottom,
                    hasBottomLeft,
                    hasBottomRight,
                    hasLeft,
                    hasRight))
                {
                    result = type;
                    break;
                }
            }

            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if (result == null)
            {
                WallNodeType = WallNodeType.None;
                Debug.LogError($"No matching wall type found for wall '{gameObject.name}' with surrounding configuration: " +
                    $"Top: {hasTop}, TopLeft: {hasTopLeft}, TopRight: {hasTopRight}, " +
                    $"Bottom: {hasBottom}, BottomLeft: {hasBottomLeft}, BottomRight: {hasBottomRight}, " +
                    $"Left: {hasLeft}, Right: {hasRight}");
                if (meshFilter != null)
                {
                    meshFilter.sharedMesh = null;
                    transform.localScale = Vector3.one;
                }
            }
            else
            {
                WallNodeType = result.Description;
                if (meshFilter != null)
                {
                    meshFilter.sharedMesh = result.Mesh;
                    transform.rotation = result.YRotation != 0 ? Quaternion.Euler(0, result.YRotation, 0) : Quaternion.identity;
                }
            }
        }
    }
}