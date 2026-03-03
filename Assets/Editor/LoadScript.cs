using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

[InitializeOnLoad]
public static class LoadScript
{

    [MenuItem("Tools/Build Ojects")]
    static void LoadAllScenes()
    {
        

        EditorApplication.delayCall += () =>
        {
            EditorSceneManager.OpenScene("Assets/Scenes/StartMenu.unity");
        };
        
    }

}