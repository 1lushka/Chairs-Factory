using UnityEngine;

public abstract class Damageable : MonoBehaviour, IDamageable
{
    [SerializeField] float maxHealth;
    float currentHealth;

    public bool IsAlive => currentHealth > 0;
    public Transform Transform => transform;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float amount)
    {
        if (!IsAlive) 
            return;
        currentHealth -= amount;
        if (currentHealth <= 0) 
            Die();
    }

    protected abstract void Die();
}
