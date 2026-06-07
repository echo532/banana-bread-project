using System.Collections.Generic;
using UnityEngine;

public class StatusBarUI : MonoBehaviour
{
    [SerializeField] private EnemyDamageHandler enemy;

    [SerializeField] private GameObject iconPrefab;

    [SerializeField] private List<StatusIconData> icons;

    private Dictionary<string, StatusEffectIconUI> activeIcons =
        new Dictionary<string, StatusEffectIconUI>();

    void Update()
    {
        if (enemy == null) return;

        UpdateIcons();
    }

    private void UpdateIcons()
    {
        foreach (var effect in enemy.activeEffects)
        {
            Debug.Log("Found effect: " + effect.Id);

            if (activeIcons.ContainsKey(effect.Id))
                continue;

            Sprite sprite = GetSprite(effect.Id);

            if (sprite == null)
            {
                Debug.LogWarning("No sprite found for " + effect.Id);
                continue;
            }

            Debug.Log("Creating icon for " + effect.Id);
            
            GameObject obj = Instantiate(iconPrefab, transform);
            Debug.Log("Created: " + obj.name);
            StatusEffectIconUI icon =
                obj.GetComponent<StatusEffectIconUI>();

            icon.Initialize(effect, sprite);

            activeIcons.Add(effect.Id, icon);
        }

        // Remove expired icons
        List<string> removeList = new();

        foreach (var pair in activeIcons)
        {
            if (!enemy.HasEffect(pair.Key))
            {
                Destroy(pair.Value.gameObject);
                removeList.Add(pair.Key);
            }
        }

        foreach (var id in removeList)
        {
            activeIcons.Remove(id);
        }
    }

    private Sprite GetSprite(string statusId)
    {
        foreach (var data in icons)
        {
            if (data.statusId == statusId)
                return data.icon;
        }

        return null;
    }
}