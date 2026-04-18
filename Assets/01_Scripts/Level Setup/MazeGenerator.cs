using CoreSystem;
using System.Threading.Tasks;
using UnityEngine;
using Utilities;

public class MazeGenerator : NonPersistentSingleton<MazeGenerator>
{
    [field: SerializeField] public GameObject NodeParent { get; private set; }
    [field: SerializeField] public GameObject WallParent { get; private set; }

    public int width = 26;
    public int height = 29;
    public float nodeDistance = 1f;
    public Vector3 startPosition = new(-13.5f, 2.5f, -14.5f);

    public NodeScript nodePrefab;
    [field: SerializeField] public NodeScript[,] Nodes { get; private set; }

    private void GenerateRandomMaze()
    {

    }

    private void GenerateRandomMaze(int seed)
    {

    }

    private void GenerateWalls()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                // generate the 1x1 'Cells' for each not path position

                // Merge all connected Cells into walls and add colliders to them

                // procedurally generate curves and corners for the walls using 3D tile rules
            }
        }
    }

    [ContextMenu("Generate and Validate Nodes")]
    public async Task GenerateAndValidateNodes()
    {
        if (nodePrefab == null)
        {
            Debug.LogError("Nodes prefab is not assigned.");
            return;
        }
        Nodes = new NodeScript[width, height];
        
        await GenerateNodes();

        await ValidateNodes();
    }

    public async Task GenerateNodes()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector3 position = new(
                    startPosition.x + i * nodeDistance,
                    startPosition.y,
                    startPosition.z + j * nodeDistance
                );
                Collider[] results = new Collider[10];
                int wallCount = Physics.OverlapSphereNonAlloc(position, 0.25f, results, LayerMask.GetMask("Walls"));
                if (wallCount == 0)
                {
                    NodeScript node = Instantiate(nodePrefab, position, Quaternion.identity, NodeParent.transform);
                    Nodes[i, j] = node;
                    node.name = $"Node_{i}_{j}";
                }
            }
        }

        await Task.CompletedTask;
    }

    public async Task ValidateNodes()
    {
        if (Nodes == null || Nodes.Length == 0)
        {
            Debug.LogWarning("Nodes array is not initialized.");
            NodeScript[] nodes = NodeParent.GetComponentsInChildren<NodeScript>();
            foreach (NodeScript node in nodes)
            {
                if (!node.ValidateNodePosition())
                {
                    Debug.LogWarning($"Node '{node.gameObject.name}' has invalid position.");
                    node.gameObject.SetActive(false);
                    continue;
                }
                node.CheckAvailableMoves(nodeDistance);
            }
        }
        else
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    NodeScript node = Nodes[i, j];
                    if (node != null)
                    {
                        if (!node.ValidateNodePosition())
                        {
                            Debug.LogWarning($"Node at position ({i}, {j}) has invalid position.");
                            node.gameObject.SetActive(false);
                            continue;
                        }
                        node.CheckAvailableMoves(nodeDistance);

                        node.SetNodeType(NodeType.Pellet);
                    }
                }
            }
        }
        await Task.CompletedTask;
    }

    #region Editor Validation

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

    [ContextMenu("Generate Nodes")]
    private void GenerateNodesVoid()
    {
        if (nodePrefab == null)
        {
            Debug.LogError("Nodes prefab is not assigned.");
            return;
        }
        Nodes = new NodeScript[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector3 position = new(
                    startPosition.x + i * nodeDistance,
                    startPosition.y,
                    startPosition.z + j * nodeDistance
                );

                NodeScript node = Instantiate(nodePrefab, position, Quaternion.identity, NodeParent.transform);
                Nodes[i, j] = node;
                node.name = $"Node_{i}_{j}";
            }
        }
    }

    [ContextMenu("Clear Nodes")]
    private void ClearNodes()
    {
        if (Nodes == null || Nodes.Length == 0)
        {
            Debug.LogWarning("Nodes array is not initialized.");
            NodeScript[] nodes = NodeParent.GetComponentsInChildren<NodeScript>();
            foreach (NodeScript node in nodes)
            {
                if (node != null)
                {
#if UNITY_EDITOR
                    DestroyImmediate(node.gameObject);
#endif
                    Destroy(node.gameObject);
                }
            }
            Nodes = new NodeScript[width, height];
            return;
        }
        Debug.LogWarning("Nodes array is initialized. Size: " + Nodes.Length);

        for (int i = 0; i < Nodes.GetLength(0); i++)
        {
            for (int j = 0; j < Nodes.GetLength(1); j++)
            {
                if (Nodes[i, j] != null)
                {
#if UNITY_EDITOR
                    DestroyImmediate(Nodes[i, j].gameObject);
#endif
                    Destroy(Nodes[i, j].gameObject);
                    Nodes[i, j] = null;           
                }
            }
        }

        Nodes = null;
    }

    [ContextMenu("Validate Nodes")]
    private void ValidateNodesVoid()
    {
        Physics.SyncTransforms();

        if (Nodes == null || Nodes.Length == 0)
        {
            Debug.LogWarning("Nodes array is not initialized.");
            NodeScript[] nodeObjs = gameObject.GetComponentsInChildren<NodeScript>();
            foreach (NodeScript node in nodeObjs)
            {
                if (!node.ValidateNodePosition())
                {
                    Debug.LogWarning($"Node '{node.gameObject.name}' has invalid position.");
                    node.gameObject.SetActive(false);
                    continue;
                }
                node.CheckAvailableMoves(nodeDistance);
            }
            return;
        }
        for (int i = 0; i < Nodes.GetLength(0); i++)
        {
            for (int j = 0; j < Nodes.GetLength(1); j++)
            {
                if (Nodes[i, j] == null)
                {
                    Debug.LogWarning($"Node at position ({i}, {j}) is missing.");
                    continue;
                }

                NodeScript nodeScript = Nodes[i, j];
                if (!nodeScript.ValidateNodePosition())
                {
                    Debug.LogWarning($"Node at position ({i}, {j}) has invalid position.");
                    nodeScript.gameObject.SetActive(false);
                    continue;
                }
                nodeScript.CheckAvailableMoves(nodeDistance);
            }
        }
    }
    #endregion
}
