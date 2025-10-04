using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : Damageable
{
    [SerializeField] float attackRange;
    [SerializeField] float attackDamage;
    [SerializeField] float attackCooldown;

    [Header("Экономика")]
    [SerializeField] int reward = 10;
    public int Reward => reward;

    public event Action<Enemy> OnDeath;

    NavMeshAgent agent;
    IDamageable attackTarget;
    IDamageable player;
    Coroutine attackCoroutine;

    protected override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        player = FindFirstObjectByType<Player>();
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
                if (attackCoroutine == null)
                    attackCoroutine = StartCoroutine(AttackRoutine());
            }
            else
            {
                StopAttackCoroutine();
                agent.isStopped = false;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision == null) return;
        print("готрфшгигшни");
        Player player = collision.gameObject.GetComponentInParent<Player>();
        if (player != null) 
        {
            player.TakeDamage(player.currentHealth);
        }
    }
    void DetectAttackTarget()
    {
        Collider[] objs = Physics.OverlapSphere(transform.position, attackRange);
        attackTarget = null;

        foreach (var obj in objs)
        {
            IDamageable dmg = obj.GetComponentInParent<IDamageable>();
            if (dmg != null && dmg.IsAlive && !(dmg is Enemy) && !(dmg is Player))
            {
                attackTarget = dmg;
                break;
            }
        }
    }

    IEnumerator AttackRoutine()
    {
        while (attackTarget != null && attackTarget.IsAlive &&
               Vector3.Distance(transform.position, attackTarget.Transform.position) <= attackRange)
        {
            attackTarget.TakeDamage(attackDamage);
            yield return new WaitForSeconds(attackCooldown);
        }

        attackCoroutine = null;
        agent.isStopped = false;
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
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
