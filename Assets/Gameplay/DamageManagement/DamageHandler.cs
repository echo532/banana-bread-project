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
        originalColor = spriteRenderer.color; // store the original color
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
            HandleDamage(projectile.Damage);
            projectile = null;
        }

        int totalDamage = 0;

        foreach (var w in weapons) totalDamage += w.Damage;
        foreach (var e in enemies) totalDamage += e.Damage;

        if (totalDamage > 0)
            HandleDamage(totalDamage);
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

    private void HandleDamage(int dmg)
    {
        healthSystem.TakeDamage(dmg);
        StartCoroutine(FlashRed());
        ShowDamageNumber(dmg, "playerhit");
        lastDamageTime = Time.time;
        
    }

    void ShowDamageNumber(int damage, string element)
    {
        GameObject dmgText = Instantiate(DamageTextPrefab, transform.position + Vector3.up,Quaternion.identity);

        // Stack offset
        float yOffset = activeDamageTexts.Count * Random.Range(-0.02f, 0.02f); // 0.3 units above previous
        float xOffset = Random.Range(-0.5f, 0.5f); // horizontal variation
        dmgText.transform.position += new Vector3(xOffset, yOffset, 0);

        // Set damage & element
        dmgText.GetComponent<DamageText>().SetDamage(damage, element, true, false);

        // Track active number
        activeDamageTexts.Add(dmgText);

        // Remove when lifetime ends
        DamageText dt = dmgText.GetComponent<DamageText>();
        dt.OnDestroyEvent += () => activeDamageTexts.Remove(dmgText);
    }
    IEnumerator FlashRed()
    {
        for (int i = 0; i < 3; i++)
        {
            // Turn red
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.15f);

            // Back to original color
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.15f);
        }
    }
}
