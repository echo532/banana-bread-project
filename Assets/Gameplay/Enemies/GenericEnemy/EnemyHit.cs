using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyHit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider belongs to the sword
        if (other.CompareTag("Sword"))
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
