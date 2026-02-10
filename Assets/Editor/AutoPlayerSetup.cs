using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
static class AutoPlayerSetup
{
    const string markerPath = "Assets/PlayerSetupDone.txt";
    const string sampleScenePath = "Assets/Scenes/SampleScene.unity";

    static AutoPlayerSetup()
    {
        // Delay execution until editor is ready
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (File.Exists(markerPath)) return;

        // Ensure prefab exists
        var prefab = PlayerSetupUtility.EnsurePlayerPrefabExists();

        // If SampleScene exists, open it and add the player instance if missing
        if (File.Exists(sampleScenePath) && prefab != null)
        {
            var scene = EditorSceneManager.OpenScene(sampleScenePath, OpenSceneMode.Single);

            bool hasPlayer = false;
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == "Player") { hasPlayer = true; break; }
            }

            if (!hasPlayer)
            {
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
                instance.name = "Player";
                instance.transform.position = Vector3.zero;
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }
        }

        // Write marker so we don't repeat setup
        File.WriteAllText(markerPath, "Player setup completed.");
        AssetDatabase.ImportAsset(markerPath);
        AssetDatabase.Refresh();
    }
}
