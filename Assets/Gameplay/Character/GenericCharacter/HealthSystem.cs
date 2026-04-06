using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour, IHealth
{
    private int maxHealth;

    public int MaxHealth
    {
        get => maxHealth;
        set => maxHealth = value;
    }

    private int currentHealth;

    public int CurrentHealth
    {
        get => currentHealth;
        set => currentHealth = value;
    }

    
    [SerializeField] private Image healthBarFill;

    [SerializeField] private PlayerController player; // Reference to player
    
    void Start()
    {
        maxHealth = player.maxHealth;
        currentHealth = maxHealth;

        healthBarFill.fillAmount = 1;
    }
    
    public void TakeDamage(int damage)
    {

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth); // Don't go below 0

        Debug.Log("Current Health: " + currentHealth);

        
        
        float fillAmount = (float)currentHealth / maxHealth;
        healthBarFill.fillAmount = fillAmount;
        
        if (currentHealth <= 0)
        {
            Die();
        }

    }
    
    void Die()
    {
        Debug.Log("Player died!");
        // You can add game over logic here
    }

}