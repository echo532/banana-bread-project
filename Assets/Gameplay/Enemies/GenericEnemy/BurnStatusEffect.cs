using UnityEngine;

public class BurnStatusEffect : StatusEffect
{
    public int damagePerTick = 2;
    public float tickInterval = 1f;

    private float tickTimer;

    private System.Action<int, string, string> dealDamage;

    public BurnStatusEffect(System.Action<int, string, string> damageCallback)
    {
        Id = "burn";
        dealDamage = damageCallback;
    }

    public override void OnTick(IHealth target)
    {
        tickTimer += Time.deltaTime;

        if (tickTimer >= tickInterval)
        {
            tickTimer = 0f;

            dealDamage?.Invoke(damagePerTick, "fire", "tick");
        }
    }
}