using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Terrain : MonoBehaviour, ITickDmg
{
    [SerializeField] private int damagePerTick = 5;

    [SerializeField] private int duration = 1; // Duration of damage in seconds

    private DamageHandler damageHandler;

    public int DamagePerTick { get => damagePerTick; set => damagePerTick = value; }
    public int Duration { get => duration; set => duration = value; }

    public string Element => "fire";

    private void Start()
    {

        damageHandler = GetComponent<DamageHandler>();
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        damageHandler?.HandleEnter(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        damageHandler?.HandleExit(collision);
    }
}