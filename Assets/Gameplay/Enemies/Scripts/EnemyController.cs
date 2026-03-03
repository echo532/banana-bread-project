using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float directionChangeInterval = 2f;
    
    private Vector2 moveDirection;
    private float timeSinceDirectionChange;
    private Camera mainCamera;
    private Vector2 screenBounds;
    
    void Start()
    {
        mainCamera = Camera.main;
        CalculateScreenBounds();
        ChooseRandomDirection();
    }
    
    void Update()
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
    
    void ChooseRandomDirection()
    {
        float angle = Random.Range(0f, 360f);
        moveDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
    }
    
    void CalculateScreenBounds()
    {
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
    }
    
    void ClampToScreenBounds()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -screenBounds.x, screenBounds.x);
        pos.y = Mathf.Clamp(pos.y, -screenBounds.y, screenBounds.y);
        transform.position = pos;
        
        // Bounce off edges by reversing direction
        if (pos.x <= -screenBounds.x || pos.x >= screenBounds.x)
        {
            moveDirection.x *= -1;
        }
        if (pos.y <= -screenBounds.y || pos.y >= screenBounds.y)
        {
            moveDirection.y *= -1;
        }
    }
}