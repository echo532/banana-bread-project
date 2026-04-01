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
        IWeapon weapon = other.GetComponent<IWeapon>();
        IProjectile projectile = other.GetComponent<IProjectile>();

        // Check if we hit an enemy
        if (weapon != null && other.CompareTag("Enemy"))
        {
            HandleDamage(weapon.Damage);
        }
        else if (projectile != null && other.CompareTag("Enemy"))
        {
            HandleDamage(projectile.Damage);
        }

        // Check cooldown to prevent rapid damage
        if (Time.time - lastDamageTime >= damageCooldown)
        {
            if (healthSystem != null)
            {
                healthSystem.TakeDamage(weapon.Damage);
                lastDamageTime = Time.time;
                Debug.Log($"Player hit enemy! Health reduced. Damage: {damagePerHit}");
            }
        }
    }

    private void HandleDamage(int dmg)
    {
        if (Time.time - lastDamageTime >= damageCooldown)
        {
            if (healthSystem != null)
            {
                healthSystem.TakeDamage(dmg);
                lastDamageTime = Time.time;
                Debug.Log($"Player hit enemy! Health reduced. Damage: {damagePerHit}");
            }
        }
    }
}