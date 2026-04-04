using UnityEngine;
using System.Collections.Generic;
 // assign your Canvas

[RequireComponent(typeof(Collider2D))]
public class EnemyHit : MonoBehaviour
{

    [SerializeField] public PlayerController player;
    [SerializeField] private int maxHealth = 10;
    public int currentHealth;
    public GameObject DamageTextPrefab;
    public EnemyHealthbar healthBar;
    private List<GameObject> activeDamageTexts = new List<GameObject>();
    private void Start()
    {
        currentHealth = maxHealth;
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        float damage = 0f;

        bool critchance = RollChance(player.critChance);
        

        // Check for IWeapon
        IWeapon weapon = other.GetComponent<IWeapon>();
        IProjectile projectile = other.GetComponent<IProjectile>();
        if (weapon != null && other.CompareTag("Weapon")) //check that it is a player weapon, not an enemy weapon
        {
            damage = weapon.Damage;
        }
        else if(projectile != null && other.CompareTag("Weapon")) //check that it is a player projectile, not an enemy projectile
        {
            damage = projectile.Damage;
        }

        if(damage > 0)
        {
            damage = critchance ? damage * (1.0f+player.critDmg) : damage; // double damage on crit

            ShowDamageNumber((int)damage, "", critchance);



            currentHealth -= (int)damage;
            healthBar.UpdateHealthBar(currentHealth, maxHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    
        
    }

    public bool RollChance(int percent)
    {
        int roll = UnityEngine.Random.Range(0, 100); // 0–99
        return roll < percent;
    }

    private void Die()
    {
        // Optional: play death animation, effects, sound, etc.
        Destroy(gameObject);
    }
    void ShowDamageNumber(int damage, string element, bool crit)
    {
        GameObject dmgText = Instantiate(DamageTextPrefab, transform.position + Vector3.up,Quaternion.identity);

        // Stack offset
        float yOffset = activeDamageTexts.Count * Random.Range(-0.02f, 0.02f); // 0.3 units above previous
        float xOffset = Random.Range(-0.5f, 0.5f); // horizontal variation
        dmgText.transform.position += new Vector3(xOffset, yOffset, 0);

        // Set damage & element
        dmgText.GetComponent<DamageText>().SetDamage(damage, element,false, crit);

        // Track active number
        activeDamageTexts.Add(dmgText);

        // Remove when lifetime ends
        DamageText dt = dmgText.GetComponent<DamageText>();
        dt.OnDestroyEvent += () => activeDamageTexts.Remove(dmgText);
    }
}
