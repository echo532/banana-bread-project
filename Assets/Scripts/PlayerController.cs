using UnityEngine;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Tooltip("Movement speed in units/second")]
    public float moveSpeed = 5f;
#ur gay
    Rigidbody2D rb;
    Vector2 movement;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        Vector2 move = Vector2.zero;
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed) move.y += 1f;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed) move.y -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) move.x += 1f;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) move.x -= 1f;
        }

        var gp = Gamepad.current;
        if (gp != null)
        {
            move += gp.leftStick.ReadValue();
        }

        movement = move;
#else
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        // Fallback to explicit WASD keys if axes aren't configured
        if (Mathf.Approximately(x, 0f) && Mathf.Approximately(y, 0f))
        {
            x = (Input.GetKey(KeyCode.D) ? 1f : 0f) - (Input.GetKey(KeyCode.A) ? 1f : 0f);
            y = (Input.GetKey(KeyCode.W) ? 1f : 0f) - (Input.GetKey(KeyCode.S) ? 1f : 0f);
        }

        movement = new Vector2(x, y);
#endif

        if (movement.sqrMagnitude > 1f) movement.Normalize();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement * moveSpeed;
    }
}
