using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    [SerializeField] protected float cooldown = 1f;
    protected float lastUseTime;
    protected virtual void Start()
    {
        // Makes ability ready immediately at game start
        lastUseTime = -cooldown;
    }
    public virtual bool CanUse()
    {
        return Time.time >= lastUseTime + cooldown;
    }

    public void TryActivate()
    {
        if (!CanUse()) return;

        Activate();
        lastUseTime = Time.time;
    }

    protected abstract void Activate();
    public float CooldownProgress
    {
        get
        {
            if (cooldown <= 0f) return 1f;

            return Mathf.Clamp01((Time.time - lastUseTime) / cooldown);
        }
    }
    public float CooldownRemaining
    {
        get
        {
            float remaining = (lastUseTime + cooldown) - Time.time;
            return Mathf.Max(0f, remaining);
        }
    }
}