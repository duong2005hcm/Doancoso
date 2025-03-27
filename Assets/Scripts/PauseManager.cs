using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseGamePanel;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject settingsPanel;

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
        SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);

    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void SetCanPause(bool value)
    {
        canPause = value;
    }
}
