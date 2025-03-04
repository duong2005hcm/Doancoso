using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartButtonClicked()
    {
        SceneManager.LoadScene("Gameplay");
    }
}
