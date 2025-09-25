using UnityEngine;

[System.Serializable]
public class ConstructionLevelData
{
    public int upgradePrice;
    public int maxHealth;
}

[System.Serializable]
public class WallLevelData : ConstructionLevelData
{
    
}

[System.Serializable]
public class TurretLevelData : ConstructionLevelData
{
    public float attackDamage;
    public float attackCooldown;
    public float bulletSpeed;
}