using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.SceneManagement;
using UnityEngine.InputSystem.UI;
using UnityEditor.Events; // For persistent listeners

public static class MenuSetup
{
    static MenuSetup(){
        EditorApplication.delayCall += GenerateMenu;
    }
    public static void GenerateMenu()
    {
        // -------------------------
        // Create a new scene
        // -------------------------
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);

        // Ensure folder exists
        if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            AssetDatabase.CreateFolder("Assets", "Scenes");

        // -------------------------
        // Canvas
        // -------------------------
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<GraphicRaycaster>();

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        // -------------------------
        // EventSystem (Modern Input System)
        // -------------------------
        GameObject eventSystemGO = new GameObject("EventSystem");
        eventSystemGO.AddComponent<EventSystem>();

        var inputModule = eventSystemGO.AddComponent<InputSystemUIInputModule>();

        // -------------------------
        // Menu Controller
        // -------------------------
        GameObject controllerGO = new GameObject("MenuController");
        StartMenuController controller = controllerGO.AddComponent<StartMenuController>();

        // -------------------------
        // Main Menu Panel
        // -------------------------
        GameObject mainPanel = CreatePanel("MainMenuPanel", canvasGO.transform);
        mainPanel.SetActive(true);

        GameObject startButtonGO = CreateButton("Start Game", mainPanel.transform, Vector2.zero);
        Button startButton = startButtonGO.GetComponent<Button>();

        // -------------------------
        // Character Select Panel
        // -------------------------
        GameObject charPanel = CreatePanel("CharacterSelectPanel", canvasGO.transform);
        charPanel.SetActive(false);

        float y = 100f;
        GameObject charButton1GO = CreateButton("Character 1", charPanel.transform, new Vector2(0, y));
        GameObject charButton2GO = CreateButton("Character 2", charPanel.transform, new Vector2(0, y - 80));
        GameObject charButton3GO = CreateButton("Character 3", charPanel.transform, new Vector2(0, y - 160));

        Button charButton1 = charButton1GO.GetComponent<Button>();
        Button charButton2 = charButton2GO.GetComponent<Button>();
        Button charButton3 = charButton3GO.GetComponent<Button>();

        GameObject confirmButtonGO = CreateButton("Start Game", charPanel.transform, new Vector2(0, -150));
        Button confirmButton = confirmButtonGO.GetComponent<Button>();

        // -------------------------
        // Assign References
        // -------------------------
        controller.mainMenuPanel = mainPanel;
        controller.characterSelectPanel = charPanel;
        controller.startButton = startButton;
        controller.confirmButton = confirmButton;
        controller.charButton1 = charButton1;
        controller.charButton2 = charButton2;
        controller.charButton3 = charButton3;

        // -------------------------
        // Add persistent listeners (Editor-time)
        // -------------------------
        UnityEventTools.AddPersistentListener(startButton.onClick, controller.ShowCharacterSelect);

        UnityEventTools.AddPersistentListener(charButton1.onClick, controller.SelectCharacter1);
        UnityEventTools.AddPersistentListener(charButton2.onClick, controller.SelectCharacter2);
        UnityEventTools.AddPersistentListener(charButton3.onClick, controller.SelectCharacter3);

        UnityEventTools.AddPersistentListener(confirmButton.onClick, controller.StartGame);

        // Mark buttons dirty so Unity serializes the listeners
        EditorUtility.SetDirty(startButton);
        EditorUtility.SetDirty(charButton1);
        EditorUtility.SetDirty(charButton2);
        EditorUtility.SetDirty(charButton3);
        EditorUtility.SetDirty(confirmButton);

        // -------------------------
        // Save Scene
        // -------------------------
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/StartMenu.unity");
    }

    // -------------------------
    // Helper Functions
    // -------------------------
    static GameObject CreatePanel(string name, Transform parent)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image img = panel.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0.6f);

        return panel;
    }

    static GameObject CreateButton(string text, Transform parent, Vector2 position)
    {
        GameObject buttonGO = new GameObject(text + " Button");
        buttonGO.transform.SetParent(parent, false);

        RectTransform rect = buttonGO.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 50);
        rect.anchoredPosition = position;

        Image img = buttonGO.AddComponent<Image>();
        img.color = Color.white;

        Button btn = buttonGO.AddComponent<Button>();
        btn.targetGraphic = img;

        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);

        RectTransform textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text txt = textGO.AddComponent<Text>();
        txt.text = text;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.black;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        return buttonGO;
    }
}
