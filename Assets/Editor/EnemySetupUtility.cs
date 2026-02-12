using UnityEngine;
using UnityEditor;
using System.IO;

public static class EnemySetupUtility
{
    const string spritePath =  EditorAssetUtility.GeneratedTextures + "/enemy_sprite.png"; // TODO: Replace with your enemy texture path

    const string prefabPath = EditorAssetUtility.GeneratedPrefabs + "/Enemy.prefab";

    public static Sprite GetOrCreateEnemySprite()
    {
        if (!File.Exists(spritePath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(spritePath));
            Texture2D tex = new Texture2D(16, 16);
            Color fill = Color.red; // Default red color for enemy
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

    // Recreates the Enemy prefab from scratch at Assets/Prefabs/Enemy.prefab.
    public static GameObject RecreateEnemyPrefab()
    {
        

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

        Sprite enemySprite = GetOrCreateEnemySprite();

        Directory.CreateDirectory(EditorAssetUtility.GeneratedPrefabs);

        GameObject enemy = new GameObject("Enemy");
        var sr = enemy.AddComponent<SpriteRenderer>();
        sr.sprite = enemySprite;
        sr.sortingOrder = 0;

        var rb = enemy.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic; // Prevent physics collisions
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        var collider = enemy.AddComponent<BoxCollider2D>();
        collider.isTrigger = true; // Make it a trigger instead of solid collision
        enemy.AddComponent<EnemyController>();

        var prefab = PrefabUtility.SaveAsPrefabAsset(enemy, prefabPath);
        Object.DestroyImmediate(enemy);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return prefab as GameObject;
    }
}