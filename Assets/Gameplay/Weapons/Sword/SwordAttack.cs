using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PolygonCollider2D), typeof(SpriteRenderer))]
public class SwordAttack : MonoBehaviour
{
    [Tooltip("Duration of sword swing in seconds")]
    public float swingDuration = 0.5f;

    [Tooltip("Total swing arc in degrees")]
    public float swingAngle = 120f;

    private SpriteRenderer sprite;
    private PolygonCollider2D col;

    private bool isSwinging = false;
    private float timer = 0f;
    

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        col = GetComponent<PolygonCollider2D>();

        // Start with sword invisible and collider disabled
        sprite.enabled = false;
        col.enabled = false;
    }

    private float startAngle => -swingAngle / 2f;
    private float endAngle => startAngle - swingAngle;

    void Update()
    {
        Vector2 attackInput = Vector2.zero;

        // --- Keyboard input ---
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.spaceKey.wasPressedThisFrame) attackInput.y = 1f; // trigger attack
        }

        // --- Gamepad input ---
        var gp = Gamepad.current;
        if (gp != null)
        {
            if (gp.buttonSouth.wasPressedThisFrame) attackInput.y = 1f; // "A" button on Xbox
        }

        // Trigger swing
        if (!isSwinging && attackInput.y > 0f)
        {
            StartSwing();
        }

        // Update swing
        if (isSwinging)
        {
            timer += Time.deltaTime;
            float progress = timer / swingDuration;

            // Rotate from -swingAngle/2 → +swingAngle/2
            float angle = Mathf.Lerp(startAngle, endAngle, progress);
            transform.localRotation = Quaternion.Euler(0, 0, angle);
            transform.localRotation = Quaternion.Euler(0f, 0f, angle);

            // End swing after duration
            if (timer >= swingDuration)
            {
                EndSwing();
            }
        }
    }

    private void StartSwing()
    {
        sprite.enabled = true;
        col.enabled = true;

        isSwinging = true;
        timer = 0f;
    }

    private void EndSwing()
    {
        isSwinging = false;
        sprite.enabled = false;
        col.enabled = false;
    }
}
