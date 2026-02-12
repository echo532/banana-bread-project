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
    const string scenePath  = "Assets/Scenes/App.unity";

    static AutoEnemySetup()
    {
        string path = "Assets/Editor/debug.txt";

        if (File.Exists(path))
        {
            string searchString = "yes";

            
            string contents = File.ReadAllText(path);

            if (contents.Contains(searchString))
            {
                EditorApplication.delayCall += RunOnce;
            }
        }
        
    }

    static void RunOnce()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        var prefab = RebuildEnemyPrefab();   // ALWAYS rebuild
        if (prefab == null || !File.Exists(scenePath))
            return;

        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        var spawner = FindSpawner(scene);
        if (spawner == null)
        {
            spawner = new GameObject("EnemySpawner");
            spawner.AddComponent<EnemySpawner>();
        }

        EnsureSpawnerHasPrefab(spawner, prefab);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    // --------------------------------------------------
    // PREFAB REBUILD (GUID SAFE)
    // --------------------------------------------------

    static GameObject RebuildEnemyPrefab()
    {
        Directory.CreateDirectory("Assets/Prefabs");

        // Always regenerate sprite (overwrite PNG safely)
        var sprite = RegenerateEnemySprite();

        var existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (existingPrefab == null)
        {
            var newObj = BuildEnemyObject(sprite);
            var newPrefab = PrefabUtility.SaveAsPrefabAsset(newObj, prefabPath);
            Object.DestroyImmediate(newObj);
            AssetDatabase.SaveAssets();
            return newPrefab;
        }

        // Load existing prefab contents (keeps GUID)
        var prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);

        // Strip everything except Transform
        foreach (var comp in prefabRoot.GetComponents<Component>())
        {
            if (!(comp is Transform))
                Object.DestroyImmediate(comp);
        }

        foreach (Transform child in prefabRoot.transform)
            Object.DestroyImmediate(child.gameObject);

        // Rebuild structure
        ApplyEnemyComponents(prefabRoot, sprite);

        PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
        PrefabUtility.UnloadPrefabContents(prefabRoot);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
    }

    static GameObject BuildEnemyObject(Sprite sprite)
    {
        var enemy = new GameObject("Enemy");
        ApplyEnemyComponents(enemy, sprite);
        return enemy;
    }

    static void ApplyEnemyComponents(GameObject obj, Sprite sprite)
    {
        var sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = 0;

        var rb = obj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        var collider = obj.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;

        obj.AddComponent<EnemyController>();
    }

    // --------------------------------------------------
    // SPRITE REGENERATION (GUID SAFE)
    // --------------------------------------------------

    static Sprite RegenerateEnemySprite()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(spritePath));

        Texture2D tex = new Texture2D(16, 16);
        Color[] pixels = new Color[16 * 16];

        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.red; // ← change color here

        tex.SetPixels(pixels);
        tex.Apply();

        // Overwrite PNG file (does NOT touch .meta)
        File.WriteAllBytes(spritePath, tex.EncodeToPNG());
        AssetDatabase.ImportAsset(spritePath, ImportAssetOptions.ForceUpdate);

        var importer = AssetImporter.GetAtPath(spritePath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.filterMode = FilterMode.Point;
            importer.spritePixelsPerUnit = 100;
            importer.SaveAndReimport();
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
    }

    // --------------------------------------------------
    // SCENE HELPERS
    // --------------------------------------------------

    static GameObject FindSpawner(Scene scene)
    {
        foreach (var root in scene.GetRootGameObjects())
            if (root.name == "EnemySpawner")
                return root;

        return null;
    }

    static void EnsureSpawnerHasPrefab(GameObject spawner, GameObject prefab)
    {
        var spawnerScript = spawner.GetComponent<EnemySpawner>();
        if (spawnerScript == null)
            return;

        var field = typeof(EnemySpawner)
            .GetField("enemyPrefab", BindingFlags.NonPublic | BindingFlags.Instance);

        if (field != null)
        {
            field.SetValue(spawnerScript, prefab);
            EditorUtility.SetDirty(spawnerScript);
        }
    }
}
#endif
