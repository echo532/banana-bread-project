using System.Collections.Generic;

public class TickSystem {
    
    public class ActiveTickEffect : ITickDmg
    {
        public int DamagePerTick { get; set; }
        public int Duration { get; set; }
        public int SourceId { get; set; }

        public string Element {get; set;}

        public float tickTimer;
        public float durationTimer;
    }

    private List<ActiveTickEffect> activeTickDamage = new();

    public void ApplyTickDamage(ActiveTickEffect tickEffect, System.Action<int, string, string> handleDamage)
    {
        // Check if same effect already exists (prevent duplicates)
        var existing = activeTickDamage.Find(t => t.SourceId == tickEffect.SourceId);

        if (existing != null)
        {
            existing.durationTimer = 0f;
        }
        else
        {
            activeTickDamage.Add(tickEffect);
            handleDamage(tickEffect.DamagePerTick, tickEffect.Element, "tick"); // Apply initial tick damage immediately
        }
    }

    public void Update(float deltaTime, List<(ITickDmg tick, int sourceId)> tickDamage, System.Action<int, string, string> handleDamage)
    {
        foreach (var i in tickDamage){

            ActiveTickEffect tickEffect = new ActiveTickEffect
            {
                DamagePerTick = i.tick.DamagePerTick,
                Duration = i.tick.Duration, // Subtract 1 second to account for immediate application
                tickTimer = 0f,
                durationTimer = 0f,
                SourceId = i.sourceId,
                Element = i.tick.Element
            };
            ApplyTickDamage(tickEffect, handleDamage);
        }

        for (int i = activeTickDamage.Count - 1; i >= 0; i--)
        {
            var tick = activeTickDamage[i];

            tick.tickTimer += deltaTime;
            tick.durationTimer += deltaTime;
            //Debug.Log(tick.durationTimer);

            // Apply tick damage
            if (tick.tickTimer >= 1f)
            {
                handleDamage(tick.DamagePerTick, tick.Element, "tick"); // ✅ ignores cooldown
                tick.tickTimer = 0f;
            }

            // Remove expired effects
            if (tick.durationTimer >= tick.Duration)
            {
                activeTickDamage.RemoveAt(i);
            }
        }
    }
}