using UnityEngine;

public class Player : Damageable
{
    protected override void Die()
    {
        print("игрок погиб");
    }
}
