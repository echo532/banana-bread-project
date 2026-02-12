using UnityEditor;

public static class EditorAssetUtility
{
    public const string Root = "Assets/";


    public const string GeneratedPrefabs = Root + "/Prefabs/Generated/";
    public const string GeneratedTextures = Root + "/Textures/Generated/";
    public const string GeneratedScriptableObjects = Root + "/ScriptableObjects/Generated/";

    public const string GeneratedScenes = Root + "/Scenes/Generated/";

    public static string GeneratedPrefab(string name)
        => $"{GeneratedPrefabs}/{name}.prefab";

    public static string GeneratedTexture(string name)
        => $"{GeneratedTextures}/{name}.png";

    public static string GeneratedAsset(string name)
        => $"{GeneratedScriptableObjects}/{name}.asset";

    public static void EnsureFolder(string fullPath)
    {
        if (AssetDatabase.IsValidFolder(fullPath))
            return;

        string[] parts = fullPath.Split('/');
        string currentPath = parts[0];

        for (int i = 1; i < parts.Length; i++)
        {
            string nextPath = currentPath + "/" + parts[i];

            if (!AssetDatabase.IsValidFolder(nextPath))
            {
                AssetDatabase.CreateFolder(currentPath, parts[i]);
            }

            currentPath = nextPath;
        }
    }

    public static void DeleteFolderIfExists(string fullPath)
    {
        if (AssetDatabase.IsValidFolder(fullPath))
        {
            AssetDatabase.DeleteAsset(fullPath);
        }
    }
}
