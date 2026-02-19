#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public static class AutoUISetup
{
    const string sampleScenePath = "Assets/Scenes/App.unity";

    public static void RunOnce()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        Scene scene;

        // --------------------------------------------------
        // CREATE SCENE IF MISSING
        // --------------------------------------------------
        if (!File.Exists(sampleScenePath))
        {
            Directory.CreateDirectory("Assets/Scenes");

            scene = EditorSceneManager.NewScene(
                NewSceneSetup.DefaultGameObjects,
                NewSceneMode.Single
            );

            EditorSceneManager.SaveScene(scene, sampleScenePath);
            Debug.Log("Scene created at " + sampleScenePath);
        }
        else
        {
            scene = EditorSceneManager.OpenScene(
                sampleScenePath,
                OpenSceneMode.Single
            );
        }

        // --------------------------------------------------
        // ENSURE UIManager EXISTS
        // --------------------------------------------------
        bool uiManagerExists = false;

        foreach (var root in scene.GetRootGameObjects())
        {
            if (root.name == "UIManager")
            {
                uiManagerExists = true;
                break;
            }
        }

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
