using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TeleportAbility : Ability
{
    private PlayerController playerController;
    [SerializeField] private float distance = 4f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Transform visual;
    [SerializeField] private GameObject poofEffect;
    private Rigidbody2D rb;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();

        if (visual == null)
        {
            PlayerController pc = GetComponent<PlayerController>();
            if (pc != null)
                visual = pc.visual;
        }
    }

    protected override void Activate()
    {
        Vector2 direction;
        if (playerController == null) return;

        Vector2 input = playerController.Movement;

        if (input != Vector2.zero)
    {
        direction = input.normalized;
    }
        else
    {
        float facing = visual.localScale.x > 0 ? 1f : -1f;
        direction = new Vector2(facing, 0);
    }

        Vector2 start = rb.position;

        // 🔥 Spawn poof at start
        if (poofEffect != null)
            Instantiate(poofEffect, start, Quaternion.identity);

        RaycastHit2D hit = Physics2D.Raycast(start, direction, distance, obstacleLayer);

        Vector2 target = (hit.collider != null)
            ? hit.point - direction * 0.5f
            : start + direction * distance;

        rb.linearVelocity = Vector2.zero;
        rb.position = target;

        // 🔥 Spawn poof at destination
        if (poofEffect != null)
            Instantiate(poofEffect, target, Quaternion.identity);
    }
}