using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject itemPrefab; // Prefab của vật phẩm
    public Transform itemGrid; // Grid chứa vật phẩm (Content của ScrollView)
    public GameObject shopUI; // UI của Shop

    public TMP_Text coinText, diamondText; // Hiển thị tiền tệ
    public int coins = 1000, diamonds = 50; // Tiền người chơi

    public ItemDetailPanel detailPanel; // Panel chi tiết vật phẩm

    [Header("Danh Mục Vật Phẩm")]
    public ShopCategory currentCategory; // Danh mục đang chọn (Chỉnh trong Inspector)

    [Header("Danh Sách Vật Phẩm")]
    public List<ShopItem> allItems; // Danh sách tất cả vật phẩm

    private List<GameObject> spawnedItems = new List<GameObject>(); // Lưu trữ vật phẩm đã spawn

    private void Start()
    {
        // Đảm bảo Shop UI hiển thị khi mở Scene
        shopUI.SetActive(true);
        UpdateCurrencyUI();
        LoadShopItems();
    }

    // 🛒 Tải vật phẩm theo danh mục đã chọn
    public void LoadShopItems()
    {
        // Xóa các item cũ trước khi tạo lại
        foreach (GameObject item in spawnedItems)
        {
            Destroy(item);
        }
        spawnedItems.Clear();

        foreach (var item in allItems)
        {
            if (item.category == currentCategory) // Chỉ hiện vật phẩm thuộc danh mục chọn
            {
                GameObject newItem = Instantiate(itemPrefab, itemGrid);
                ShopItemUI itemUI = newItem.GetComponent<ShopItemUI>();
                itemUI.SetItem(item, this);
                spawnedItems.Add(newItem);
            }
        }
    }

    // 🛒 Mở chi tiết vật phẩm
    public void OpenDetailPanel(ShopItem item)
    {
        detailPanel.ShowItem(item, this);
    }

    // 💰 Cập nhật hiển thị tiền tệ
    public void UpdateCurrencyUI()
    {
        coinText.text = coins.ToString();
        diamondText.text = diamonds.ToString();
    }

    // 💰 Mua vật phẩm
    public bool PurchaseItem(ShopItem item, int quantity)
    {
        int totalPrice = item.price * quantity;
        if (item.currencyType == CurrencyType.Coin && coins >= totalPrice)
        {
            coins -= totalPrice;
            UpdateCurrencyUI();
            return true;
        }
        else if (item.currencyType == CurrencyType.Diamond && diamonds >= totalPrice)
        {
            diamonds -= totalPrice;
            UpdateCurrencyUI();
            return true;
        }
        return false;
    }

    // 📌 Cập nhật danh mục vật phẩm (Gọi khi ấn nút)
    public void ChangeCategory(ShopCategory newCategory)
    {
        currentCategory = newCategory;
        LoadShopItems(); // Cập nhật lại danh sách hiển thị
    }

    // 🎮 **Quay lại Main Menu khi ấn nút Return**
    public void CloseShop()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
