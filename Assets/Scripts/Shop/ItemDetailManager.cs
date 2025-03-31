using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDetailManager : MonoBehaviour
{
    public static ItemDetailManager Instance;

    [SerializeField] private GameObject detailPanel;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI originalPriceText; // 🔹 Thêm hiển thị giá gốc
    [SerializeField] private TextMeshProUGUI itemPriceText;
    [SerializeField] private Image currencyIcon;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Slider quantitySlider;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private Image purchaseCurrencyIcon;
    [SerializeField] private TextMeshProUGUI purchasePriceText;
    [SerializeField] private Button returnButton;

    private string itemId;
    private string itemType;
    private int price;
    private string currency;
    private bool isSupportItem;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        if (returnButton != null)
            returnButton.onClick.AddListener(ClosePanel);
        else
            Debug.LogError("❌ returnButton bị MISSING trong Inspector!");

        if (quantitySlider != null)
            quantitySlider.onValueChanged.AddListener(UpdateQuantity);
        else
            Debug.LogError("❌ quantitySlider bị MISSING trong Inspector!");

        detailPanel.SetActive(false);
    }

    public void ShowDetails(string id, string name, string type, int itemPrice, string currencyType, string desc, Sprite image)
    {
        Debug.Log($"📌 Hiển thị chi tiết: {name}");

        itemId = id;
        itemType = type;
        price = itemPrice;
        currency = currencyType;
        isSupportItem = itemType == "supportItem";

        // Cập nhật UI
        itemImage.sprite = image;
        itemNameText.text = name;
        descriptionText.text = desc;

        // Hiển thị giá gốc (giá của 1 vật phẩm)
        originalPriceText.text = price.ToString(); // 🔹 Giá gốc

        // Load ảnh currency
        Sprite currencySprite = Resources.Load<Sprite>($"Images/Currency/{currency}");
        if (currencySprite != null)
        {
            currencyIcon.sprite = currencySprite;
            purchaseCurrencyIcon.sprite = currencySprite;
        }
        else
        {
            Debug.LogError($"❌ Không tìm thấy ảnh currency: {currency}");
        }

        // Hiển thị panel
        detailPanel.SetActive(true);

        // Hiển thị slider nếu là vật phẩm hỗ trợ
        quantitySlider.gameObject.SetActive(isSupportItem);
        quantityText.gameObject.SetActive(isSupportItem);

        // Đặt giá trị ban đầu cho slider
        quantitySlider.value = 1;
        UpdateQuantity(1);

        // Gán sự kiện cho nút Mua
        purchaseButton.onClick.RemoveAllListeners();
        purchaseButton.onClick.AddListener(() =>
        {
            int quantity = isSupportItem ? (int)quantitySlider.value : 1;
            PurchaseManager.Instance.TryPurchase(itemId, itemType, price, currency, quantity);
        });
    }

    private void UpdateQuantity(float value)
    {
        int quantity = (int)value;
        quantityText.text = quantity.ToString();
        purchasePriceText.text = (price * quantity).ToString();
    }

    public void ClosePanel()
    {
        Debug.Log("📌 Đóng panel chi tiết.");
        detailPanel.SetActive(false);
    }
}
