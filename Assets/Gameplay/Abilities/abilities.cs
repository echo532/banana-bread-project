using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    [SerializeField] protected float cooldown = 1f;
    protected float lastUseTime;

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
}