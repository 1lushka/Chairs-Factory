using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : Damageable
{
    [SerializeField] float attackRange;
    [SerializeField] float attackDamage;
    [SerializeField] float attackCooldown;
    
    NavMeshAgent agent;
    IDamageable attackTarget;
    IDamageable player;
    Coroutine attackCoroutine;

    override protected void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        player  = FindFirstObjectByType<Player>();
    }

    void Update()
    {
        if (!IsAlive) return;

        DetectAttackTarget();

        if (attackTarget != null && attackTarget.IsAlive)
        {
            float dist = Vector3.Distance(transform.position, attackTarget.Transform.position);
            if (dist <= attackRange)
            {
                agent.isStopped = true;
                if (attackCoroutine == null) attackCoroutine = StartCoroutine(AttackRoutine());
            }
            else
            {
                //agent.isStopped = false;
                //agent.SetDestination(attackTarget.Transform.position);
                //StopAttackCoroutine();
            }
        }
        else
        {
            StopAttackCoroutine();
            if (player != null && player.IsAlive)
            {
                agent.isStopped = false;
                agent.SetDestination(player.Transform.position);
            }
        }
    }

    void DetectAttackTarget()
    {
        Collider[] objs = Physics.OverlapSphere(transform.position, attackRange);
        attackTarget = null;

        foreach (var obj in objs)
        {
            IDamageable dmg = obj.GetComponentInParent<IDamageable>();
            if (dmg != null && dmg.IsAlive && !(dmg is Enemy))
            {
                attackTarget = dmg;
                break;
            }
        }
    }

    IEnumerator AttackRoutine()
    {
        while (attackTarget != null && attackTarget.IsAlive && Vector3.Distance(transform.position, attackTarget.Transform.position) <= attackRange)
        {
            attackTarget.TakeDamage(attackDamage);
            yield return new WaitForSeconds(attackCooldown);
        }
        attackCoroutine = null;
    }

    void StopAttackCoroutine()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }

    protected override void Die()
    {
        StopAttackCoroutine();
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
