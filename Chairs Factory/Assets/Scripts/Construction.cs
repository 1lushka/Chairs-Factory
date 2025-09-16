using UnityEngine;
using UnityEngine.AI;

public class Construction : Damageable
{
    

    protected override void Die()
    {
        Destroy(gameObject);
    }
}
