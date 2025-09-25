using UnityEngine;

[CreateAssetMenu(menuName = "Construction/Wall Upgrade Config")]
public class WallUpgradeConfig : ConstructionUpgradeConfig
{
    public WallLevelData[] levels;

    public override int LevelCount => levels.Length;
    public override ConstructionLevelData GetLevel(int index) => levels[index];
}