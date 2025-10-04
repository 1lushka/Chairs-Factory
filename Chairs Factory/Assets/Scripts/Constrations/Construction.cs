using UnityEngine;
using DG.Tweening;

public abstract class Construction : Damageable
{
    [SerializeField] public string buildingName;
    [SerializeField] public Vector2Int currentSize;
    [SerializeField] public int price;

    [SerializeField] protected ConstructionUpgradeConfig upgradeConfig;
    [SerializeField] public int currentLevel = 0;


    private void Start()
    {
        ApplyLevel(upgradeConfig.GetLevel(currentLevel));
    }

    public int GetUpgradePrice()
    {
        if (currentLevel + 1 < upgradeConfig.LevelCount)
        {
            return upgradeConfig.GetLevel(currentLevel + 1).upgradePrice;
        }
        return -1;
    }

    public virtual void Upgrade()
    {
        if (currentLevel + 1 < upgradeConfig.LevelCount)
        {
            currentLevel++;
            ApplyLevel(upgradeConfig.GetLevel(currentLevel));

            Vector3 originalScale = transform.localScale;
            Vector3 originalPos = transform.position;

            Sequence seq = DOTween.Sequence();

            seq.Append(transform.DOScale(originalScale * 1.15f, 0.25f).SetEase(Ease.OutBack));
            seq.Join(transform.DOMoveY(originalPos.y + 0.5f, 0.25f).SetEase(Ease.OutQuad));

            seq.Append(transform.DOScale(originalScale, 0.2f).SetEase(Ease.InOutSine));
            seq.Join(transform.DOMoveY(originalPos.y, 0.2f).SetEase(Ease.InOutSine));
        }
    }

    protected virtual void ApplyLevel(ConstructionLevelData data)
    {
        maxHealth = data.maxHealth;
        currentHealth = maxHealth;
    }

    public virtual void SetLevel(int level)
    {
        print(level);
        currentLevel = level;
        ApplyLevel(upgradeConfig.GetLevel(currentLevel));
    }

    public virtual void SetHealth(float health)
    {
        currentHealth = health;
    }
    protected override void Die()
    {
        Destroy(gameObject);
    }

    
}
