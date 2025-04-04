using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItemUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI quantityText;

    private string itemId;

    public void Setup(string id, int quantity)
    {
        itemId = id;
        itemIcon.sprite = Resources.Load<Sprite>($"Images/Items/{id}");

        if (itemIcon.sprite == null)
        {
            Debug.LogError($"Không tìm thấy ảnh: Images/Items/{id}");
        }

        quantityText.text = quantity.ToString();
    }

    public void OnItemClick()
    {
        if (InventoryManager.Instance.UseItem(itemId))
        {
            ItemEffectManager.Instance.ActivateItem(itemId);
            InventoryUIManager.Instance.UpdateInventoryUI();
        }
    }
}
