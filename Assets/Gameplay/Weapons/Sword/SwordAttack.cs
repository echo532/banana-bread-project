using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PolygonCollider2D), typeof(SpriteRenderer))]
public class SwordAttack : MonoBehaviour, IWeapon
{
    [Tooltip("Duration of sword swing in seconds")]
    public float swingDuration = 0.5f;

    [Tooltip("Total swing arc in degrees")]
    public float swingAngle = 120f;

    private SpriteRenderer sprite;
    private PolygonCollider2D col;

    private bool isSwinging = false;
    private float timer = 0f;

    private float startAngle, endAngle;

    private int damage = 5;

    public int Damage
    {
        get => damage;
        set => damage = value;
    }


    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        col = GetComponent<PolygonCollider2D>();

        // Start with sword invisible and collider disabled
        sprite.enabled = false;
        col.enabled = false;
    }

    void Update()
    {
        if (isSwinging)
        {
            timer += Time.deltaTime;
            float progress = timer / swingDuration;

            float angle = Mathf.Lerp(startAngle, endAngle, progress);
            transform.localRotation = Quaternion.Euler(0f, 0f, angle);

            if (timer >= swingDuration)
            {
                EndSwing();
            }
        }
    }

    public void Attack()
    {
        if (isSwinging) return;

        // Setup swing
        sprite.enabled = true;
        col.enabled = true;

        isSwinging = true;
        timer = 0f;

        startAngle = -45f;
        endAngle = -135f;
        transform.localRotation = Quaternion.Euler(0f, 0f, startAngle);
    }

    private void EndSwing()
    {
        isSwinging = false;
        sprite.enabled = false;
        col.enabled = false;
    }
}
