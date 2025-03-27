using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [SerializeField] private TextMeshProUGUI coinText;  // Text để hiển thị số coin
    [SerializeField] private Image coinIcon;           // Icon của coin

    private int currentCoins = 0; // Số coin thu thập được trong màn chơi

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateCoinUI();
    }

    // Gọi hàm này khi người chơi thu thập coin
    public void AddCoin(int amount)
    {
        currentCoins += amount;
        UpdateCoinUI();
    }

    // Cập nhật UI
    private void UpdateCoinUI()
    {
        coinText.text = currentCoins.ToString();
    }

    // Lấy số coin trong màn chơi (có thể dùng khi lưu vào database)
    public int GetCollectedCoins()
    {
        return currentCoins;
    }
}
