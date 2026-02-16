#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public static class AutoUISetup
{
    const string sampleScenePath = "Assets/Scenes/SampleScene.unity";

    public static void RunOnce()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        if (!File.Exists(sampleScenePath))
            return;

        var scene = EditorSceneManager.OpenScene(sampleScenePath, OpenSceneMode.Single);

        bool uiManagerExists = false;

        foreach (var root in scene.GetRootGameObjects())
        {
            if (root.name == "UIManager")
            {
                uiManagerExists = true;
                break;
            }
        }

        // ✅ Only create if missing
        if (!uiManagerExists)
        {
            GameObject uiManager = new GameObject("UIManager");
            uiManager.AddComponent<UIManager>();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            Debug.Log("UIManager created.");
        }
    }
}
#endif
