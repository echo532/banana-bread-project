using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

public class DamageHandler
{
    [SerializeField] private float damageCooldown = 0.5f;

    private PlayerController player;

    private float lastDamageTime = -999f;
    public List<(IDamageDealer dealer, int sourceId)> damageDealers = new();

    private List<(ITickDmg tick, int sourceId)> tickDamage = new();
    public List<(IProjectile projectile, int sourceId)> projectiles = new();

    public GameObject DamageTextPrefab;

    private List<GameObject> activeDamageTexts = new List<GameObject>();
    public SpriteRenderer spriteRenderer; // Assign in Inspector
    private Color originalColor;

    private int critChance;

    private bool isPlayer;

    

    private EnemyController enemy; // Reference to enemy, if applicable
    private bool isFrozen = false;
    private float tempSpeed;

    private TickSystem tickSystem = new TickSystem();

    private IHealth healthSystem; // Can be either player or enemy health system



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

    public void Setup(IHealth health)
    {
        healthSystem = health;
    }

    void Update()
    {

        //all tick updates and applications
        tickSystem.Update(Time.deltaTime, tickDamage, HandleDamage);


    }

    public bool ProcessDamage()
    {

        int totalDamage = 0;
        foreach (var w in projectiles)
        {
            if (w.projectile.Damage > 0)
            {
                HandleDamage(w.projectile.Damage, w.projectile.Element);
                totalDamage += w.projectile.Damage;
                //  if(w.projectile.Element == "ice" && !isPlayer) // Freeze player if hit by enemy projectile
                //  {
                //     StartCoroutine(Freeze(5f));
                //  }
            }
        }
        projectiles.Clear(); // assume projectile is consumed on hit

        foreach (var w in damageDealers)
        {
            if (w.dealer.Damage > 0)
            {
                HandleDamage(w.dealer.Damage, w.dealer.Element);
                totalDamage += w.dealer.Damage;
            }
                
        }
        damageDealers.Clear(); // prevent multiple hits from same source without exiting and re-entering
        return totalDamage > 0;
    }

    private void HandleDamage(int damage, string element, string type="normal") //should probs work on this
    {
        healthSystem.TakeDamage(damage);
        HandleVisibleDamage(damage, element, type);
    }

    private void HandleVisibleDamage(int damage, string element, string type="normal") //should probs work on this
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

        
        //StartCoroutine(FlashRed());
        lastDamageTime = Time.time;


        
        
    }


    void ShowDamageNumber(int damage, string element, bool playerhit = false, string type = "normal")
    {
        // GameObject dmgText = Instantiate(DamageTextPrefab, transform.position + Vector3.up,Quaternion.identity);

        // // Stack offset
        // float yOffset = activeDamageTexts.Count * UnityEngine.Random.Range(-0.02f, 0.02f); // 0.3 units above previous
        // float xOffset = UnityEngine.Random.Range(-0.5f, 0.5f); // horizontal variation
        // dmgText.transform.position += new Vector3(xOffset, yOffset, 0);

        // // Set damage & element
        // dmgText.GetComponent<DamageText>().SetDamage(damage, element, playerhit, type);

        // // Track active number
        // activeDamageTexts.Add(dmgText);

        // // Remove when lifetime ends
        // DamageText dt = dmgText.GetComponent<DamageText>();
        // dt.OnDestroyEvent += () => activeDamageTexts.Remove(dmgText);
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
