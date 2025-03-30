using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDetailManager : MonoBehaviour
{
    public static ItemDetailManager Instance;

    [SerializeField] private GameObject detailPanel;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemPriceText;
    [SerializeField] private Image currencyIcon;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Slider quantitySlider;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Button purchaseButton;

    private string itemId;
    private string itemType;
    private int price;
    private string currency;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void ShowDetails(string id, string name, string type, int itemPrice, string currencyType, string desc, Sprite image)
    {
        itemId = id;
        itemType = type;
        price = itemPrice;
        currency = currencyType;

        itemImage.sprite = image;
        itemNameText.text = name;
        itemPriceText.text = price.ToString();
        descriptionText.text = desc;
        currencyIcon.sprite = Resources.Load<Sprite>($"Images/Currency/{currency}");

        detailPanel.SetActive(true);

        bool isSupportItem = itemType == "supportItem";
        quantitySlider.gameObject.SetActive(isSupportItem);
        quantityText.gameObject.SetActive(isSupportItem);

        quantitySlider.value = 1;
        quantityText.text = "1";

        purchaseButton.onClick.RemoveAllListeners();
        purchaseButton.onClick.AddListener(() => PurchaseManager.Instance.TryPurchase(itemId, itemType, price, currency, (int)quantitySlider.value));
    }

    public void ClosePanel()
    {
        detailPanel.SetActive(false);
    }
}
