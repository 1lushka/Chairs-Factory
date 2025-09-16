using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Turret : Construction
{
    [SerializeField] float attackDamage;
    [SerializeField] float attackCooldown;

    IDamageable attackTarget;
    Coroutine attackCoroutine;
    bool isAttacking = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isAttacking) return;

        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null && enemy.IsAlive)
        {
            attackTarget = enemy;
            isAttacking = true;
            attackCoroutine = StartCoroutine(AttackRoutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null && enemy == attackTarget)
        {
            StopAttack();
        }
    }

    IEnumerator AttackRoutine()
    {
        while (attackTarget != null && attackTarget.IsAlive && isAttacking)
        {
            attackTarget.TakeDamage(attackDamage);
            yield return new WaitForSeconds(attackCooldown);
        }
        StopAttack();
    }

    void StopAttack()
    {
        isAttacking = false;
        attackTarget = null;
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }

    private void Update()
    {
        if (isAttacking && (attackTarget == null || !attackTarget.IsAlive))
        {
            StopAttack();
        }
    }

}