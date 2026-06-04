public abstract class StatusEffect
{
    public string Id;
    public float Duration;
    public float Timer;

    public bool IsExpired => Timer >= Duration;

    public virtual void OnApply(IHealth target) { }

    public virtual void OnTick(IHealth target) { }

    public virtual void OnExpire(IHealth target) { }
}