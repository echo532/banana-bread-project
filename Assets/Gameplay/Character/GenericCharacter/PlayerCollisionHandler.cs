using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class PlayerCollisionHandler : MonoBehaviour
{
    [SerializeField] private float damageCooldown = 0.5f; // Prevent taking damage too rapidly
    
    private HealthSystem healthSystem;

    public GameObject DamageTextPrefab;

    public SpriteRenderer spriteRenderer; // Assign in Inspector

    private DamageHandler damageHandler;
    
    void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
        if (healthSystem == null)
        {
            Debug.LogError("PlayerCollisionHandler requires HealthSystem component!");
        }

        damageHandler = GetComponent<DamageHandler>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {

        damageHandler?.HandleEnter(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        damageHandler?.HandleExit(other);
    }

}