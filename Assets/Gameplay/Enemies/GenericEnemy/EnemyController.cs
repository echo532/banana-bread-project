using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float directionChangeInterval = 2f;

    protected Vector2 moveDirection;        // now protected so child classes can read it
    private float timeSinceDirectionChange;
    private Camera mainCamera;
    private Vector2 screenBounds;

    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }

    protected virtual void Start()
    {
        mainCamera = Camera.main;
        CalculateScreenBounds();
        ChooseRandomDirection();
    }

    void Update()
    {
        // Move and handle direction changes
        HandleMovement();
    }

    /// <summary>
    /// Call this in Update() to handle movement, bouncing, and direction changes
    /// Can be called from child classes.
    /// </summary>
    protected void HandleMovement()
    {
        // Move in current direction
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // Keep within screen bounds
        ClampToScreenBounds();

        // Change direction periodically
        timeSinceDirectionChange += Time.deltaTime;
        if (timeSinceDirectionChange >= directionChangeInterval)
        {
            ChooseRandomDirection();
            timeSinceDirectionChange = 0f;
        }
    }

    /// <summary>
    /// Picks a random normalized direction
    /// </summary>
    protected void ChooseRandomDirection()
    {
        float angle = Random.Range(0f, 360f);
        moveDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
    }

    /// <summary>
    /// Calculates the orthographic screen bounds in world coordinates
    /// </summary>
    protected void CalculateScreenBounds()
    {
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
    }

    /// <summary>
    /// Keeps the enemy inside the screen bounds and bounces off edges
    /// </summary>
    protected void ClampToScreenBounds()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -screenBounds.x, screenBounds.x);
        pos.y = Mathf.Clamp(pos.y, -screenBounds.y, screenBounds.y);
        transform.position = pos;

        // Bounce off edges by reversing direction
        if (pos.x <= -screenBounds.x || pos.x >= screenBounds.x)
            moveDirection.x *= -1;
        if (pos.y <= -screenBounds.y || pos.y >= screenBounds.y)
            moveDirection.y *= -1;
    }
}
