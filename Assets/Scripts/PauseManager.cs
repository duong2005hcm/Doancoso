using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseGamePanel;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private bool canPause = true;



    private bool isGamePaused = false;

    public void TogglePause()
    {
        if (!canPause)
            return;

        isGamePaused = !isGamePaused;

        if (isGamePaused)
        {
            Time.timeScale = 0;
            pauseGamePanel.SetActive(true);
            pauseButton.SetActive(false);
        }
        else
        {
            ResumeGame();
        }

    }

    public void ResumeGame()
    {
        Debug.Log("Continue button clicked!");

        isGamePaused = false;
        Time.timeScale = 1;
        pauseGamePanel.SetActive(false);
        pauseButton.SetActive(true);

    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Gameplay");
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenSettings()
    {
        Debug.Log("Open Settings");
    }

    public void SetCanPause(bool value)
    {
        canPause = value;
    }
}
