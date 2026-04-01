using UnityEngine;

public class ChasingEnemyController : EnemyController
{
    [SerializeField] private float moveSpeed = 2f;       // Maximum movement speed
    [SerializeField] private float stoppingDistance = 0.5f; // How close to the player it stops
    
    private Transform playerTransform;
    
    void Start()
    {
        // Find the player in the scene (assumes tag "Player")
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Player not found in scene. Make sure it has the 'Player' tag.");
        }
    }
    
    void Update()
    {
        if (playerTransform == null) return;
        
        // Calculate direction to player
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        
        // Calculate distance to player
        float distance = Vector2.Distance(transform.position, playerTransform.position);
        
        // Only move if outside stopping distance
        if (distance > stoppingDistance)
        {
            // Move towards player, limited by moveSpeed
            Vector2 move = direction * moveSpeed * Time.deltaTime;
            
            // Optional: Clamp to max speed (not strictly necessary here)
            if (move.magnitude > moveSpeed * Time.deltaTime)
                move = move.normalized * moveSpeed * Time.deltaTime;
            
            transform.Translate(move);
        }
    }
}
