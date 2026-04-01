using UnityEngine;

public class RangedEnemyController : EnemyController
{
    [SerializeField] private float fireCooldown = 1.5f;      // Time between shots
    [SerializeField] private GameObject arrowPrefab;         // Assign in Inspector

    private Transform playerTransform;
    private float fireTimer;

    protected override void Start()
    {
        base.Start();
        MoveSpeed = 2f; // uses parent's property

         GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
        else
            Debug.LogWarning("Player not found!");

        }

    void Update()
    {
        HandleMovement();

        fireTimer += Time.deltaTime;

        // Parent handles movement and screen clamping automatically
        // EnemyController already moves using moveDirection and changes it periodically
        

        // Shoot at player if cooldown elapsed
        if (fireTimer >= fireCooldown)
        {
            FireAtPlayer();
            fireTimer = 0f;
        }
    }

    private void FireAtPlayer()
    {
        if (arrowPrefab == null || playerTransform == null) return;

        Vector2 direction = (playerTransform.position - transform.position).normalized;

        // Instantiate arrow
        GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);

        // Rotate arrow to face direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Shoot via EnemyProjectile
        EnemyProjectile projectile = arrow.GetComponent<EnemyProjectile>();
        if (projectile != null)
            projectile.Shoot(direction);
    }
}
