using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Terrain : MonoBehaviour, ITickDmg
{
    [SerializeField] private int damagePerTick = 5;

    [SerializeField] private int duration = 1; // Duration of damage in seconds

    private EnemyDamageHandler enemyDamageHandler; //temporary for now
    private PlayerDamageHandler playerDamageHandler;

    public int DamagePerTick { get => damagePerTick; set => damagePerTick = value; }
    public int Duration { get => duration; set => duration = value; }

    public string Element => "fire";

    private void Start()
    {

        enemyDamageHandler = GetComponent<EnemyDamageHandler>();
        playerDamageHandler = GetComponent<PlayerDamageHandler>();
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            playerDamageHandler.HandleEnter(collision);
        }
        else if (collision.CompareTag("Enemy"))
        {
            enemyDamageHandler.HandleEnter(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            playerDamageHandler?.HandleExit(collision);
        }
        else if (collision.CompareTag("Enemy"))
        {
            enemyDamageHandler?.HandleExit(collision);
        }
    }
}