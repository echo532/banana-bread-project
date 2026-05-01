using System.Collections;
using UnityEngine;

public class EnemyDamageHandler : MonoBehaviour
{

    private EnemyController enemy;

    private float lastDamageTime = -999f;

    private IHealth healthSystem; // Can be either player or enemy health system
    private DamageHandler damageHandler = new DamageHandler();

    public SpriteRenderer spriteRenderer; // Assign in Inspector
    private Color originalColor;

    [SerializeField] private float damageCooldown = 0.5f;

    private int critChance;

    void Awake()
    {
        healthSystem = GetComponentInChildren<IHealth>();

        enemy = this.gameObject.GetComponent<EnemyController>();
        critChance = 0;
        damageCooldown = 0.0f;
        //tempSpeed = enemy.moveSpeed;

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

        //damageHandler.SomethingHealth(healthSystem);

        bool canTakeDamage = Time.time - lastDamageTime >= damageCooldown;

        if (canTakeDamage && damageHandler.ProcessDamage()) //if damage taken
        {
            StartCoroutine(FlashRed());
            lastDamageTime = Time.time;
        }


        //StartCoroutine(FlashRed());
        //lastDamageTime = Time.time;

        //freeze section
        // if (!isPlayer)
        // {
        //     if(isFrozen)
        //     {
        //         enemy.moveSpeed = 0f; // skip rest of update if frozen}
        //     } else
        //     {
        //         enemy.moveSpeed = tempSpeed; // reset to normal speed if not frozen
        //     }
        // }
        
    }

    private void Die()
    {
        // Optional: play death animation, effects, sound, etc.
        Destroy(gameObject);
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