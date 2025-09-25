using UnityEngine;

[CreateAssetMenu(menuName = "Construction/Turret Upgrade Config")]
public class TurretUpgradeConfig : ConstructionUpgradeConfig
{
    public TurretLevelData[] levels;

    public override int LevelCount => levels.Length;
    public override ConstructionLevelData GetLevel(int index) => levels[index];
}