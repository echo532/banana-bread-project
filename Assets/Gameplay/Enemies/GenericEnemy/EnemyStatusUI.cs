using UnityEngine;

public class StatusIcon : MonoBehaviour
{
    [SerializeField] private EnemyDamageHandler enemy;
    [SerializeField] private string statusId = "burn";

    void Update()
    {
        if (enemy == null) return;

        gameObject.SetActive(enemy.HasEffect(statusId));
    }
}