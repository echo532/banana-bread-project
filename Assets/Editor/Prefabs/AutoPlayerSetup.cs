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

        RecreateOrUpdatePlayerPrefab();
    }

    // ✅ Always overwrite PNG (GUID preserved)
    static Sprite GetOrCreatePlayerSprite()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(spritePath));

        Texture2D tex = new Texture2D(16, 16);
        Color fill = Color.white; // change this and it WILL update
        Color[] cols = new Color[16 * 16];

        for (int i = 0; i < cols.Length; i++)
            cols[i] = fill;

        tex.SetPixels(cols);
        tex.Apply();

        // Overwrite PNG WITHOUT deleting asset (.meta preserved)
        File.WriteAllBytes(spritePath, tex.EncodeToPNG());

        AssetDatabase.ImportAsset(
            spritePath,
            ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport
        );

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

    // ✅ Safe prefab create/update (no deletion, GUID preserved)
    static GameObject RecreateOrUpdatePlayerPrefab()
    {
        Directory.CreateDirectory("Assets/Prefabs");

        Sprite playerSprite = GetOrCreatePlayerSprite();

        GameObject existingPrefab =
            AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        // Create once if missing
        if (existingPrefab == null)
        {
            GameObject player = CreatePlayerObject(playerSprite);
            var newPrefab = PrefabUtility.SaveAsPrefabAsset(player, prefabPath);
            Object.DestroyImmediate(player);
            AssetDatabase.SaveAssets();
            return newPrefab;
        }

        // Modify in place (preserves component fileIDs)
        var prefabContents = PrefabUtility.LoadPrefabContents(prefabPath);

        var sr = prefabContents.GetComponent<SpriteRenderer>();
        if (sr == null) sr = prefabContents.AddComponent<SpriteRenderer>();
        sr.sprite = playerSprite;
        sr.sortingOrder = 0;

        var rb = prefabContents.GetComponent<Rigidbody2D>();
        if (rb == null) rb = prefabContents.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (prefabContents.GetComponent<BoxCollider2D>() == null)
            prefabContents.AddComponent<BoxCollider2D>();

        if (prefabContents.GetComponent<PlayerController>() == null)
            prefabContents.AddComponent<PlayerController>();

        if (prefabContents.GetComponent<HealthSystem>() == null)
            prefabContents.AddComponent<HealthSystem>();

        if (prefabContents.GetComponent<PlayerCollisionHandler>() == null)
            prefabContents.AddComponent<PlayerCollisionHandler>();

        PrefabUtility.SaveAsPrefabAsset(prefabContents, prefabPath);
        PrefabUtility.UnloadPrefabContents(prefabContents);

        AssetDatabase.SaveAssets();

        return AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
    }

    static GameObject CreatePlayerObject(Sprite sprite)
    {
        GameObject player = new GameObject("Player");

        var sr = player.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = 0;

        var rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        player.AddComponent<BoxCollider2D>();
        player.AddComponent<PlayerController>();
        player.AddComponent<HealthSystem>();
        player.AddComponent<PlayerCollisionHandler>();

        return player;
    }
}
#endif
