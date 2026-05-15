using CoreSystem;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

public class MazeGenerator : NonPersistentSingleton<MazeGenerator>
{
    [field: Header("Parents")]
    [field: SerializeField] public GameObject NodeParent { get; private set; }
    [field: SerializeField] public GameObject WallParent { get; private set; }
    [field: SerializeField] public GameObject WallProtoParent { get; private set; }

    [field: Header("Dimensions")]
    public int width = 26;
    public int height = 29;
    public float nodeDistance = 1f;
    public Vector3 startPosition = new(-13.5f, 2.5f, -14.5f);

    [field: Header("Wall Meshes")]
    [field: SerializeField] public WallType[] WallTypes { get; private set; }

    [field: Header("Prefabs")]
    public NodeScript nodePrefab;
    public WallScript wallPrefab;
    [field: SerializeField] public NodeScript[,] Nodes { get; private set; }
    [field: SerializeField] public WallScript[,] Walls { get; private set; }

    public void Initialise(LevelContext context)
    {

    }

    #region Path Generation

    //[ContextMenu("Generate Random Maze")]
    //private void GenerateRandomMaze(int seed = 0)
    //{
    //    //Random path generator logic here
    //}

    #endregion

    #region Wall Generation

    private void IterateWalls(Action<WallScript> action)
    {
        if (Walls == null || Walls.Length == 0)
        {
            Debug.LogWarning("[IterateWalls] Walls array is not initialized.");
            WallScript[] walls = WallParent.GetComponentsInChildren<WallScript>(true);

            foreach (WallScript wall in walls)
            {
                if (wall != null)
                {
                    action(wall);
                }
            }
        }
        else
        {
            for (int i = 0; i < width + 2; i++)
            {
                for (int j = 0; j < height + 2; j++)
                {
                    WallScript wall = Walls[i, j];
                    if (wall != null)
                    {
                        action(wall);
                    }
                }
            }
        }
    }

    [ContextMenu("Generate and Validate Wall Nodes")]
    public async Task GenerateAndValidateWallNodes()
    {
        if (wallPrefab == null)
        {
            Debug.LogError("Wall prefab is not assigned.");
            return;
        }
        Walls = new WallScript[width + 2, height + 2];

        await GenerateWallNodes();

        await ValidateWallNodes();
    }

    public async Task GenerateWallNodes()
    {
        int wallWidth = width + 2;  
        int wallHeight = height + 2;  
        Vector3 wallStartPosition = startPosition + new Vector3(-1f, 0, -1f);

        for (int i = 0; i < wallWidth; i++)
        {
            for (int j = 0; j < wallHeight; j++)
            {
                Vector3 position = new(
                    wallStartPosition.x + i * nodeDistance,
                    wallStartPosition.y,
                    wallStartPosition.z + j * nodeDistance
                );
                Collider[] results = new Collider[10];
                int wallCount = Physics.OverlapSphereNonAlloc(position, 0.25f, results, LayerMask.GetMask("Walls"));
                if (wallCount > 0)
                {
                    WallScript wall = Instantiate(wallPrefab, position, Quaternion.identity, WallParent.transform);
                    Walls[i, j] = wall;
                    wall.name = $"Wall_{i}_{j}";
                }
            }
        }

        await Task.CompletedTask;
    }

    [ContextMenu("Validate Walls")]
    public async Task ValidateWallNodes()
    {
        IterateWalls(wall =>
        {
            if (!wall.ValidateWallPosition())
            {
                Debug.LogWarning($"Wall '{wall.gameObject.name}' has invalid position.");
                wall.gameObject.SetActive(false);
                return;
            }

            wall.CheckSurroundingWalls(nodeDistance);
        });

        await Task.CompletedTask;
    }

    [ContextMenu("Assign Wall Type")]
    public void AssignWallType()
    {
        IterateWalls(wall => wall.SetWallType(WallTypes));
    }

    [ContextMenu("Clear Walls")]
    private void ClearWalls()
    {
        IterateWalls(wall => DestroyImmediate(wall.gameObject));
        Walls = null;
    }

    [ContextMenu("Combine Walls")]
    private void CombineWalls()
    {
        MeshFilter[] meshFilters = WallParent.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine);

        GetComponent<MeshFilter>().mesh = combinedMesh;

        IterateWalls(wall => wall.gameObject.SetActive(false));
    }

    private void IterateProtoWalls(bool enabled)
    {
        WallProtoParent.SetActive(enabled);
        //Transform[] walls = WallProtoParent.GetComponentsInChildren<Transform>();
        //foreach (Transform wall in walls)
        //{
        //    wall.gameObject.SetActive(enabled);
        //    //if (wall.gameObject.TryGetComponent(out Collider collider))
        //    //{
        //    //    collider.enabled = enabled;
        //    //}

        //    //if (wall.gameObject.TryGetComponent(out MeshRenderer renderer))
        //    //{
        //    //    renderer.enabled = enabled;
        //    //}
        //}
    }

    [ContextMenu("Enable Proto Walls")]
    private void EnableProtoWalls()
    {
        IterateProtoWalls(true);
    }

    [ContextMenu("Disable Proto Walls")]
    private void DisableProtoWalls()
    {
        IterateProtoWalls(false);
    }

    #endregion

    #region Node Generation

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

    [ContextMenu("Generate and Validate Path Nodes")]
    public async Task GenerateAndValidatePathNodes()
    {
        if (nodePrefab == null)
        {
            Debug.LogError("Node prefab is not assigned.");
            return;
        }
        Nodes = new NodeScript[width, height];

        await GeneratePathNodes();

        await ValidatePathNodes();
    }

    public async Task GeneratePathNodes()
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

    public async Task ValidatePathNodes()
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

    [ContextMenu("Clear Nodes")]
    private void ClearNodes()
    {
        IterateNodes(node => DestroyImmediate(node.gameObject));
        Nodes = null;
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
