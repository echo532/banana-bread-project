using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private Image healthBarFill;
    
    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }
    
    public void TakeDamage(int damage)
    {
        int oldHealth = currentHealth;
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth); // Don't go below 0
        
        Debug.Log($"Health changed: {oldHealth} -> {currentHealth} (max: {maxHealth})");
        
        UpdateHealthBar();
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            float fillAmount = (float)currentHealth / maxHealth;
            healthBarFill.fillAmount = fillAmount;
            Debug.Log($"Health bar updated: fillAmount = {fillAmount} (health: {currentHealth}/{maxHealth})");
        }
        else
        {
            Debug.LogWarning("Health bar fill image is null! Cannot update UI.");
        }
    }
    
    void Die()
    {
        Debug.Log("Player died!");
        // You can add game over logic here
    }
    
    public void SetHealthBarUI(Image fillImage)
    {
        healthBarFill = fillImage;
        UpdateHealthBar();
    }
}