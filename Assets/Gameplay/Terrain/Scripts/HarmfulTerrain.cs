using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Terrain : MonoBehaviour, ITickDmg
{
    [SerializeField] private int damagePerTick = 5;

    [SerializeField] private int duration = 1; // Duration of damage in seconds

    private EnemyDamageHandler damageHandler; //temporary for now
    private PlayerDamageHandler playerDamageHandler;

    public int DamagePerTick { get => damagePerTick; set => damagePerTick = value; }
    public int Duration { get => duration; set => duration = value; }

    public string Element => "fire";

    private void Start()
    {

        damageHandler = GetComponent<EnemyDamageHandler>();
        playerDamageHandler = GetComponent<PlayerDamageHandler>();
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            playerDamageHandler = collision.GetComponent<PlayerDamageHandler>();
            playerDamageHandler?.HandleEnter(collision);
        }
        else if (collision.CompareTag("Enemy"))
        {
            damageHandler?.HandleEnter(collision);
        }
        damageHandler?.HandleEnter(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            playerDamageHandler?.HandleExit(collision);
        }
        else if (collision.CompareTag("Enemy"))
        {
            damageHandler?.HandleExit(collision);
        }
        damageHandler?.HandleExit(collision);
    }
}