#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;
using System.IO;

[InitializeOnLoad]
public static class AutoPlayerSetup
{
    const string spritePath = "Assets/Textures/player_square.png";
    const string prefabPath = "Assets/Prefabs/Player.prefab";

    static AutoPlayerSetup()
    {
        string path = "Assets/Editor/debug.txt";
        if (File.Exists(path))
        {
            string contents = File.ReadAllText(path);
            if (contents.Contains("yes"))
            {
                EditorApplication.delayCall += RunOnce;
            }
        }
    }

    static void RunOnce()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        RecreateOrUpdatePlayerPrefab();
    }

    // --------------------------------------------------
    // SPRITE REGENERATION (GUID SAFE)
    // --------------------------------------------------
    static Sprite RegeneratePlayerSprite()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(spritePath));

        Texture2D tex = new Texture2D(16, 16);
        Color[] pixels = new Color[16 * 16];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.white; // ← change this color to update sprite

        tex.SetPixels(pixels);
        tex.Apply();

        // Overwrite PNG without touching .meta
        File.WriteAllBytes(spritePath, tex.EncodeToPNG());
        AssetDatabase.ImportAsset(spritePath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);

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
    // PREFAB REBUILD / UPDATE (FILEID SAFE)
    // --------------------------------------------------
    static GameObject RecreateOrUpdatePlayerPrefab()
    {
        Directory.CreateDirectory("Assets/Prefabs");

        Sprite playerSprite = RegeneratePlayerSprite();
        GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        // If prefab doesn't exist → create it once
        if (existingPrefab == null)
        {
            GameObject player = BuildPlayerObject(playerSprite);
            GameObject newPrefab = PrefabUtility.SaveAsPrefabAsset(player, prefabPath);
            Object.DestroyImmediate(player);
            AssetDatabase.SaveAssets();
            return newPrefab;
        }

        // If prefab exists → update components safely
        var prefabContents = PrefabUtility.LoadPrefabContents(prefabPath);

        UpdateOrAddPlayerComponents(prefabContents, playerSprite);

        PrefabUtility.SaveAsPrefabAsset(prefabContents, prefabPath);
        PrefabUtility.UnloadPrefabContents(prefabContents);
        AssetDatabase.SaveAssets();

        return AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
    }

    // --------------------------------------------------
    // BUILD / UPDATE PLAYER GAMEOBJECT
    // --------------------------------------------------
    static GameObject BuildPlayerObject(Sprite sprite)
    {
        GameObject player = new GameObject("Player");
        UpdateOrAddPlayerComponents(player, sprite);
        return player;
    }

    static void UpdateOrAddPlayerComponents(GameObject obj, Sprite sprite)
    {
        // SpriteRenderer
        var sr = obj.GetComponent<SpriteRenderer>();
        if (sr == null) sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = 0;

        // Rigidbody2D
        var rb = obj.GetComponent<Rigidbody2D>();
        if (rb == null) rb = obj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // BoxCollider2D
        if (obj.GetComponent<BoxCollider2D>() == null)
            obj.AddComponent<BoxCollider2D>();

        // PlayerController
        if (obj.GetComponent<PlayerController>() == null)
            obj.AddComponent<PlayerController>();

        // HealthSystem
        if (obj.GetComponent<HealthSystem>() == null)
            obj.AddComponent<HealthSystem>();

        // PlayerCollisionHandler
        if (obj.GetComponent<PlayerCollisionHandler>() == null)
            obj.AddComponent<PlayerCollisionHandler>();
    }
}
#endif
