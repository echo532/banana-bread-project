using UnityEngine;

public class TeleportDamageHitbox : MonoBehaviour, IDamageDealer, ITickDmg
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int damage = 5;
    public float lifetime = 1f;

    [SerializeField] private int damagePerTick = 1;

    [SerializeField] private int duration = 1; // Duration of damage in seconds


    public int Damage
    {
        get => damage;
        set => damage = value;
    }

    public int DamagePerTick { get => damagePerTick; set => damagePerTick = value; }
    public int Duration { get => duration; set => duration = value; }
    

    private DamageHandler damageHandler;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Player entered harmful terrain");
        damageHandler?.HandleEnter(collision);
    }
}
