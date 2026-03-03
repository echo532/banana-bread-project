using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    [SerializeField] private int damagePerHit = 10;
    [SerializeField] private float damageCooldown = 1f; // Prevent taking damage too rapidly
    
    private HealthSystem healthSystem;
    private float lastDamageTime = -999f;
    
    void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
        if (healthSystem == null)
        {
            Debug.LogError("PlayerCollisionHandler requires HealthSystem component!");
        }
    }
    
    void OnTriggerStay2D(Collider2D other)
    {
        // Check if we hit an enemy
        if (other.gameObject.name.StartsWith("Enemy"))
        {
            // Check cooldown to prevent rapid damage
            if (Time.time - lastDamageTime >= damageCooldown)
            {
                if (healthSystem != null)
                {
                    healthSystem.TakeDamage(damagePerHit);
                    lastDamageTime = Time.time;
                    Debug.Log($"Player hit enemy! Health reduced. Damage: {damagePerHit}");
                }
            }
        }
    }
}