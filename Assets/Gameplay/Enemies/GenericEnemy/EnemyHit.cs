using UnityEngine;
using System.Collections.Generic;
 // assign your Canvas

[RequireComponent(typeof(Collider2D))]
public class EnemyHit : MonoBehaviour
{

    private PlayerController player;
    
    
    public GameObject DamageTextPrefab;
    private EnemyHealthbar healthBar;

    private DamageHandler damageHandler;
    private void Start()
    {

        player = GetComponent<PlayerController>();
        healthBar = GetComponent<EnemyHealthbar>();

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

    public bool RollChance(int percent)
    {
        int roll = UnityEngine.Random.Range(0, 100); // 0–99
        return roll < percent;
    }
}
