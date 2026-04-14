using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject characterSelectPanel;

    [Header("Buttons")]
    public Button startButton;
    public Button confirmButton;
    public Button charButton1;
    public Button charButton2;
    public Button charButton3;

    private int selectedCharacter = -1;

    // -------------------------
    // Methods called by buttons
    // -------------------------
    public void ShowCharacterSelect()
    {
        mainMenuPanel.SetActive(false);
        characterSelectPanel.SetActive(true);
    }

    public void SelectCharacter1() => SelectCharacter(1);
    public void SelectCharacter2() => SelectCharacter(2);
    public void SelectCharacter3() => SelectCharacter(3);

    private void SelectCharacter(int index)
    {
        selectedCharacter = index;
    }

    public void StartGame()
    {
        if (selectedCharacter == -1)
        {
            return;
        }

        // Replace this with your actual gameplay scene
        SceneManager.LoadScene("App");
    }
}
