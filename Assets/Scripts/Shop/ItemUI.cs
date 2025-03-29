using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemPriceText;
    [SerializeField] private Image currencyIcon;

    private string itemId;
    private string itemType;
    private int price;
    private string currency;
    private string description;

    public void Setup(string id, string name, string type, int itemPrice, string currencyType, string desc, Sprite image)
    {
        itemId = id;
        itemType = type;
        price = itemPrice;
        currency = currencyType;
        description = desc;

        itemImage.sprite = image;
        itemNameText.text = name;
        itemPriceText.text = price.ToString();

        currencyIcon.sprite = Resources.Load<Sprite>($"Images/Currency/{currency}");
    }
}
