using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMainMenu : MonoBehaviour
{
    public void OnReturnButtonClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
