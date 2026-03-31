using UnityEngine;

public class EnemyHealthbar : MonoBehaviour
{
    [SerializeField] private Transform fill; // the green bar or fill object

    public void SetHealth(float fraction)
    {
        fraction = Mathf.Clamp01(fraction); // ensure 0–1
        fill.localScale = new Vector3(fraction, 1f, 1f);
    }

    public void UpdateHealthBar(float currentHealth,float maxHealth)
    {
        float fillAmount = (float)currentHealth / maxHealth;
        SetHealth(fillAmount);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
