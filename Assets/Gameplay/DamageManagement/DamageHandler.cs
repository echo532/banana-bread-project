using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

public class DamageHandler : MonoBehaviour
{
    [SerializeField] private float damageCooldown = 0.5f;

    private PlayerController player;

    private float lastDamageTime = -999f;
    private List<(IDamageDealer dealer, int sourceId)> damageDealers = new();

    private List<(ITickDmg tick, int sourceId)> tickDamage = new();
    private List<(IProjectile projectile, int sourceId)> projectiles = new();

    public GameObject DamageTextPrefab;

    private List<GameObject> activeDamageTexts = new List<GameObject>();
    public SpriteRenderer spriteRenderer; // Assign in Inspector
    private Color originalColor;

    private int critChance;

    private bool isPlayer;

    //tick
    private float tickInterval = 1f; // once per second



    private IHealth healthSystem; // Can be either player or enemy health system

    private EnemyController enemy; // Reference to enemy, if applicable
    private bool isFrozen = false;
    private float tempSpeed;

    private TickSystem tickSystem = new TickSystem();

    void Awake()
    {
        player = UnityEngine.Object.FindObjectOfType<PlayerController>();

        healthSystem = GetComponentInChildren<IHealth>();
        

        originalColor = spriteRenderer.color; // store the original color

        if(CompareTag("Player")) //if this is a player
        {
            isPlayer = true;
            critChance = player.critChance;
            damageCooldown = 0.5f;
        } else //this is an enemy or some other thing (terrain, etc.)
        {
            isPlayer = false;
            enemy = this.gameObject.GetComponent<EnemyController>();
            critChance = 0;
            damageCooldown = 0.0f;
            tempSpeed = enemy.moveSpeed;
        }
    }

    public void HandleEnter(Collider2D other)
    {
        AddIfInterface<IProjectile, int>(other, other.gameObject.GetInstanceID(), projectiles);
        AddIfInterface<IDamageDealer, int>(other, other.gameObject.GetInstanceID(), damageDealers);
        AddIfInterface<ITickDmg, int>(other, other.gameObject.GetInstanceID(), tickDamage);
        
    }

    public void HandleExit(Collider2D other)
    {
        RemoveIfInterface<IDamageDealer>(other, damageDealers);
        RemoveIfInterface<ITickDmg>(other, tickDamage);
    }

    void Update()
    {


        //freeze section
        if (!isPlayer)
        {
            if(isFrozen)
            {
                enemy.moveSpeed = 0f; // skip rest of update if frozen}
            } else
            {
                enemy.moveSpeed = tempSpeed; // reset to normal speed if not frozen
            }
        }

        tickSystem.Update(Time.deltaTime, tickDamage, HandleDamage);


        bool canTakeDamage = Time.time - lastDamageTime >= damageCooldown;

       

        if (canTakeDamage)
        {
            foreach (var w in projectiles){
                if (w.projectile.Damage > 0)
                {
                     HandleDamage( w.projectile.Damage, w.projectile.Element);
                     if(w.projectile.Element == "ice" && !isPlayer) // Freeze player if hit by enemy projectile
                     {
                        StartCoroutine(Freeze(5f));
                     }
                }
                   
            } 

            projectiles.Clear(); // assume projectile is consumed on hit

            foreach (var w in damageDealers)
            {
                if (w.dealer.Damage > 0)
                    HandleDamage( w.dealer.Damage, w.dealer.Element);
            }
            damageDealers.Clear(); // prevent multiple hits from same source without exiting and re-entering
        }

    }

    private void AddIfInterface<T, Integer>(Collider2D col, int sourceId, List<(T, int)> list) where T : class
    {
        var comp = col.GetComponentInParent<T>();
        if (comp != null && !list.Contains((comp, sourceId)))
            list.Add((comp, sourceId));
    }

    private void RemoveIfInterface<T>(Collider2D col, List<(T, int)> list) where T : class
    {
        var comp = col.GetComponentInParent<T>();
        if (comp != null)
            list.RemoveAll(item => item.Item1 == comp);
    }

    private void HandleDamage(int damage, string element, string type="normal") //should probs work on this
    {
        if (isPlayer) //player
        {
            if(type == "tick")
            {
                ShowDamageNumber(damage, element, true, "tick");
            }
            else
            {
                ShowDamageNumber(damage, element, true);
            }
            
        } else //handle enemy
        {
            
            if (type == "tick")
            {
               ShowDamageNumber(damage, element, false, type); //handles normal and tick damage types for enemies
            }
            else
            {
                bool crit = RollChance(player.critChance);
                damage = (int)(crit ? damage * (1.0f+player.critDmg) : damage); // crit for enemies only
                if (crit)
                    ShowDamageNumber(damage, element, false, "crit");
                else
                    ShowDamageNumber(damage, element, false);
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
        float yOffset = activeDamageTexts.Count * UnityEngine.Random.Range(-0.02f, 0.02f); // 0.3 units above previous
        float xOffset = UnityEngine.Random.Range(-0.5f, 0.5f); // horizontal variation
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

    IEnumerator Freeze(float duration)
    {
        isFrozen = true;

        yield return new WaitForSeconds(duration);

        isFrozen = false;
    }

    public bool RollChance(int percent)
    {
        int roll = UnityEngine.Random.Range(0, 100); // 0–99
        return roll < percent;
    }
}
