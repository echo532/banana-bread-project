using UnityEngine;
using UnityEditor;
using System.IO;

public static class PlayerSetupUtility
{
    const string spritePath = EditorAssetUtility.GeneratedTextures + "player_square.png";

    const string prefabPath = EditorAssetUtility.GeneratedPrefabs + "Player.prefab";

    public static Sprite GetOrCreatePlayerSprite()
    {
        if (!File.Exists(spritePath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(spritePath));
            Texture2D tex = new Texture2D(16, 16);
            Color fill = Color.white;
            Color[] cols = new Color[16 * 16];
            for (int i = 0; i < cols.Length; i++) cols[i] = fill;
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
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
    }
    

    // Recreates the Player prefab from scratch at Assets/Prefabs/Player.prefab.
    public static GameObject RecreatePlayerPrefab()
    {

        EditorAssetUtility.EnsureFolder(EditorAssetUtility.GeneratedPrefabs);

        // Delete existing prefab if it exists
        if (File.Exists(prefabPath))
        {
            AssetDatabase.DeleteAsset(prefabPath);
        }

        // Delete and recreate sprite
        if (File.Exists(spritePath))
        {
            AssetDatabase.DeleteAsset(spritePath);
        }

        Sprite playerSprite = GetOrCreatePlayerSprite();

        Directory.CreateDirectory("Assets/Prefabs");

        GameObject player = new GameObject("Player");
        var sr = player.AddComponent<SpriteRenderer>();
        sr.sprite = playerSprite;
        sr.sortingOrder = 0;

        var rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        player.AddComponent<BoxCollider2D>();
        player.AddComponent<PlayerController>();
        player.AddComponent<HealthSystem>();
        player.AddComponent<PlayerCollisionHandler>();

        var prefab = PrefabUtility.SaveAsPrefabAsset(player, prefabPath);
        Object.DestroyImmediate(player);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return prefab as GameObject;
    }
}