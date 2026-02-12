#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Reflection;

[InitializeOnLoad]
public static class AutoEnemySetup
{
    const string prefabPath = "Assets/Prefabs/Enemy.prefab";
    const string spritePath = "Assets/Textures/enemy_sprite.png";
    const string sampleScenePath = "Assets/Scenes/App.unity";

    static AutoEnemySetup()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        GameObject prefab = GetOrCreateEnemyPrefab();

        if (File.Exists(sampleScenePath) && prefab != null)
        {
            var scene = EditorSceneManager.OpenScene(sampleScenePath, OpenSceneMode.Single);

            GameObject existingSpawner = null;

            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == "EnemySpawner")
                {
                    existingSpawner = root;
                    break;
                }
            }

            if (existingSpawner == null)
            {
                GameObject spawner = new GameObject("EnemySpawner");
                var spawnerScript = spawner.AddComponent<EnemySpawner>();

                // Set private serialized field via reflection
                var prefabField = typeof(EnemySpawner)
                    .GetField("enemyPrefab", BindingFlags.NonPublic | BindingFlags.Instance);

                if (prefabField != null)
                {
                    prefabField.SetValue(spawnerScript, prefab);
                }

                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }
        }
    }

    // ✅ Only creates prefab if it doesn't exist
    static GameObject GetOrCreateEnemyPrefab()
    {
        GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (existingPrefab != null)
        {
            return existingPrefab;
        }

        Sprite enemySprite = GetOrCreateEnemySprite();

        Directory.CreateDirectory("Assets/Prefabs");

        GameObject enemy = new GameObject("Enemy");

        var sr = enemy.AddComponent<SpriteRenderer>();
        sr.sprite = enemySprite;
        sr.sortingOrder = 0;

        var rb = enemy.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        var collider = enemy.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;

        enemy.AddComponent<EnemyController>();

        var prefab = PrefabUtility.SaveAsPrefabAsset(enemy, prefabPath);

        Object.DestroyImmediate(enemy);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return prefab;
    }

    static Sprite GetOrCreateEnemySprite()
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        if (sprite != null)
            return sprite;

        Directory.CreateDirectory(Path.GetDirectoryName(spritePath));

        Texture2D tex = new Texture2D(16, 16);
        Color[] cols = new Color[16 * 16];

        for (int i = 0; i < cols.Length; i++)
            cols[i] = Color.red;

        tex.SetPixels(cols);
        tex.Apply();

        File.WriteAllBytes(spritePath, tex.EncodeToPNG());
        AssetDatabase.ImportAsset(spritePath);

        var ti = AssetImporter.GetAtPath(spritePath) as TextureImporter;
        if (ti != null)
        {
            ti.textureType = TextureImporterType.Sprite;
            ti.filterMode = FilterMode.Point;
            ti.spritePixelsPerUnit = 100;
            ti.SaveAndReimport();
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
    }
}
#endif
