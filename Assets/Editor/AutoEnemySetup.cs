using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
static class AutoEnemySetup
{
    const string sampleScenePath = "Assets/Scenes/SampleScene.unity";

    static AutoEnemySetup()
    {
        // Delay execution until editor is ready
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        // Don't run during play mode
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;

        // Always recreate the enemy prefab from scratch
        var prefab = EnemySetupUtility.RecreateEnemyPrefab();
        
        // Set up EnemySpawner in the scene
        if (File.Exists(sampleScenePath) && prefab != null)
        {
            var scene = EditorSceneManager.OpenScene(sampleScenePath, OpenSceneMode.Single);

            // Remove existing Enemy GameObjects (from old script versions)
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == "Enemy" || root.name.StartsWith("Enemy_"))
                {
                    Object.DestroyImmediate(root);
                }
            }

            // Remove existing EnemySpawner if it exists
            GameObject existingSpawner = null;
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == "EnemySpawner")
                {
                    existingSpawner = root;
                    break;
                }
            }

            if (existingSpawner != null)
            {
                Object.DestroyImmediate(existingSpawner);
            }

            // Create new EnemySpawner GameObject
            GameObject spawner = new GameObject("EnemySpawner");
            var spawnerScript = spawner.AddComponent<EnemySpawner>();
            
            // Use reflection to set the private serialized field
            var spawnerType = typeof(EnemySpawner);
            var prefabField = spawnerType.GetField("enemyPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (prefabField != null)
            {
                prefabField.SetValue(spawnerScript, prefab);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }
    }
}