using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager Instance;

    [SerializeField] private Transform content;
    [SerializeField] private GameObject inventoryItemPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateInventoryUI()
    {
        Debug.Log("🔹 Cập nhật UI Inventory...");

        if (content == null || inventoryItemPrefab == null)
        {
            Debug.LogError("Lỗi: Content hoặc Prefab bị null!");
            return;
        }

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        Dictionary<string, int> items = InventoryManager.Instance.GetAllItems();
        Debug.Log("Số vật phẩm: " + items.Count);

        foreach (var kvp in items)
        {
            if (kvp.Value > 0)
            {
                Debug.Log($"Hiển thị {kvp.Key} - Số lượng: {kvp.Value}");

                GameObject itemGO = Instantiate(inventoryItemPrefab, content);
                itemGO.SetActive(true);

                Button itemButton = itemGO.GetComponent<Button>();
                InventoryItemUI itemUI = itemGO.GetComponent<InventoryItemUI>();

                if (itemUI != null)
                {
                    itemUI.Setup(kvp.Key, kvp.Value);
                }
                else
                {
                    Debug.LogError($"Prefab bị thiếu InventoryItemUI!");
                }

                if (itemButton != null)
                {
                    itemButton.onClick.AddListener(() => itemUI.OnItemClick());
                }
                else
                {
                    Debug.LogError($"Prefab không có Button component!");
                }
            }
        }
    }
}
