using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Tooltip("Movement speed in units/second")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;

    [SerializeField] public Transform visual;
    private Vector2 movement;

    private IWeapon[] weapons;

    private IWeapon currentWeapon;
    [SerializeField] private int startingWeaponIndex = 0;
    [SerializeField] private int weaponDamage = 8;

    [SerializeField] public int critChance = 50;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        weapons = GetComponentsInChildren<IWeapon>(true);
        EquipWeapon(startingWeaponIndex); // Bow

        currentWeapon.Damage = weaponDamage;
    }
    public void EquipWeapon(int index)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            (weapons[i] as MonoBehaviour).gameObject.SetActive(i == index);
        }
        currentWeapon = weapons[index];
    }

    void Update()
    {
        Vector2 move = Vector2.zero;

        // --- Keyboard input ---
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed) move.y += 1f;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed) move.y -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) move.x += 1f;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) move.x -= 1f;
        }

        // --- Gamepad input ---
        var gp = Gamepad.current;
        if (gp != null)
        {
            move += gp.leftStick.ReadValue();
        }

        // Normalize diagonal movement
        if (move.sqrMagnitude > 1f) move.Normalize();

        movement = move;

        // --- Movement-based flipping (default) ---
        if (movement.x > 0)
        {
            visual.localScale = new Vector3(1, 1, 1);
        }
        else if (movement.x < 0)
        {
            visual.localScale = new Vector3(-1, 1, 1);
        }

        // --- Attack input ---
        var mouse = Mouse.current;
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            // Determine direction relative to mouse
            Vector3 mousePos = mouse.position.ReadValue();
            Vector3 worldMouse = Camera.main.ScreenToWorldPoint(mousePos);
            float direction = Mathf.Sign(worldMouse.x - transform.position.x);

            // Temporarily flip visual to face mouse
            Vector3 originalScale = visual.localScale;
            visual.localScale = new Vector3(Mathf.Abs(visual.localScale.x) * direction, 1, 1);

            // Trigger attack
            currentWeapon.Attack();
        }



    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement * moveSpeed;
    }
}
