using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class DamageHandler : MonoBehaviour
{
    [SerializeField] private float damageCooldown = 0.5f;

    [SerializeField] public PlayerController player;

    private float lastDamageTime = -999f;

    private List<IEnemy> enemies = new();
    private List<IWeapon> weapons = new();

    private List<IWeapon> tickDamage = new();
    private IProjectile projectile;

    public GameObject DamageTextPrefab;

    private List<GameObject> activeDamageTexts = new List<GameObject>();
    public SpriteRenderer spriteRenderer; // Assign in Inspector
    private Color originalColor;

    private int critChance;

    private bool isPlayer;



    private IHealth healthSystem; // Can be either player or enemy health system


    void Awake()
    {
        healthSystem = GetComponentInChildren<IHealth>();
        

        originalColor = spriteRenderer.color; // store the original color

        if(GetComponent<PlayerController>() != null) //if this is a player
        {
            isPlayer = true;
            critChance = GetComponent<PlayerController>().critChance;
            damageCooldown = 0.5f;
        } else //this is an enemy or some other thing (terrain, etc.)
        {
            isPlayer = false;
            critChance = 0;
            damageCooldown = 0.0f;
        }

        Debug.Log("Player: " + isPlayer);

    }

    public void HandleEnter(Collider2D other)
    {
        IProjectile proj = other.GetComponent<IProjectile>();
        if (proj != null)
            projectile = proj;

        AddIfInterface<IEnemy>(other, enemies);
        AddIfInterface<IWeapon>(other, weapons);
    }

    public void HandleExit(Collider2D other)
    {
        RemoveIfInterface<IEnemy>(other, enemies);
        RemoveIfInterface<IWeapon>(other, weapons);
    }

    void Update()
    {
        if (Time.time - lastDamageTime < damageCooldown)
        {
            projectile = null;
            return;
        }

        if (projectile != null)
        {
            HandleDamage(projectile.Damage);
            projectile = null;
        }

        int totalDamage = 0;

        foreach (var w in weapons) totalDamage += w.Damage;
        foreach (var e in enemies) totalDamage += e.Damage;

        if (totalDamage > 0)
            HandleDamage(totalDamage);

            

        
    }

    private void AddIfInterface<T>(Collider2D col, List<T> list) where T : class
    {
        var comp = col.GetComponentInParent<T>();
        if (comp != null && !list.Contains(comp))
            list.Add(comp);
    }

    private void RemoveIfInterface<T>(Collider2D col, List<T> list) where T : class
    {
        var comp = col.GetComponentInParent<T>();
        if (comp != null)
            list.Remove(comp);
    }

    private void HandleDamage(int damage)
    {
        Debug.Log($"{gameObject.name} takes {damage} damage!");
        if (isPlayer) //player
        {
            ShowDamageNumber(damage, "fire", true);
        } else //handle enemy
        {
            bool crit = RollChance(player.critChance);
            damage = (int)(crit ? damage * (1.0f+player.critDmg) : damage); // crit for enemies only
            ShowDamageNumber(damage, "fire", false, crit);
        }

        healthSystem.TakeDamage(damage);
        StartCoroutine(FlashRed());
        lastDamageTime = Time.time;


        if (healthSystem.CurrentHealth <= 0)
        {
            Die();
        }
        
        
    }

    private void Die()
    {
        // Optional: play death animation, effects, sound, etc.
        Destroy(gameObject);
    }

    void ShowDamageNumber(int damage, string element, bool playerhit = false, bool crit = false)
    {
        GameObject dmgText = Instantiate(DamageTextPrefab, transform.position + Vector3.up,Quaternion.identity);

        // Stack offset
        float yOffset = activeDamageTexts.Count * Random.Range(-0.02f, 0.02f); // 0.3 units above previous
        float xOffset = Random.Range(-0.5f, 0.5f); // horizontal variation
        dmgText.transform.position += new Vector3(xOffset, yOffset, 0);

        // Set damage & element
        dmgText.GetComponent<DamageText>().SetDamage(damage, element, playerhit, crit);

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

    public bool RollChance(int percent)
    {
        int roll = UnityEngine.Random.Range(0, 100); // 0–99
        return roll < percent;
    }
}
