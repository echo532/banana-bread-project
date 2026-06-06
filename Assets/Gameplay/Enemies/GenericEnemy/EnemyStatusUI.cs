using UnityEngine;
using UnityEngine.UI;

public class StatusIcon : MonoBehaviour
{
    [SerializeField] private EnemyDamageHandler enemy;
    [SerializeField] private string statusId = "burn";

    [SerializeField] private Image icon;
    [SerializeField] private Image cooldownFill;

    void Update()
    {
        if (enemy == null) return;

        var effect = enemy.GetEffect(statusId);

        bool active = effect != null;

        icon.enabled = active;
        cooldownFill.enabled = active;

        if (!active) return;

        cooldownFill.fillAmount =
            1f - (effect.Timer / effect.Duration);
    }
}