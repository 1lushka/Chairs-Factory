using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RuntimeGridVisualizer : MonoBehaviour
{
    [Header("Настройки сетки")]
    public float gridSize = 2f;
    public Color gridColor = new Color(1, 1, 1, 0.6f);
    public float lineWidth = 0.05f;
    public int gridRadius = 2;
    public bool useCircularMask = true;
    public float circleRadiusInCells = 3f;

    public LineRenderer lineRenderer;

    void Awake()
    {
        SetupLineRenderer();
    }

    void SetupLineRenderer()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer.material == null)
        {
            Shader shader = Shader.Find("Unlit/Color");
            if (shader == null) shader = Shader.Find("Sprites/Default");

            lineRenderer.material = new Material(shader);
        }

        lineRenderer.startColor = gridColor;
        lineRenderer.endColor = gridColor;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.useWorldSpace = true;
        lineRenderer.receiveShadows = false;
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.alignment = LineAlignment.View;
    }


    public void UpdateGrid(Vector2Int centerCell)
    {
        List<Vector3> points = new List<Vector3>();

        for (int x = -gridRadius; x <= gridRadius; x++)
        {
            for (int z = -gridRadius; z <= gridRadius; z++)
            {
                Vector2Int cell = centerCell + new Vector2Int(x, z);
                if (useCircularMask)
                {
                    Vector2 offset = new Vector2(x, z);
                    if (offset.sqrMagnitude > circleRadiusInCells * circleRadiusInCells)
                    {
                        continue;
                    }
                }
                Vector3 corner = new Vector3(cell.x * gridSize, 0f, cell.y * gridSize);
                Vector3 right = corner + Vector3.right * gridSize;
                Vector3 forward = corner + Vector3.forward * gridSize;
                Vector3 farCorner = right + Vector3.forward * gridSize;

                AddLine(points, corner, right);
                AddLine(points, corner, forward);
                AddLine(points, right, farCorner);
                AddLine(points, forward, farCorner);
            }
        }

        lineRenderer.positionCount = points.Count;
        if (points.Count > 0)
        {
            lineRenderer.SetPositions(points.ToArray());
        }
    }

    void AddLine(List<Vector3> list, Vector3 a, Vector3 b)
    {
        list.Add(a);
        list.Add(b);
    }

    public void Hide()
    {
        if (lineRenderer == null) return;
        lineRenderer.positionCount = 0;
    }
}