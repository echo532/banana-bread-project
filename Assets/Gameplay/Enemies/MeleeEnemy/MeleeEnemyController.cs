using UnityEngine;

public class MeleeEnemyController : EnemyController, IDamageDealer
{
    [SerializeField] private float stoppingDistance = 0.5f; // How close to the player it stops
    
    private Transform playerTransform;

    private EnemyController parentComponent;

    
    protected override void Start()
    {
        parentComponent = GetComponent<EnemyController>();
        Damage = 5;
        MoveSpeed = 3f;
        // Find the player in the scene (assumes tag "Player")
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }
    
    void Update()
    {

        if (playerTransform == null) return;

        if(parentComponent.MoveSpeed == 0f) return; // skip movement if frozen
        Debug.Log(MoveSpeed);
        // Calculate direction to player
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        
        // Calculate distance to player
        float distance = Vector2.Distance(transform.position, playerTransform.position);
        
        // Only move if outside stopping distance
        if (distance > stoppingDistance)
        {
            // Move towards player, limited by moveSpeed
            Vector2 move = direction * MoveSpeed * Time.deltaTime;
            
            // Optional: Clamp to max speed (not strictly necessary here)
            if (move.magnitude > MoveSpeed * Time.deltaTime)
                move = move.normalized * MoveSpeed * Time.deltaTime;
            
            transform.Translate(move);
        }
    }
}
