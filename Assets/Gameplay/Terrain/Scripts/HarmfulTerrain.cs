using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Terrain : MonoBehaviour, ITickDmg
{
    [SerializeField] private int damagePerTick = 5;

    [SerializeField] private int time = 1; // Duration of damage in seconds

    public int DamagePerTick { get => damagePerTick; set => damagePerTick = value; }
    public int Time { get => time; set => time = value; }

    
}