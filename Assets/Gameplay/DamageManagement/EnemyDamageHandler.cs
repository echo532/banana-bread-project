using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageHandler : MonoBehaviour
{
    private PlayerController player;

    private IHealth healthSystem;
    private DamageHandler damageHandler = new DamageHandler();
    private TickSystem tickSystem = new TickSystem();

    [SerializeField] private SpriteRenderer spriteRenderer;
    private Color originalColor;

    public GameObject DamageTextPrefab;

    private float lastDamageTime = -999f;

    [SerializeField] private float damageCooldown = 0.5f;

    // ---------------------------
    // STATUS EFFECTS
    // ---------------------------
    public List<StatusEffect> activeEffects = new();

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        healthSystem = GetComponentInChildren<IHealth>();
        damageHandler.Setup(healthSystem);

        originalColor = spriteRenderer.color;
    }

    void Update()
    {
        HandleIncomingDamage();
        HandleStatusEffects();
    }

    // ---------------------------
    // COLLISIONS
    // ---------------------------
    public void HandleEnter(Collider2D other)
    {
        damageHandler.HandleEnter(other);
    }

    public void HandleExit(Collider2D other)
    {
        damageHandler.HandleExit(other);
    }

    // ---------------------------
    // MAIN DAMAGE LOOP
    // ---------------------------
    private void HandleIncomingDamage()
    {
        // Tick system (DO NOT bypass DealDamage)
        tickSystem.Update(Time.deltaTime, damageHandler.tickDamage, DealDamage);

        // Projectiles
        foreach (var w in damageHandler.projectiles)
        {
            if (w.projectile.Damage > 0)
            {
                DealDamage(w.projectile.Damage, w.projectile.Element, "normal");

                // Example: fire → burn
                if (w.projectile.Element == "fire")
                {
                    ApplyStatus(new BurnStatusEffect(DealDamage)
                    {
                        damagePerTick = 2,
                        Duration = 5f
                    });
                }
            }
        }
        damageHandler.projectiles.Clear();

        // Melee / contact damage
        foreach (var w in damageHandler.damageDealers)
        {
            if (w.dealer.Damage > 0)
            {
                DealDamage(w.dealer.Damage, w.dealer.Element, "normal");
            }
        }
        damageHandler.damageDealers.Clear();
    }

    // ---------------------------
    // UNIFIED DAMAGE FUNCTION (IMPORTANT)
    // ---------------------------
    private void DealDamage(int damage, string element, string type = "normal")
    {
        healthSystem.TakeDamage(damage);

        ShowDamageNumber(damage, element, type);

        CheckDeath();
    }

    // ---------------------------
    // DEATH CHECK (USED BY EVERYTHING)
    // ---------------------------
    private void CheckDeath()
    {
        if (healthSystem.CurrentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    // ---------------------------
    // DAMAGE NUMBER UI
    // ---------------------------
    private void ShowDamageNumber(int damage, string element, string type)
    {
        if (DamageTextPrefab == null) return;

        GameObject obj = Instantiate(
            DamageTextPrefab,
            transform.position + Vector3.up,
            Quaternion.identity
        );

        obj.GetComponent<DamageText>()
            .SetDamage(damage, element, false, type);
    }

    // ---------------------------
    // STATUS EFFECT SYSTEM
    // ---------------------------
    private void HandleStatusEffects()
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            var effect = activeEffects[i];

            effect.Timer += Time.deltaTime;
            effect.OnTick(healthSystem);

            if (effect.IsExpired)
            {
                effect.OnExpire(healthSystem);
                activeEffects.RemoveAt(i);
            }
        }
    }

    public void ApplyStatus(StatusEffect effect)
    {
        effect.OnApply(healthSystem);
        activeEffects.Add(effect);
    }

    public bool HasEffect(string id)
    {
        return activeEffects.Exists(e => e.Id == id);
    }
}