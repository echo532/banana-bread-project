using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
static class AutoPlayerSetup
{
    const string sampleScenePath = "Assets/Scenes/App.unity";

    static AutoPlayerSetup()
    {
        // Delay execution until editor is ready
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {

        // Don't run during play mode
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        
        // Always recreate the player prefab from scratch
        var prefab = PlayerSetupUtility.RecreatePlayerPrefab();

        // If SampleScene exists, open it and replace the player instance
        if (File.Exists(sampleScenePath) && prefab != null)
        {
            var scene = EditorSceneManager.OpenScene(sampleScenePath, OpenSceneMode.Single);

            // Remove existing player instance if it exists
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == "Player")
                {
                    Object.DestroyImmediate(root);
                    break;
                }
            }

            // Add fresh player instance
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
            instance.name = "Player";
            instance.transform.position = Vector3.zero;
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }
    }
}