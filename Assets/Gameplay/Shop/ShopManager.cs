using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public TMP_Text goldText;

    public TMP_Text playerStatsText;


    void Start()
    {

    }

    void Update()
    {
        goldText.text = "Coins: " + GameData.coins;
        playerStatsText.text = "Max Health: " + GameData.PlayerMaxHealth;


    }

}