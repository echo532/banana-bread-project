using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
static class AutoUISetup
{
    const string sampleScenePath = "Assets/Scenes/App.unity";

    static AutoUISetup()
    {
        // Delay execution until editor is ready
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        // Don't run during play mode
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;

        // Set up UIManager in the scene
        if (File.Exists(sampleScenePath))
        {
            var scene = EditorSceneManager.OpenScene(sampleScenePath, OpenSceneMode.Single);

            // Remove existing UIManager if it exists
            GameObject existingUIManager = null;
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == "UIManager")
                {
                    existingUIManager = root;
                    break;
                }
            }

            if (existingUIManager != null)
            {
                Object.DestroyImmediate(existingUIManager);
            }

            // Create new UIManager GameObject
            GameObject uiManager = new GameObject("UIManager");
            uiManager.AddComponent<UIManager>();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            
            Debug.Log("UIManager added to scene");
        }
    }
}