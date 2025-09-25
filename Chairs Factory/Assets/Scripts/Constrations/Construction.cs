using UnityEngine;

public abstract class Construction : Damageable
{
    [SerializeField] public string buildingName;
    [SerializeField] public Vector2Int currentSize;
    [SerializeField] public int price;

    [SerializeField] protected ConstructionUpgradeConfig upgradeConfig;
    [SerializeField] protected int currentLevel = 0;
    

    //private void Awake()
    //{
    //    currentSize = upgradeConfig.GetLevel(currentLevel).gridSize;
    //}
    public int GetUpgradePrice()
    {
        if (currentLevel + 1 < upgradeConfig.LevelCount)
        {
            return upgradeConfig.GetLevel(currentLevel+1).upgradePrice;
        }
        return -1;
    }

    public virtual void Upgrade()
    {
        if (currentLevel + 1 < upgradeConfig.LevelCount)
        {
            currentLevel++;
            ApplyLevel(upgradeConfig.GetLevel(currentLevel));
        }
    }

    protected virtual void ApplyLevel(ConstructionLevelData data)
    {
        maxHealth = data.maxHealth;
        currentHealth = maxHealth;
        //currentSize = data.gridSize;
    }

    protected override void Die()
    {
        Destroy(gameObject);
    }

    
    private void Start()
    {
        ApplyLevel(upgradeConfig.GetLevel(currentLevel));
    }
}
