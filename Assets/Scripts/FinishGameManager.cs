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
        CoinManager.Instance.SaveMoney();

        if (MetersManager.Instance != null)
        {
            float meters = MetersManager.Instance.GetMetersTraveled();
            metersText.text = "Quãng đường đã chạy: " + (int)meters + " m";
        }

        if (coinText != null)
        {
            int collectedCoins = CoinManager.Instance.GetCollectedCoinsThisRun();
            coinText.text = "Xu đã thu thập: " + collectedCoins;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        CoinManager.Instance.ResetCoins();
        SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        CoinManager.Instance.ResetCoins();
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
