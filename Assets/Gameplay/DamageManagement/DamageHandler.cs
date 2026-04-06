using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class DamageHandler : MonoBehaviour
{
    [SerializeField] private float damageCooldown = 0.5f;

    private float lastDamageTime = -999f;
    private HealthSystem healthSystem;

    private List<IEnemy> enemies = new();
    private List<IWeapon> weapons = new();
    private IProjectile projectile;

    public GameObject DamageTextPrefab;

    private List<GameObject> activeDamageTexts = new List<GameObject>();
    public SpriteRenderer spriteRenderer; // Assign in Inspector
    private Color originalColor;

    [SerializeField] private MonoBehaviour healthsystem; // Can be either player or enemy health system

    private IHealth health => healthsystem as IHealth;

    void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
    }

    public void HandleEnter(Collider2D other)
    {
        IProjectile proj = other.GetComponent<IProjectile>();
        if (proj != null)
            projectile = proj;

        AddIfInterface<IEnemy>(other, enemies);
        AddIfInterface<IWeapon>(other, weapons);
    }

    public void HandleExit(Collider2D other)
    {
        RemoveIfInterface<IEnemy>(other, enemies);
        RemoveIfInterface<IWeapon>(other, weapons);
    }

    void Update()
    {
        if (Time.time - lastDamageTime < damageCooldown)
        {
            projectile = null;
            return;
        }

        if (projectile != null)
        {
            ApplyDamage(projectile.Damage);
            projectile = null;
        }

        int totalDamage = 0;

        foreach (var w in weapons) totalDamage += w.Damage;
        foreach (var e in enemies) totalDamage += e.Damage;

        if (totalDamage > 0)
            ApplyDamage(totalDamage);
    }

    void ApplyDamage(int dmg)
    {
        healthSystem?.TakeDamage(dmg);
        lastDamageTime = Time.time;
    }

    private void AddIfInterface<T>(Collider2D col, List<T> list) where T : class
    {
        var comp = col.GetComponentInParent<T>();
        if (comp != null && !list.Contains(comp))
            list.Add(comp);
    }

    private void RemoveIfInterface<T>(Collider2D col, List<T> list) where T : class
    {
        var comp = col.GetComponentInParent<T>();
        if (comp != null)
            list.Remove(comp);
    }
}
