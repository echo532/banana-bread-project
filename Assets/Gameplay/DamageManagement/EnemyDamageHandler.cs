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
    [SerializeField] private Color burnColor = new Color(1f, 0.5f, 0f);

    public static List<EnemyDamageHandler> JoltedEnemies = new();

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
        UpdateVisuals();
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
        // Existing tick system
        tickSystem.Update(Time.deltaTime, damageHandler.tickDamage, DealDamage);

        // Projectiles
        foreach (var w in damageHandler.projectiles)
        {
            if (w.projectile.Damage > 0)
            {
                DealDamage(w.projectile.Damage, w.projectile.Element, "normal");

                if (w.projectile.Element == "fire")
                {
                    ApplyStatus(new BurnStatusEffect(DealDamage)
                    {
                        damagePerTick = 2,
                        Duration = 5f
                    });
                }

                if (w.projectile.Element == "lightning")
                {
                    ApplyStatus(new JoltStatusEffect()
                    {
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

                if (w.dealer.Element == "fire")
                {
                    ApplyStatus(new BurnStatusEffect(DealDamage)
                    {
                        damagePerTick = 2,
                        Duration = 5f
                    });
                }

                if (w.dealer.Element == "lightning")
                {
                    ApplyStatus(new JoltStatusEffect()
                    {
                        Duration = 5f
                    });
                }
            }
        }

        damageHandler.damageDealers.Clear();
    }

    // ---------------------------
    // DAMAGE
    // ---------------------------
    private void DealDamage(int damage, string element, string type = "normal")
    {
        healthSystem.TakeDamage(damage);

        ShowDamageNumber(damage, element, type);

        // Trigger chain lightning only from normal hits
        if (HasEffect("jolt") && type != "jolt")
        {
            TriggerJolt();
        }

        CheckDeath();
    }

    // ---------------------------
    // JOLT SYSTEM
    // ---------------------------
    private void TriggerJolt()
    {
        foreach (var enemy in JoltedEnemies)
        {
            if (enemy == null || enemy == this)
                continue;

            enemy.ReceiveJoltDamage(1);
        }
    }

    public void ReceiveJoltDamage(int damage)
    {
        healthSystem.TakeDamage(damage);

        ShowDamageNumber(damage, "lightning", "jolt");

        CheckDeath();
    }

    // ---------------------------
    // DEATH CHECK
    // ---------------------------
    private void CheckDeath()
    {
        if (healthSystem.CurrentHealth <= 0)
        {
            JoltedEnemies.Remove(this);

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
                if (effect.Id == "jolt")
                {
                    JoltedEnemies.Remove(this);
                }

                effect.OnExpire(healthSystem);
                activeEffects.RemoveAt(i);
            }
        }
    }

    public void ApplyStatus(StatusEffect effect)
    {
        var existing = activeEffects.Find(e => e.Id == effect.Id);

        if (existing != null)
        {
            existing.Timer = 0f;
            return;
        }

        effect.OnApply(healthSystem);
        activeEffects.Add(effect);

        if (effect.Id == "jolt")
        {
            if (!JoltedEnemies.Contains(this))
            {
                JoltedEnemies.Add(this);
            }
        }
    }

    public bool HasEffect(string id)
    {
        return activeEffects.Exists(e => e.Id == id);
    }

    public StatusEffect GetEffect(string id)
    {
        return activeEffects.Find(e => e.Id == id);
    }

    // ---------------------------
    // VISUALS
    // ---------------------------
    private void UpdateVisuals()
    {
        bool isBurning = HasEffect("burn");

        if (spriteRenderer == null) return;

        spriteRenderer.color = isBurning ? burnColor : originalColor;
    }
}