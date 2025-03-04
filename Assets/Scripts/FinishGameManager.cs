using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishGameManager : MonoBehaviour
{
    public static FinishGameManager Instance;

    [SerializeField] private GameObject gameOverPanel;

    private void Awake()
    {
        Instance = this;
    }

    public void FinishGame()
    {
        Time.timeScale = 0;
        gameOverPanel.SetActive(true);
        PlayerMoney.Instance.SaveMoney();

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
}
