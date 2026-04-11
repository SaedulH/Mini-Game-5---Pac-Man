using CoreSystem;
using UnityEngine;
using Utilities;

public class MazeGenerator : MonoBehaviour
{
    public int width = 26;
    public int height = 29;
    public float nodeDistance = 1f;
    public Vector3 startPosition = new(-13.5f, 2.5f, -14.5f);

    public GameObject nodePrefab;
    public GameObject[,] nodes;

    [ContextMenu("Generate Nodes")]
    private void GenerateNodes()
    {
        if (nodePrefab == null)
        {
            Debug.LogError("Nodes prefab is not assigned.");
            return;
        }
        nodes = new GameObject[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector3 position = new(
                    startPosition.x + i * nodeDistance,
                    startPosition.y,
                    startPosition.z + j * nodeDistance
                );

                GameObject nodeObj = Instantiate(nodePrefab, position, Quaternion.identity, transform);
                nodes[i, j] = nodeObj;
                nodeObj.name = $"Node_{i}_{j}";
            }
        }
    }

    [ContextMenu("Clear Nodes")]
    private void ClearNodes()
    {
        if (nodes == null)
        {
            Debug.LogWarning("Nodes array is not initialized.");
            NodeScript[] nodeObjs = gameObject.GetComponentsInChildren<NodeScript>();
            foreach (NodeScript nodeObj in nodeObjs)
            {
                DestroyImmediate(nodeObj.gameObject);
            }
            nodes = new GameObject[width, height];
            return;
        }
        for (int i = 0; i < nodes.GetLength(0); i++)
        {
            for (int j = 0; j < nodes.GetLength(1); j++)
            {
                if (nodes[i, j] != null)
                {
                    DestroyImmediate(nodes[i, j]);
                    nodes[i, j] = null;
                }
            }
        }
    }

    [ContextMenu("Validate Nodes")]
    private void ValidateNodes()
    {
        if (nodes == null)
        {
            Debug.LogWarning("Nodes array is not initialized.");
            return;
        }
        for (int i = 0; i < nodes.GetLength(0); i++)
        {
            for (int j = 0; j < nodes.GetLength(1); j++)
            {
                if (nodes[i, j] == null)
                {
                    Debug.LogWarning($"Node at position ({i}, {j}) is missing.");
                    return;
                }

                NodeScript nodeScript = nodes[i, j].GetOrAdd<NodeScript>();
                if (!nodeScript.ValidateNodePosition())
                {
                    Debug.LogWarning($"Node at position ({i}, {j}) has invalid position.");
                    nodeScript.gameObject.SetActive(false);
                    return;
                }
                nodeScript.CheckAvailableMoves(nodeDistance);
            }
        }
    }
}
