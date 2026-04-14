using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityCooldownUI : MonoBehaviour
{
    [SerializeField] private Ability ability;
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI cooldownText;

    void Update()
    {
        if (ability == null || fillImage == null) return;

        // Fill bar
        fillImage.fillAmount = ability.CooldownProgress;

        // Number display
        if (ability.CooldownProgress >= 1f)
        {
            cooldownText.text = "";
        }
        else
        {
            cooldownText.text = Mathf.Ceil(ability.CooldownRemaining).ToString();
        }
    }
}
