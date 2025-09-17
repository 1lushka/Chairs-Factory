using UnityEngine;

public abstract class Construction : Damageable
{
    [SerializeField] protected ConstructionUpgradeConfig upgradeConfig;
    [SerializeField] protected int currentLevel = 0;

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
    }

    protected override void Die()
    {
        Destroy(gameObject);
    }

    protected virtual void Start()
    {
        ApplyLevel(upgradeConfig.GetLevel(currentLevel));
    }
}
