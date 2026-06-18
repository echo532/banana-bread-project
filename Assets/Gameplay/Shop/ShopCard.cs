using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopCard : MonoBehaviour, IPointerClickHandler
{
    [Header("UI")]
    public TMP_Text costText;
    public TMP_Text statGainText;

    [Header("Data")]
    public int cost;
    public int statGain;

    void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (costText != null)
            costText.text = "Cost: " + cost.ToString();

        if (statGainText != null)
            statGainText.text = statGain.ToString() + " Stat Gain";
    }

    // This runs when you click the card
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked card");

        OnCardClicked();
    }

    // Put your custom behavior here
    private void OnCardClicked()
    {
        // Example behavior:
        if (GameData.coins >= cost)
        {
            GameData.coins -= cost;
            GameData.PlayerMaxHealth += statGain; // Example: increase max health

            // You can also trigger other effects, like updating the UI or playing a sound
            Debug.Log("Purchased card! New coins: " + GameData.coins + ", New Max Health: " + GameData.PlayerMaxHealth);

            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Not enough coins to purchase this card.");
        }

        // You can:
        // - Add to inventory
        // - Open a detail panel
        // - Highlight the card
        // - Trigger purchase logic
    }
}