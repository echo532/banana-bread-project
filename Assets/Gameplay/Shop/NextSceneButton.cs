using UnityEngine;
using UnityEngine.SceneManagement;
public class UIActions : MonoBehaviour
{
    public void NextRound()
    {
        Debug.Log("Button clicked!");
        SceneManager.LoadScene("App");
    }
}