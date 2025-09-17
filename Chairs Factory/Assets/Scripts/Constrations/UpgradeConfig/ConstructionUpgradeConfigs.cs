using UnityEngine;

public abstract class ConstructionUpgradeConfig : ScriptableObject
{
    public abstract int LevelCount { get; }
    public abstract ConstructionLevelData GetLevel(int index);
}

[CreateAssetMenu(menuName = "Game/Wall Upgrade Config")]
public class WallUpgradeConfig : ConstructionUpgradeConfig
{
    public WallLevelData[] levels;

    public override int LevelCount => levels.Length;
    public override ConstructionLevelData GetLevel(int index) => levels[index];
}

[CreateAssetMenu(menuName = "Game/Turret Upgrade Config")]
public class TurretUpgradeConfig : ConstructionUpgradeConfig
{
    public TurretLevelData[] levels;

    public override int LevelCount => levels.Length;
    public override ConstructionLevelData GetLevel(int index) => levels[index];
}