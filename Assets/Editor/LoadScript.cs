using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

[InitializeOnLoad]
public static class LoadScript
{

    [MenuItem("Tools/Build Objects")]
    static void LoadAllScenes()
    {
        

        EditorApplication.delayCall += () =>
        {
            EditorSceneManager.OpenScene("Assets/Scenes/StartMenu.unity");
        };
        
    }

    [MenuItem("Tools/Set Starting Weapon")]
    static void SetStartingWeapon()
    {
        // Get the PlayerController in the scene
        PlayerController player = GameObject.FindFirstObjectByType<PlayerController>();
        if (player == null)
        {
            Debug.LogWarning("No PlayerController found in the scene.");
            return;
        }

        // Example: set the first weapon active
        player.EquipWeapon(0);

        // Mark the scene dirty so it saves the change
        EditorSceneManager.MarkSceneDirty(player.gameObject.scene);
        Debug.Log("Starting weapon set to index 0");
    }

}