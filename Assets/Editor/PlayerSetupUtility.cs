using UnityEngine;
using UnityEditor;
using System.IO;

public static class PlayerSetupUtility
{
    const string spritePath = "Assets/Textures/player_square.png";

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

    // Ensures a Player prefab asset exists at Assets/Prefabs/Player.prefab and returns it.
    public static GameObject EnsurePlayerPrefabExists()
    {
        const string prefabPath = "Assets/Prefabs/Player.prefab";

        Sprite playerSprite = GetOrCreatePlayerSprite();

        if (File.Exists(prefabPath))
        {
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (existing != null) return existing;
        }

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

        var prefab = PrefabUtility.SaveAsPrefabAsset(player, prefabPath);
        Object.DestroyImmediate(player);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return prefab as GameObject;
    }
}
