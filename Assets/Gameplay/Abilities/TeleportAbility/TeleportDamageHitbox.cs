using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportDamageHitbox : MonoBehaviour, IDamageDealer, ITickDmg
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int damage = 5;
    public float lifetime = 0.2f;

    private int damagePerTick = 1;

    private int duration = 3; // Duration of damage in seconds


    public int Damage
    {
        get => damage;
        set => damage = value;
    }

    public string Element => "fire";

    public int DamagePerTick { get => damagePerTick; set => damagePerTick = value; }
    public int Duration { get => duration; set => duration = value; }
    
    
    private EnemyDamageHandler damageHandler;

    private List<Collider2D> currentCollisions = new List<Collider2D>();

    void Start()
    {
        damageHandler = GetComponent<EnemyDamageHandler>();
        StartCoroutine(LifetimeRoutine());
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!currentCollisions.Contains(collision))
            currentCollisions.Add(collision);
        
        Debug.Log("Hitbox collided with: " + collision.gameObject.name);
        damageHandler?.HandleEnter(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        currentCollisions.Remove(collision);
        damageHandler?.HandleExit(collision);
    }

    private void OnDestroy()
    {
        Debug.Log("Deletion Happening");
        if (damageHandler != null)
        {
            foreach (var col in currentCollisions)
            {
                if (col != null)
                    damageHandler.HandleExit(col);
            }
        }
    }

    private IEnumerator LifetimeRoutine()
    {
        yield return new WaitForSeconds(lifetime);

        // First: manually trigger exits
        foreach (var col in currentCollisions)
        {
            if (col != null)
            {
                Debug.Log("Cleaning up hitbox collision for: " + col.gameObject.name);
                damageHandler?.HandleExit(col);
            }
        }

        // Then destroy
        Destroy(gameObject);
    }
}
