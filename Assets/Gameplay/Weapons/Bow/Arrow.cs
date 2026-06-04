using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Arrow : MonoBehaviour, IProjectile
{
    [SerializeField] private float speed = 10f;

    private int damage;
    private ElementType element = ElementType.Physical;

    private Rigidbody2D rb;

    public int Damage => damage;

    // Convert enum to string for compatibility with your existing damage system
    public string Element => element.ToString().ToLower();

    public void SetDamage(int damageAmount)
    {
        damage = damageAmount;
    }

    public void SetElement(ElementType newElement)
    {
        element = newElement;
    }

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

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHit enemy = collision.GetComponent<EnemyHit>();

        if (enemy != null)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);

        if (screenPos.x < 0 || screenPos.x > 1 ||
            screenPos.y < 0 || screenPos.y > 1)
        {
            Destroy(gameObject);
        }
    }
}