using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BuildingPlacer : MonoBehaviour
{
    public float gridSize = 2f;
    public Color validColor = Color.green;
    public Color invalidColor = Color.red;
    public bool showRuntimeGrid = true;
    public RuntimeGridVisualizer runtimeGrid;
    public BuildingManager buildingManager;

    private GameObject ghost;
    private HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();
    private int rot;
    private Vector3 ghostOriginalScale;

    void Start()
    {
        CreateGhost();
    }

    void Update()
    {
        HandleScroll();
        UpdateGhostPosition();

        if (showRuntimeGrid && runtimeGrid != null && ghost != null)
            runtimeGrid.UpdateGrid(GetCell().GetValueOrDefault());
        else if (runtimeGrid != null)
            runtimeGrid.Hide();

        if (Input.GetKeyDown(KeyCode.Mouse0)) Place();
        if (Input.GetKeyDown(KeyCode.R)) RotateBuilding();
    }

    void HandleScroll()
    {
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0 && buildingManager != null)
        {
            int direction = scroll > 0 ? -1 : 1;
            buildingManager.NextBuilding(direction);
            CreateGhost();
        }
    }

    void CreateGhost()
    {
        if (ghost != null)
            Destroy(ghost);

        var data = buildingManager?.GetSelectedBuildingData();
        if (data == null || data.ghostPrefab == null) return;

        ghost = Instantiate(data.ghostPrefab);
        ghost.name = "Ghost";
        ghost.transform.rotation = Quaternion.identity;
        rot = 0;

        Renderer renderer = ghost.GetComponent<Renderer>();
        if (renderer != null && renderer.sharedMaterial != null)
        {
            renderer.material = new Material(renderer.sharedMaterial);
            renderer.material.color = validColor;
        }

        ghostOriginalScale = ghost.transform.localScale;

        ghost.transform.localScale = Vector3.zero;
        ghost.transform.DOScale(ghostOriginalScale, 0.3f)
            .SetEase(Ease.OutBack);
    }

    void UpdateGhostPosition()
    {
        if (ghost == null || buildingManager == null) return;

        Vector2Int? maybeCell = GetCell();
        if (!maybeCell.HasValue) return;

        Vector2Int baseCell = maybeCell.Value;
        Vector2Int size = buildingManager.GetSize(buildingManager.GetSelectedBuildingData().buildingPrefab);

        Vector3 pos = GetCenter(baseCell, size, rot);
        Renderer rend = ghost.GetComponent<Renderer>();
        float yOffset = rend != null ? rend.bounds.extents.y : 0f;

        ghost.transform.position = new Vector3(pos.x, yOffset, pos.z);
        ghost.transform.rotation = Quaternion.Euler(0f, 90f * rot, 0f);

        Vector2Int[] cells = GetCells(baseCell, size, rot);
        bool canPlace = true;
        foreach (var cell in cells)
            if (occupied.Contains(cell)) canPlace = false;

        if (rend != null && rend.material != null)
            rend.material.color = canPlace ? validColor : invalidColor;
    }

    void RotateBuilding()
    {
        rot = (rot + 1) % 4;
        if (ghost != null)
        {
            ghost.transform.DOKill();
            ghost.transform.DOScale(ghostOriginalScale * 1.1f, 0.1f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    ghost.transform.DOScale(ghostOriginalScale, 0.1f).SetEase(Ease.InOutSine);
                });
            ghost.transform.rotation = Quaternion.Euler(0f, 90f * rot, 0f);
        }
    }

    void Place()
    {
        if (ghost == null || buildingManager == null) return;

        Vector2Int? maybeCell = GetCell();
        if (!maybeCell.HasValue) return;

        Vector2Int baseCell = maybeCell.Value;
        var data = buildingManager.GetSelectedBuildingData();
        Vector2Int size = buildingManager.GetSize(data.buildingPrefab);

        Vector2Int[] cells = GetCells(baseCell, size, rot);
        foreach (var c in cells)
            if (occupied.Contains(c)) return;

        GameManager gameManager = FindAnyObjectByType<GameManager>();
        if (!gameManager.SpendMoney(buildingManager.GetPrice(data.buildingPrefab))) return;

        Vector3 pos = GetCenter(baseCell, size, rot);
        Renderer ghostRend = ghost.GetComponent<Renderer>();
        float yOffset = ghostRend != null ? ghostRend.bounds.extents.y : 0f;

        GameObject newBuilding = Instantiate(
            data.buildingPrefab,
            new Vector3(pos.x, yOffset, pos.z),
            ghost.transform.rotation
        );

        Vector3 originalScale = newBuilding.transform.localScale;
        newBuilding.transform.localScale = originalScale * 0.8f;
        newBuilding.transform.DOScale(originalScale * 1.1f, 0.2f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                newBuilding.transform.DOScale(originalScale, 0.15f)
                    .SetEase(Ease.InOutSine);
            });

        foreach (var c in cells)
            occupied.Add(c);
    }

    Vector2Int? GetCell()
    {
        if (Camera.main == null) return null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        if (plane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            int x = Mathf.RoundToInt(hitPoint.x / gridSize);
            int z = Mathf.RoundToInt(hitPoint.z / gridSize);
            return new Vector2Int(x, z);
        }
        return null;
    }

    Vector3 GetCenter(Vector2Int baseCell, Vector2Int size, int rotation)
    {
        Vector2Int[] cells = GetCells(baseCell, size, rotation);

        int minX = int.MaxValue, minZ = int.MaxValue;
        int maxX = int.MinValue, maxZ = int.MinValue;

        foreach (var c in cells)
        {
            minX = Mathf.Min(minX, c.x);
            minZ = Mathf.Min(minZ, c.y);
            maxX = Mathf.Max(maxX, c.x);
            maxZ = Mathf.Max(maxZ, c.y);
        }

        float cx = (minX + maxX + 1f) * 0.5f * gridSize;
        float cz = (minZ + maxZ + 1f) * 0.5f * gridSize;

        return new Vector3(cx, 0f, cz);
    }

    Vector2Int[] GetCells(Vector2Int baseCell, Vector2Int size, int rotation)
    {
        var list = new List<Vector2Int>();

        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.y; z++)
            {
                Vector2Int p = new Vector2Int(x, z);
                for (int i = 0; i < rotation % 4; i++)
                    p = new Vector2Int(-p.y, p.x);
                list.Add(baseCell + p);
            }
        }
        return list.ToArray();
    }

    void OnDisable()
    {
        if (ghost != null)
        {
            ghost.transform.DOScale(Vector3.zero, 0.2f)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    if (ghost != null)
                    {
                        if (Application.isPlaying)
                            Destroy(ghost);
                        else
                            DestroyImmediate(ghost);
                    }
                    ghost = null;
                });
        }

        if (runtimeGrid != null)
            runtimeGrid.Hide();
    }

    private void OnEnable()
    {
        CreateGhost();
    }
}
