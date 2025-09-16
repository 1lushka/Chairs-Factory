using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float amount);
    bool IsAlive { get; }
    Transform Transform { get; }
}
