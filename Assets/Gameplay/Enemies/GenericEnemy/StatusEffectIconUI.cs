using UnityEngine;
using UnityEngine.UI;

public class StatusEffectIconUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image cooldownFill;

    private StatusEffect effect;

    public void Initialize(StatusEffect statusEffect, Sprite sprite)
    {
        effect = statusEffect;
        Debug.Log("Initializing icon with sprite: " +
              (sprite != null ? sprite.name : "NULL"));
        if (icon != null)
            icon.sprite = sprite;
    }

    void Update()
    {
        if (effect == null)
        {
            Destroy(gameObject);
            return;
        }

        if (cooldownFill != null)
        {
            cooldownFill.fillAmount =
                Mathf.Clamp01(1f - effect.Timer / effect.Duration);
        }
    }
}