using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;


public static class LoadScript
{

    [MenuItem("Tools/Build Ojects")]
    static void LoadAllScenes()
    {
        //Menu Scene
        MenuSetup.GenerateMenu();
        //Scene 1
        AutoUISetup.RunOnce();
        AutoPlayerSetup.RunOnce();
        AutoEnemySetup.RunOnce();

        EditorApplication.delayCall += () =>
        {
            EditorSceneManager.OpenScene("Assets/Scenes/StartMenu.unity");
        };
        
    }

}