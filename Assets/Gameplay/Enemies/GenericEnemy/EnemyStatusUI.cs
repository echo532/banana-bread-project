using UnityEngine;

public class StatusIcon : MonoBehaviour
{
    [SerializeField] private EnemyDamageHandler enemy;
    [SerializeField] private string statusId = "burn";

    [SerializeField] private UnityEngine.UI.Image icon;

    void Update()
    {
        if (enemy == null) return;

        icon.enabled = enemy.HasEffect(statusId);
    }
}