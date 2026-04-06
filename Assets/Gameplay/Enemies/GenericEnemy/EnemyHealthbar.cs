using UnityEngine;

public class EnemyHealthbar : MonoBehaviour, IHealth
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

    [SerializeField] private Transform fill; // the green bar or fill object


    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        float fillAmount = (float)currentHealth / maxHealth;
        SetHealth(fillAmount);
    }

    public void SetHealth(float fraction)
    {
        fraction = Mathf.Clamp01(fraction); // ensure 0–1
        fill.localScale = new Vector3(fraction, 1f, 1f);
    }
}
