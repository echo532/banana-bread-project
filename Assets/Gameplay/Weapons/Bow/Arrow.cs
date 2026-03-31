using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Arrow : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
    }

    public void Shoot(Vector2 direction)
    {
        direction.Normalize();
        rb.linearVelocity = direction * speed;

        // Rotate arrow to face direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHit enemy = collision.GetComponent<EnemyHit>();
        if (enemy != null)
        {
            Destroy(gameObject); // destroy arrow
        }

        
    }

    void Update()
    {
        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);
        // Viewport: x,y in 0–1, z > 0 is in front of camera
        if (screenPos.x < 0 || screenPos.x > 1 || screenPos.y < 0 || screenPos.y > 1)
        {
            Destroy(gameObject);
        }
    }
}
