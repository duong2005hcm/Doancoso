using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    public Image itemIcon;
    public TMP_Text itemNameText;
    public TMP_Text priceText;
    public Image currencyIcon;
    public Button itemButton;

    public Sprite coinIcon;
    public Sprite diamondIcon;

    private ShopItem itemData;
    private ShopManager shopManager;

    public void SetItem(ShopItem item, ShopManager manager)
    {
        itemData = item;
        shopManager = manager;

        itemNameText.text = item.itemName;
        itemIcon.sprite = item.itemIcon;
        priceText.text = item.price.ToString();

        currencyIcon.sprite = (item.currencyType == CurrencyType.Coin) ? coinIcon : diamondIcon;

        // Gán sự kiện click vào nút
        itemButton.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        shopManager.OpenDetailPanel(itemData);
    }
}
