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
    void SetVisible(bool visible)
    {
        if (visual != null)
            visual.gameObject.SetActive(visible);
    }
    protected override void Activate()
    {
        StartCoroutine(TeleportRoutine());
    }
    System.Collections.IEnumerator TeleportRoutine()
    {
        if (playerController == null) yield break;

        Vector2 input = playerController.Movement;

        Vector2 direction;

        // If player is moving, use movement direction
        if (input != Vector2.zero)
        {
            direction = input.normalized;
        }
        else
        {
            // fallback: use facing direction (or default right)
            float facing = playerController.visual.localScale.x > 0 ? 1f : -1f;
            direction = new Vector2(facing, 0f);
        }

        Vector2 start = rb.position;

        // 💨 START EFFECT
        if (poofEffect != null)
            Instantiate(poofEffect, start, Quaternion.identity);

        // 👻 DISAPPEAR
        SetVisible(false);

        // optional: stop movement
        rb.linearVelocity = Vector2.zero;

        // ⏱️ delay before teleport (invisible phase)
        yield return new WaitForSeconds(0.15f);

        // calculate destination
        RaycastHit2D hit = Physics2D.Raycast(start, direction, distance, obstacleLayer);

        Vector2 target = (hit.collider != null)
            ? hit.point - direction * 0.5f
            : start + direction * distance;

        // TELEPORT
        rb.position = target;

        // 💨 END EFFECT
        if (poofEffect != null)
            Instantiate(poofEffect, target, Quaternion.identity);

        // ⏱️ small delay before reappearing (feels better)
        yield return new WaitForSeconds(0.1f);

        // 👀 REAPPEAR
        SetVisible(true);
    }
}