using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class DamageHandler : MonoBehaviour
{
    [SerializeField] private float damageCooldown = 0.5f;

    [SerializeField] public PlayerController player;

    private float lastDamageTime = -999f;
    private List<IDamageDealer> damageDealers = new();

    private List<ITickDmg> tickDamage = new();
    private List<ActiveTickEffect> activeTickDamage = new();
    private List<IProjectile> projectiles = new();

    public GameObject DamageTextPrefab;

    private List<GameObject> activeDamageTexts = new List<GameObject>();
    public SpriteRenderer spriteRenderer; // Assign in Inspector
    private Color originalColor;

    private int critChance;

    private bool isPlayer;

    //tick
    private float tickInterval = 1f; // once per second



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
    }

    public void HandleEnter(Collider2D other)
    {
        AddIfInterface<IProjectile>(other, projectiles);
        AddIfInterface<IDamageDealer>(other, damageDealers);
        AddIfInterface<ITickDmg>(other, tickDamage);
        
    }

    public void HandleExit(Collider2D other)
    {
        RemoveIfInterface<IDamageDealer>(other, damageDealers);
        RemoveIfInterface<ITickDmg>(other, tickDamage);
    }

    void Update()
    {
        foreach (var i in tickDamage){
            ApplyTickDamage(i.DamagePerTick, i.Duration);
        }

        for (int i = activeTickDamage.Count - 1; i >= 0; i--)
        {
            var tick = activeTickDamage[i];

            tick.tickTimer += Time.deltaTime;
            tick.durationTimer += Time.deltaTime;

            // Apply tick damage
            if (tick.tickTimer >= 1f)
            {
                HandleDamage(tick.DamagePerTick, "tick"); // ✅ ignores cooldown
                tick.tickTimer = 0f;
            }

            // Remove expired effects
            if (tick.durationTimer >= tick.Duration)
            {
                activeTickDamage.RemoveAt(i);
            }
        }


        bool canTakeDamage = Time.time - lastDamageTime >= damageCooldown;

        int totalDamage = 0;

        if (canTakeDamage)
        {
            foreach (var w in projectiles) totalDamage += w.Damage;
            projectiles.Clear(); // assume projectile is consumed on hit
            foreach (var w in damageDealers) totalDamage += w.Damage;
            damageDealers.Clear(); // prevent multiple hits from same source without exiting and re-entering
            if (totalDamage > 0)
                HandleDamage(totalDamage);
        }

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

    private void HandleDamage(int damage, string type="normal") //should probs work on this
    {
        if (isPlayer) //player
        {
            if(type == "tick")
            {
                ShowDamageNumber(damage, "", true, "tick");
            }
            else
            {
                ShowDamageNumber(damage, "", true);
            }
            
        } else //handle enemy
        {
            
            if (type == "tick")
            {
               ShowDamageNumber(damage, "normal", false, type); //handles normal and tick damage types for enemies
            }
            else
            {
                bool crit = RollChance(player.critChance);
                damage = (int)(crit ? damage * (1.0f+player.critDmg) : damage); // crit for enemies only
                if (crit)
                    ShowDamageNumber(damage, "normal", false, "crit");
                else
                    ShowDamageNumber(damage, "normal", false);
            }
            
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

    void ShowDamageNumber(int damage, string element, bool playerhit = false, string type = "normal")
    {
        GameObject dmgText = Instantiate(DamageTextPrefab, transform.position + Vector3.up,Quaternion.identity);

        // Stack offset
        float yOffset = activeDamageTexts.Count * Random.Range(-0.02f, 0.02f); // 0.3 units above previous
        float xOffset = Random.Range(-0.5f, 0.5f); // horizontal variation
        dmgText.transform.position += new Vector3(xOffset, yOffset, 0);

        // Set damage & element
        dmgText.GetComponent<DamageText>().SetDamage(damage, element, playerhit, type);

        // Track active number
        activeDamageTexts.Add(dmgText);

        // Remove when lifetime ends
        DamageText dt = dmgText.GetComponent<DamageText>();
        dt.OnDestroyEvent += () => activeDamageTexts.Remove(dmgText);
    }
    IEnumerator FlashRed()
    {
        for (int i = 0; i < 1; i++)
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


    class ActiveTickEffect : ITickDmg
    {
        public int DamagePerTick { get; set; }
        public int Duration { get; set; }

        public float tickTimer;
        public float durationTimer;
    }

    public void ApplyTickDamage(int damage, int duration)
{
    // Check if same effect already exists (prevent duplicates)
    var existing = activeTickDamage.Find(t => t.DamagePerTick == damage);

    if (existing != null)
    {
        // Refresh duration
        existing.durationTimer = 0f;
    }
    else
    {
        activeTickDamage.Add(new ActiveTickEffect
        {
            DamagePerTick = damage,
            Duration = duration - 1, // Subtract 1 second to account for immediate application
            tickTimer = 0f,
            durationTimer = 0f
        });
        HandleDamage(damage, "tick"); // Apply initial tick damage immediately
    }
}
}
