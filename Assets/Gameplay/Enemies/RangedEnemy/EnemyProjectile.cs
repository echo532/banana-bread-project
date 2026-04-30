using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyProjectile : MonoBehaviour, IProjectile
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 5;

    private Rigidbody2D rb;

    public int Damage => damage;

    public string Element => "ice"; 

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
        PlayerController player = collision.GetComponent<PlayerController>();
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
