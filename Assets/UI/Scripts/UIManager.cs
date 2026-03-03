using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Canvas canvas;
    private Image healthBarBackground;
    private Image healthBarFill;
    
    void Awake()
    {
        CreateCanvas();
        CreateHealthBar();
    }
    
    void CreateCanvas()
    {
        // Create canvas GameObject
        GameObject canvasObj = new GameObject("GameCanvas");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        // Add CanvasScaler for responsive UI
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        // Add GraphicRaycaster
        canvasObj.AddComponent<GraphicRaycaster>();
        
        Debug.Log("Canvas created");
    }
    
    void CreateHealthBar()
    {
        // Create a simple white sprite for the UI
        Texture2D whiteTexture = new Texture2D(1, 1);
        whiteTexture.SetPixel(0, 0, Color.white);
        whiteTexture.Apply();
        Sprite whiteSprite = Sprite.Create(whiteTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
        
        // Create health bar background
        GameObject bgObj = new GameObject("HealthBarBackground");
        bgObj.transform.SetParent(canvas.transform, false);
        
        RectTransform bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0.05f, 0.9f);
        bgRect.anchorMax = new Vector2(0.35f, 0.95f);
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        healthBarBackground = bgObj.AddComponent<Image>();
        healthBarBackground.sprite = whiteSprite;
        healthBarBackground.color = new Color(0.2f, 0.2f, 0.2f, 0.8f); // Dark gray
        
        // Create health bar fill
        GameObject fillObj = new GameObject("HealthBarFill");
        fillObj.transform.SetParent(bgObj.transform, false);
        
        RectTransform fillRect = fillObj.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = new Vector2(5, 5); // Padding
        fillRect.offsetMax = new Vector2(-5, -5);
        
        healthBarFill = fillObj.AddComponent<Image>();
        healthBarFill.sprite = whiteSprite;
        healthBarFill.color = new Color(0.2f, 0.8f, 0.2f, 1f); // Green
        healthBarFill.type = Image.Type.Filled;
        healthBarFill.fillMethod = Image.FillMethod.Horizontal;
        healthBarFill.fillOrigin = (int)Image.OriginHorizontal.Left;
        healthBarFill.fillAmount = 1f;
        
        Debug.Log($"Health bar fill setup: fillAmount={healthBarFill.fillAmount}, color={healthBarFill.color}");
        
        // Create health bar label
        GameObject labelObj = new GameObject("HealthBarLabel");
        labelObj.transform.SetParent(bgObj.transform, false);
        
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0);
        labelRect.anchorMax = new Vector2(1, 1);
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        
        Text labelText = labelObj.AddComponent<Text>();
        labelText.text = "HEALTH";
        labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        labelText.fontSize = 24;
        labelText.alignment = TextAnchor.MiddleCenter;
        labelText.color = Color.white;
        
        Debug.Log("Health bar created");
        
        // Connect to player's health system
        ConnectToPlayer();
    }
    
    void ConnectToPlayer()
    {
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            HealthSystem healthSystem = player.GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                healthSystem.SetHealthBarUI(healthBarFill);
                Debug.Log("Health bar connected to player");
            }
            else
            {
                Debug.LogWarning("Player doesn't have HealthSystem component! Retrying...");
                Invoke("ConnectToPlayer", 0.5f); // Retry after delay
            }
        }
        else
        {
            Debug.LogWarning("Player not found in scene! Retrying...");
            Invoke("ConnectToPlayer", 0.5f); // Retry after delay
        }
    }
}