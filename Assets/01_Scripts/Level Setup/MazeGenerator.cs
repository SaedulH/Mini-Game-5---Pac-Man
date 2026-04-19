using CoreSystem;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

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

    #region Node Generation and Validation

    private void IterateNodes(Action<NodeScript> action)
    {
        if (Nodes == null || Nodes.Length == 0)
        {
            Debug.LogWarning("[IterateNodes] Nodes array is not initialized.");
            NodeScript[] nodes = NodeParent.GetComponentsInChildren<NodeScript>();

            foreach (NodeScript node in nodes)
            {
                if (node != null)
                {
                    action(node);
                }
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
                        action(node);
                    }
                }
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
        IterateNodes(node =>
        {
            if (!node.ValidateNodePosition())
            {
                Debug.LogWarning($"Node '{node.gameObject.name}' has invalid position.");
                node.gameObject.SetActive(false);
                return;
            }

            node.CheckAvailableMoves(nodeDistance);
        });

        await Task.CompletedTask;
    }

    [ContextMenu("Assign Node Type")]
    public void AssignNodeType()
    {
        IterateNodes(node => node.SetNodeType());
    }

    [ContextMenu("Set Total Pellet Count")]
    public async Task SetTotalPelletCount()
    {
        int pelletCount = 0;

        IterateNodes(node =>
        {
            if (node.NodeType == NodeType.Pellet || node.NodeType == NodeType.PowerPellet)
                pelletCount++;
        });

        GameManager.Instance.TotalPelletCount = pelletCount;
        await Task.CompletedTask;
    }

    [ContextMenu("Clear Nodes")]
    private void ClearNodes()
    {
        IterateNodes(node => DestroyImmediate(node.gameObject));
        Nodes = null;
    }

    public async Task SpawnFruit(int currentLevel)
    {
        bool isFruitSpawned = false;
        while (!isFruitSpawned)
        {
            IterateNodes(node =>
            {
                if (!isFruitSpawned && IsNodeValidForFruit(node))
                {
                    int num = Random.Range(0, 250);
                    if (num < 5)
                    {
                        node.SpawnFruit(currentLevel);
                        isFruitSpawned = true;
                    }
                }
            });
        }

        await Task.CompletedTask;
    }

    private bool IsNodeValidForFruit(NodeScript node)
    {
        float nodeHeight = node.transform.position.z;
        if (nodeHeight <= -5.5 || nodeHeight >= 5.5)
        {
            return false;
        }

        if (node.NodeType != NodeType.Pellet)
        {
            return false;
        }
        return node.NodeType == NodeType.Pellet;
    }

    #endregion
}
