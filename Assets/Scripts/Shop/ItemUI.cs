using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemPriceText;
    [SerializeField] private Image currencyIcon;
    [SerializeField] private Button itemButton;

    private string itemId;
    private string itemType;
    private int price;
    private string currency;
    private string description;
    private Sprite itemSprite;

    public void Setup(string id, string name, string type, int itemPrice, string currencyType, string desc, Sprite image)
    {
        itemId = id;
        itemType = type;
        price = itemPrice;
        currency = currencyType;
        description = desc;
        itemSprite = image;

        itemNameText.text = name;
        itemPriceText.text = price.ToString();
        itemImage.sprite = image;
        currencyIcon.sprite = Resources.Load<Sprite>($"Images/Currency/{currency}");

        itemButton.onClick.RemoveAllListeners();
        itemButton.onClick.AddListener(ShowItemDetails);
    }

    private void ShowItemDetails()
    {
        ItemDetailManager.Instance.ShowDetails(itemId, itemNameText.text, itemType, price, currency, description, itemSprite);
    }
}
