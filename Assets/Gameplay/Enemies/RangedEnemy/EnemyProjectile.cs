using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyProjectile : MonoBehaviour, IProjectile
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 5;

    private Rigidbody2D rb;

    public int Damage => damage;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
    }

    public void SetDamage(int damageAmount)
    {
        damage = damageAmount;
    }

    public void Shoot(Vector2 direction)
    {
        direction.Normalize();
        rb.linearVelocity = direction * speed;

        // Rotate to face movement direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerCollisionHandler player = collision.GetComponentInParent<PlayerCollisionHandler>();

         Debug.Log("Projectile hit object: " + collision.name);

        // Print all components on the object we hit
        Component[] comps = collision.GetComponents<Component>();
        foreach (var comp in comps)
        {
            Debug.Log("Component on hit object: " + comp.GetType().Name);
        }

        // Also print the parent objects in case the component is higher up
        Transform current = collision.transform.parent;
        while (current != null)
        {
            Debug.Log("Parent object: " + current.name);
            Component[] parentComps = current.GetComponents<Component>();
            foreach (var comp in parentComps)
            {
                Debug.Log("Component on parent: " + comp.GetType().Name);
            }
            current = current.parent;
        }
        if (player != null)
        {
            Destroy(this.gameObject); // destroy projectile on hit
        }
    }

    void Update()
    {
        // Destroy if off-screen
        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);

        if (screenPos.x < 0 || screenPos.x > 1 || screenPos.y < 0 || screenPos.y > 1)
        {
            Destroy(this.gameObject);
        }
    }
}
