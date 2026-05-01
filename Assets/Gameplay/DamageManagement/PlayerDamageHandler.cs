using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageHandler : MonoBehaviour
{

    private PlayerController player;

    private float lastDamageTime = -999f;

    private IHealth healthSystem; // Can be either player or enemy health system
    private DamageHandler damageHandler = new DamageHandler();

    public SpriteRenderer spriteRenderer; // Assign in Inspector
    private Color originalColor;

    [SerializeField] private float damageCooldown = 0.5f;

    private int critChance;

    private List<GameObject> activeDamageTexts = new List<GameObject>();

    public GameObject DamageTextPrefab;

    private TickSystem tickSystem = new TickSystem();

    void Awake()
    {
        healthSystem = GetComponentInChildren<IHealth>();

        player = this.gameObject.GetComponent<PlayerController>();
        critChance = player.critChance;
        originalColor = spriteRenderer.color;
        damageCooldown = 0.5f;

        //setting up any references in damagehandler
        damageHandler.Setup(healthSystem);
        
    }

    public void HandleEnter(Collider2D other)
    {
        damageHandler.HandleEnter(other);
    }

    public void HandleExit(Collider2D other)
    {
        damageHandler.HandleExit(other);
    }



    void Update(){

        tickSystem.Update(Time.deltaTime, damageHandler.tickDamage, HandleDamage);


        bool canTakeDamage = Time.time - lastDamageTime >= damageCooldown;
        int totalDamage = 0;
        if(damageHandler.projectiles.Count > 0 || damageHandler.damageDealers.Count > 0)
        {
            Debug.Log("Damage sources: " + damageHandler.projectiles.Count + " projectiles, " + damageHandler.damageDealers.Count + " dealers.");
        } else
        {
            Debug.Log("No damage sources currently.");
        }
        foreach (var w in damageHandler.projectiles)
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
        damageHandler.projectiles.Clear(); // assume projectile is consumed on hit

        foreach (var w in damageHandler.damageDealers)
        {
            if (w.dealer.Damage > 0)
            {
                HandleDamage(w.dealer.Damage, w.dealer.Element);
                totalDamage += w.dealer.Damage;
            }
                
        }
        damageHandler.damageDealers.Clear(); // prevent multiple hits from same source without exiting and re-entering
        //return totalDamage > 0;

    }

    private void HandleDamage(int damage, string element, string type="normal") //should probs work on this
    {
        healthSystem.TakeDamage(damage);
        HandleVisibleDamage(damage, element, type);
    }

    private void HandleVisibleDamage(int damage, string element, string type="normal") //should probs work on this
    {
        if (true) //player
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

        
        StartCoroutine(FlashRed());
        lastDamageTime = Time.time;


        
        
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

    public bool RollChance(int percent)
    {
        int roll = UnityEngine.Random.Range(0, 100); // 0–99
        return roll < percent;
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


    
}