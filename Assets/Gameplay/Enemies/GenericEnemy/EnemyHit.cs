using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyHit : MonoBehaviour
{
    [SerializeField] private int maxHealth = 10;
    public int currentHealth;

    public EnemyHealthbar healthBar;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider belongs to the sword
        if (other.CompareTag("Weapon"))
        {
            currentHealth-= 5;
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Optional: play death animation, effects, sound, etc.
        Destroy(gameObject);
    }
}
