using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class BuildingData
{
    public GameObject buildingPrefab;
    public GameObject ghostPrefab;
}

public enum ToolMode
{
    Build,
    Upgrade
}

public class BuildingManager : MonoBehaviour
{
    [Header("Постройки")]
    public List<BuildingData> buildings;

    [Header("UI для построек")]
    public TextMeshProUGUI buildingNameText;
    public TextMeshProUGUI buildingPriceText;

    [Header("UI для улучшений")]
    public TextMeshProUGUI upgradeNameText;
    public TextMeshProUGUI upgradePriceText;

    private int selIdx;
    private Construction hoveredConstruction;

    [Header("Текущий инструмент")]
    public ToolMode currentMode = ToolMode.Build;

    private BuildingPlacer buildingPlacer;

    private void Start()
    {
        buildingPlacer = FindAnyObjectByType<BuildingPlacer>();
        UpdateUI();
    }

    private void Update()
    {
        HandleToolSwitch();

        if (currentMode == ToolMode.Upgrade)
        {
            HandleUpgradeHover();

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                TryUpgrade();
            }
        }
        else if (currentMode == ToolMode.Build)
        {
            UpdateUI();
        }
    }

    void HandleToolSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentMode = ToolMode.Build;
            buildingPlacer.enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentMode = ToolMode.Upgrade;
            buildingPlacer.enabled = false;
        }
    }

    public BuildingData GetSelectedBuildingData()
    {
        if (selIdx < 0 || selIdx >= buildings.Count) return null;
        return buildings[selIdx];
    }

    public void NextBuilding(int direction)
    {
        selIdx = Mathf.Clamp(selIdx + direction, 0, buildings.Count - 1);
        UpdateUI();
    }

    public Vector2Int GetSize(GameObject prefab)
    {
        if (prefab == null) return Vector2Int.one;
        Construction c = prefab.GetComponent<Construction>();
        return c != null ? c.currentSize : Vector2Int.one;
    }

    public int GetPrice(GameObject prefab)
    {
        if (prefab == null) return 0;
        Construction c = prefab.GetComponent<Construction>();
        return c != null ? c.price : 0;
    }

    public void UpdateUI()
    {
        if (buildingNameText == null || buildingPriceText == null)
            return;

        var data = GetSelectedBuildingData();
        if (data == null)
        {
            buildingNameText.text = "";
            buildingPriceText.text = "";
            return;
        }

        Construction c = data.buildingPrefab.GetComponent<Construction>();
        if (c != null)
        {
            buildingNameText.text = c.buildingName;
            buildingPriceText.text = $"Цена: {c.price}";
        }
        else
        {
            buildingNameText.text = "";
            buildingPriceText.text = "";
        }
    }

    void HandleUpgradeHover()
    {
        hoveredConstruction = null;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Construction c = hit.collider.GetComponent<Construction>();
            if (c != null)
            {
                hoveredConstruction = c;

                if (upgradeNameText != null)
                    upgradeNameText.text = c.buildingName;

                if (upgradePriceText != null)
                {
                    int upgradePrice = c.GetUpgradePrice();
                    if (upgradePrice == -1)
                    {
                        upgradePriceText.text = "Улучшить нельзя";
                        return;
                    }
                    upgradePriceText.text = $"Улучшение: {c.GetUpgradePrice()}";
                }

                return;
            }
        }
        if (upgradeNameText != null)
            upgradeNameText.text = "";

        if (upgradePriceText != null)
            upgradePriceText.text = "";
    }

    public void TryUpgrade()
    {
        if (hoveredConstruction == null) return;

        GameManager gameManager = FindAnyObjectByType<GameManager>();
        if(hoveredConstruction.GetUpgradePrice() == -1) return;
        if (!gameManager.SpendMoney(hoveredConstruction.GetUpgradePrice())) return;

        hoveredConstruction.Upgrade();
    }

}
