using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEdgeRenderer : MonoBehaviour
{
    public Color edgeColor = Color.black;
    public float edgeThickness = 0.02f;

    private GameObject[] edges;
    private static Material sharedLineMaterial;
    private Vector3 lastPosition;
    private Quaternion lastRotation;

    void Awake()
    {
        // Use shared material for performance
        if (sharedLineMaterial == null)
        {
            sharedLineMaterial = new Material(Shader.Find("Sprites/Default"));
        }
    }

    void Start()
    {
        CreateEdges();
    }

    void Update()
    {

    }
    void LateUpdate()
    {
        if (transform.position != lastPosition || transform.rotation != lastRotation)
        {
            UpdateEdges();
            lastPosition = transform.position;
            lastRotation = transform.rotation;
        }
    }

    void CreateEdges()
    {
        if (edges != null)
        {
            foreach (var edge in edges)
            {
                if (edge != null) Destroy(edge);
            }
        }

        edges = new GameObject[12];

        // Local corner positions of a unit cube centered at origin
        Vector3[] corners = new Vector3[]
        {
            new(-0.5f, -0.5f, -0.5f), new(0.5f, -0.5f, -0.5f),
            new(0.5f, 0.5f, -0.5f), new(-0.5f, 0.5f, -0.5f),
            new(-0.5f, -0.5f, 0.5f), new(0.5f, -0.5f, 0.5f),
            new(0.5f, 0.5f, 0.5f), new(-0.5f, 0.5f, 0.5f)
        };

        // Edge index pairs from the above corners
        int[,] edgeIndices = new int[,]
        {
            {0, 1}, {1, 2}, {2, 3}, {3, 0}, // Back face
            {4, 5}, {5, 6}, {6, 7}, {7, 4}, // Front face
            {0, 4}, {1, 5}, {2, 6}, {3, 7}  // Side edges
        };

        for (int i = 0; i < 12; i++)
        {
            Vector3 start = corners[edgeIndices[i, 0]];
            Vector3 end = corners[edgeIndices[i, 1]];

            CreateEdge(i, start, end);
        }
    }

    void CreateEdge(int index, Vector3 localStart, Vector3 localEnd)
    {
        GameObject edgeObj = new GameObject("Edge_" + index);
        edgeObj.transform.SetParent(transform, false);

        LineRenderer line = edgeObj.AddComponent<LineRenderer>();
        line.useWorldSpace = true; // So it stays consistent in world space

        Vector3 worldStart = transform.TransformPoint(localStart);
        Vector3 worldEnd = transform.TransformPoint(localEnd);

        line.positionCount = 2;
        line.SetPosition(0, worldStart);
        line.SetPosition(1, worldEnd);

        line.widthMultiplier = edgeThickness;
        line.material = sharedLineMaterial;
        line.startColor = edgeColor;
        line.endColor = edgeColor;

        // Optional smoothing for corners
        line.numCapVertices = 2;
        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        line.receiveShadows = false;

        edges[index] = edgeObj;
    }

    void UpdateEdges()
    {
        if (edges == null) return;

        Vector3[] corners = new Vector3[]
        {
        new(-0.5f, -0.5f, -0.5f), new(0.5f, -0.5f, -0.5f),
        new(0.5f, 0.5f, -0.5f), new(-0.5f, 0.5f, -0.5f),
        new(-0.5f, -0.5f, 0.5f), new(0.5f, -0.5f, 0.5f),
        new(0.5f, 0.5f, 0.5f), new(-0.5f, 0.5f, 0.5f)
        };

        int[,] edgeIndices = new int[,]
        {
        {0, 1}, {1, 2}, {2, 3}, {3, 0},
        {4, 5}, {5, 6}, {6, 7}, {7, 4},
        {0, 4}, {1, 5}, {2, 6}, {3, 7}
        };

        for (int i = 0; i < 12; i++)
        {
            Vector3 start = transform.TransformPoint(corners[edgeIndices[i, 0]]);
            Vector3 end = transform.TransformPoint(corners[edgeIndices[i, 1]]);
            LineRenderer line = edges[i].GetComponent<LineRenderer>();
            line.SetPosition(0, start);
            line.SetPosition(1, end);
        }
    }

}
