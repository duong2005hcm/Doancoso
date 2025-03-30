using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class FinishGameManager : MonoBehaviour
{
    public static FinishGameManager Instance;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI metersText;
    [SerializeField] private TextMeshProUGUI coinText;

    private void Awake()
    {
        Instance = this;
    }

    public void FinishGame()
    {
        Time.timeScale = 0;
        gameOverPanel.SetActive(true);
        PlayerMoney.Instance.SaveMoney();

        if (MetersManager.Instance != null)
        {
            float meters = MetersManager.Instance.GetMetersTraveled();
            metersText.text = "Quãng đường đã chạy: " + (int)meters + " m";
        }

        if (coinText != null)
        {
            int collectedCoins = PlayerMoney.Instance.GetCollectedCoins();
            coinText.text = "Xu đã thu thập: " + collectedCoins;
        }
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
}
