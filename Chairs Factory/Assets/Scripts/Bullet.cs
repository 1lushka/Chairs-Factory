using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float damage;
    private float speed;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponentInParent<Enemy>();
        if(damageable != null)
        {
            damageable.TakeDamage(damage);
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
