using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

[InitializeOnLoad]
public static class LoadScript
{
    
    static LoadScript()
    {
        string path = "Assets/Editor/debug.txt";
        if (File.Exists(path))
        {
            string contents = File.ReadAllText(path);
            if (contents.Contains("yes"))
            {
                EditorApplication.delayCall += LoadAllScenes;
            }
        }
    }

    [MenuItem("Tools/Generate Start Menu")]
    static void LoadAllScenes()
    {
        //Menu Scene
        MenuSetup.GenerateMenu();
        //Scene 1
        AutoUISetup.RunOnce();
        AutoPlayerSetup.RunOnce();
        AutoEnemySetup.RunOnce();
        
    }

}