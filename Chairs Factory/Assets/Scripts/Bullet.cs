using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float damage;
    private float speed;
    private Rigidbody rb;
    [SerializeField] private ParticleSystem hitParticle;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponentInParent<Enemy>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);

            if (hitParticle != null)
            {
                Instantiate(hitParticle, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }
}
