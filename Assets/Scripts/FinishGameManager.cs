using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class FinishGameManager : MonoBehaviour
{
    public static FinishGameManager Instance;


    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI metersText;

    // Thêm Awake() để kiểm tra
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Tự động tìm nếu chưa gán reference
            if (gameOverPanel == null)
                gameOverPanel = GameObject.Find("GameOverPanel"); // Hoặc đường dẫn cụ thể

            if (metersText == null && gameOverPanel != null)
                metersText = gameOverPanel.GetComponentInChildren<TextMeshProUGUI>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FinishGame()
    {
        // Kiểm tra null trước khi sử dụng
        if (gameOverPanel == null)
        {
            Debug.LogError("GameOverPanel reference is missing!");
            return;
        }

        if (metersText == null)
        {
            Debug.LogError("MetersText reference is missing!");
            return;
        }

        Time.timeScale = 0;
        gameOverPanel.SetActive(true);

        if (PlayerMoney.Instance != null)
            PlayerMoney.Instance.SaveMoney();
        else
            Debug.LogWarning("PlayerMoney.Instance is null");

        if (MetersManager.Instance != null)
        {
            float meters = MetersManager.Instance.GetMetersTraveled();
            metersText.text = "Quãng đường đã chạy: " + (int)meters + " m";
        }
        else
        {
            Debug.LogWarning("MetersManager.Instance is null");
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
