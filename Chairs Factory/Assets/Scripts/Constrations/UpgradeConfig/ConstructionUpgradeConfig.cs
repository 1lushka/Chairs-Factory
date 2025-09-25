using UnityEngine;

public abstract class ConstructionUpgradeConfig : ScriptableObject
{
    
    public abstract int LevelCount { get; }
    public abstract ConstructionLevelData GetLevel(int index);
}



