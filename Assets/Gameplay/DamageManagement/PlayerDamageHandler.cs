using System.Collections;
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

    private TickSystem tickSystem = new TickSystem();

    void Awake()
    {
        healthSystem = GetComponentInChildren<IHealth>();

        player = this.gameObject.GetComponent<PlayerController>();
        critChance = player.critChance;
        originalColor = spriteRenderer.color;
        damageCooldown = 0.5f;

        //setting up any references in damagehandler
        damageHandler.GetComponent<DamageHandler>();
        
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


        bool canTakeDamage = Time.time - lastDamageTime >= damageCooldown;

        if (canTakeDamage && damageHandler.ProcessDamage()) //if damage taken
        {
            StartCoroutine(FlashRed());
            lastDamageTime = Time.time;
        }
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