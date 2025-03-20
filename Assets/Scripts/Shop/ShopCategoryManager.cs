using UnityEngine;

public class ShopCategoryManager : MonoBehaviour
{
    public ShopManager shopManager;

    public void SelectCategory(ShopCategory category) // Chỉnh sửa tham số từ string thành ShopCategory
    {
        shopManager.ChangeCategory(category); // Gọi phương thức ChangeCategory thay vì SetCategory
    }
}
