using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDetailManager : MonoBehaviour
{
    public static ItemDetailManager Instance;

    public GameObject itemDetailPanel;
    public Image itemImage;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemPrice;
    public Image currencyIcon;
    public TextMeshProUGUI itemDescription;
    public Slider quantitySlider;
    public TextMeshProUGUI quantityText;
    public Button buyButton;
    public Button returnButton;

    private int pricePerItem;
    private string currencyType;

    private void Awake()
    {
        Instance = this;
        itemDetailPanel.SetActive(false);
    }

    private void Start()
    {
        returnButton.onClick.AddListener(CloseItemDetailPanel);
        buyButton.onClick.AddListener(BuyItem);
        quantitySlider.onValueChanged.AddListener(UpdateQuantity);
    }

    public void ShowItemDetails(string id, string name, int price, string currency, string imageName)
    {
        itemName.text = name;
        itemPrice.text = price.ToString();
        currencyIcon.sprite = LoadCurrencyIcon(currency);
        itemImage.sprite = Resources.Load<Sprite>("Images/Items/" + imageName);

        pricePerItem = price;
        currencyType = currency;

        quantitySlider.value = 1;
        UpdateQuantity(1);

        itemDetailPanel.SetActive(true);
    }

    private void UpdateQuantity(float value)
    {
        quantityText.text = value.ToString();
        int totalPrice = Mathf.RoundToInt(value * pricePerItem);
        itemPrice.text = totalPrice.ToString();
    }

    private void BuyItem()
    {
        int quantity = (int)quantitySlider.value;
        int totalPrice = quantity * pricePerItem;
        //PurchaseManager.Instance.TryPurchase(currencyType, totalPrice);
    }

    private void CloseItemDetailPanel()
    {
        itemDetailPanel.SetActive(false);
    }

    private Sprite LoadCurrencyIcon(string currency)
    {
        return Resources.Load<Sprite>("Icons/" + currency);
    }
}