using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifetime = 3f;
    float damage;

    public void Init(float damage, float speed)
    {
        Destroy(gameObject, lifetime);
        GetComponent<Rigidbody>().linearVelocity = transform.forward * speed;
        this.damage = damage;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) return;
        if (other.GetComponent<Projectile>() != null) return;
        Health health = other.GetComponent<Health>();
        if (health != null) health.TakeDamage(damage);
        Destroy(gameObject);   
    }

}
