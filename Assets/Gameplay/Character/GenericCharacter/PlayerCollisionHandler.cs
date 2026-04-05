using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class PlayerCollisionHandler : MonoBehaviour
{
    [SerializeField] private float damageCooldown = 0.5f; // Prevent taking damage too rapidly
    
    private HealthSystem healthSystem;
    private float lastDamageTime = -999f;

    public GameObject DamageTextPrefab;

    private List<GameObject> activeDamageTexts = new List<GameObject>();
    public SpriteRenderer spriteRenderer; // Assign in Inspector
    private Color originalColor;

    private List<IEnemy> enemies = new List<IEnemy>();
    private List<IWeapon> weapons = new List<IWeapon>();

    private IProjectile projectile1;
    
    void Start()
    {
        originalColor = spriteRenderer.color; // store the original color
        healthSystem = GetComponent<HealthSystem>();
        if (healthSystem == null)
        {
            Debug.LogError("PlayerCollisionHandler requires HealthSystem component!");
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {

        IProjectile projectile = other.GetComponent<IProjectile>();
        if (projectile != null)
        {
            projectile1 = projectile;
        }
        AddIfInterface<IEnemy>(other, enemies);
        AddIfInterface<IWeapon>(other, weapons);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        RemoveIfInterface<IEnemy>(other, enemies);
        RemoveIfInterface<IWeapon>(other, weapons);
    }

     // Helper to add interfaces if present
    private void AddIfInterface<T>(Collider2D col, List<T> list) where T : class
    {
        T component = col.GetComponentInParent<T>();
        if (component != null && !list.Contains(component))
        {
            list.Add(component);
        }
    }

    // Helper to remove interfaces
    private void RemoveIfInterface<T>(Collider2D col, List<T> list) where T : class
    {
        T component = col.GetComponentInParent<T>();
        if (component != null && list.Contains(component))
        {
            list.Remove(component);
        }
    }

    
    // --------------------
    // Apply damage over time
    // --------------------
    private void Update()
    {
        if (Time.time - lastDamageTime < damageCooldown)
        {
            projectile1 = null;
            return;
        }
            

        if (projectile1 != null)
        {
            HandleDamage(projectile1.Damage);
            projectile1 = null; // reset after applying damage
        }

        int totalDamage = 0;

        foreach (var weapon in weapons)
            totalDamage += weapon.Damage;

        foreach (var enemy in enemies)
            totalDamage += enemy.Damage;

        if (totalDamage > 0)
        {
            HandleDamage(totalDamage);
        }
    }

    private void HandleDamage(int dmg)
    {
        if (healthSystem != null)
        {
            healthSystem.TakeDamage(dmg);
            StartCoroutine(FlashRed());
            ShowDamageNumber(dmg, "playerhit");
            lastDamageTime = Time.time;
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
        dmgText.GetComponent<DamageText>().SetDamage(damage, element, true, false);

        // Track active number
        activeDamageTexts.Add(dmgText);

        // Remove when lifetime ends
        DamageText dt = dmgText.GetComponent<DamageText>();
        dt.OnDestroyEvent += () => activeDamageTexts.Remove(dmgText);
    }
    IEnumerator FlashRed()
    {
        for (int i = 0; i < 3; i++)
        {
            // Turn red
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.15f);

            // Back to original color
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.15f);
        }
    }

}