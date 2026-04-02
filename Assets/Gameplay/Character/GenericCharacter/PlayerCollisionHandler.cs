using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    [SerializeField] private int damagePerHit = 10;
    [SerializeField] private float damageCooldown = 1f; // Prevent taking damage too rapidly
    
    private HealthSystem healthSystem;
    private float lastDamageTime = -999f;

    public GameObject DamageTextPrefab;

    private List<GameObject> activeDamageTexts = new List<GameObject>();
    
    void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
        if (healthSystem == null)
        {
            Debug.LogError("PlayerCollisionHandler requires HealthSystem component!");
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {  

        Debug.Log("PLAYER HIT");
        Debug.Log("Projectile hit object: " + other.name);
        IEnemy enemy = other.GetComponentInParent<IEnemy>();
        IWeapon weapon = other.GetComponent<IWeapon>();
        IProjectile projectile = other.GetComponent<IProjectile>();
        
        // Check if we hit an enemy weapon 
        if (weapon != null)
        {
            HandleDamage(weapon.Damage);
        } else if(enemy != null && other.CompareTag("Enemy"))
        {
            HandleDamage(enemy.Damage);
        }
        else if (projectile != null)
        {
            HandleDamage(projectile.Damage);
        }

    }

    private void HandleDamage(int dmg)
    {
        if (Time.time - lastDamageTime >= damageCooldown)
        {
            if (healthSystem != null)
            {
                healthSystem.TakeDamage(dmg);
                ShowDamageNumber(dmg, "");
                lastDamageTime = Time.time;
                Debug.Log($"Player hit enemy! Health reduced. Damage: {damagePerHit}");
            }
        }
    }

    void ShowDamageNumber(int damage, string element)
    {
        GameObject dmgText = Instantiate(DamageTextPrefab, transform.position + Vector3.up,Quaternion.identity);

        // Stack offset
        float yOffset = activeDamageTexts.Count * Random.Range(-0.02f, 0.02f); // 0.3 units above previous
        float xOffset = Random.Range(-0.5f, 0.5f); // horizontal variation
        dmgText.transform.position += new Vector3(xOffset, yOffset, 0);

        // Set damage & element
        dmgText.GetComponent<DamageText>().SetDamage(damage, element);

        // Track active number
        activeDamageTexts.Add(dmgText);

        // Remove when lifetime ends
        DamageText dt = dmgText.GetComponent<DamageText>();
        dt.OnDestroyEvent += () => activeDamageTexts.Remove(dmgText);
    }
}