using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDetailPanel : MonoBehaviour
{
    public Image itemIcon;
    public TMP_Text itemNameText;
    public TMP_Text descriptionText;
    public TMP_Text priceText;
    public Image currencyIcon;
    public TMP_Text totalPriceText;
    public Slider quantitySlider;
    public TMP_Text quantityText;

    public Sprite coinIcon;
    public Sprite diamondIcon;

    private ShopItem currentItem;
    private ShopManager shopManager;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void ShowItem(ShopItem item, ShopManager manager)
    {
        currentItem = item;
        shopManager = manager;

        itemIcon.sprite = item.itemIcon;
        itemNameText.text = item.itemName;
        descriptionText.text = item.description;
        priceText.text = item.price.ToString();
        currencyIcon.sprite = (item.currencyType == CurrencyType.Coin) ? coinIcon : diamondIcon;

        quantitySlider.value = 1;
        UpdateTotalPrice();

        gameObject.SetActive(true);
    }

    public void UpdateTotalPrice()
    {
        int quantity = (int)quantitySlider.value;
        int totalPrice = quantity * currentItem.price;
        totalPriceText.text = totalPrice.ToString();
        quantityText.text = quantity.ToString();
    }

    public void Purchase()
    {
        int quantity = (int)quantitySlider.value;
        bool success = shopManager.PurchaseItem(currentItem, quantity);
        if (success)
        {
            gameObject.SetActive(false);
        }
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}
