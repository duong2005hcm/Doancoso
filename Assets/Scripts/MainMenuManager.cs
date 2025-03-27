using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button StartGameButton; // Nút bắt đầu trò chơi
    public Button ShopButton; // Nút vào cửa hàng (nếu có)

    private void Start()
    {
        // Gán sự kiện khi bấm nút
        StartGameButton.onClick.AddListener(StartButtonClicked);

        if (ShopButton != null) // Kiểm tra nếu có nút Shop
        {
            ShopButton.onClick.AddListener(ShopButtonClicked);
        }
    }

    private void StartButtonClicked()
    {
        Debug.Log("Bắt đầu game!");
        SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
    }

    private void ShopButtonClicked()
    {
        Debug.Log("Vào Shop!");
        SceneManager.LoadScene("Shop", LoadSceneMode.Single);
    }
}
