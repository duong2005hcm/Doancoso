using UnityEngine;

public enum CurrencyType { Coin, Diamond } // Loại tiền tệ

[System.Serializable]
public class ShopItem
{
    public string itemName;
    public Sprite itemIcon;
    public int price;
    public CurrencyType currencyType;
    public ShopCategory category; // 🌟 Thêm danh mục vật phẩm
    public string description; // 🌟 Thêm mô tả vật phẩm
}
