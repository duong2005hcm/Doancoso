using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    private Dictionary<string, int> inventory = new Dictionary<string, int>();
    private DatabaseReference dbReference;
    private string userId;

    private void Awake()
    {
        Instance = this;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        LoadInventory();
    }

    private void LoadInventory()
    {
        dbReference.Child("Inventory").Child(userId).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                if (task.Result.Exists)
                {
                    inventory.Clear();
                    Debug.Log("Dữ liệu Inventory từ Firebase: " + task.Result.GetRawJsonValue());

                    foreach (var child in task.Result.Children)
                    {
                        string itemId = child.Key;
                        int quantity = int.Parse(child.Value.ToString());
                        if (quantity > 0)
                        {
                            inventory[itemId] = quantity;
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Firebase: Inventory của user này không có dữ liệu.");
                }

                InventoryUIManager.Instance.UpdateInventoryUI();
            }
            else
            {
                Debug.LogError("Lỗi khi tải Inventory từ Firebase: " + task.Exception);
            }
        });
    }

    public bool HasItem(string itemId)
    {
        return inventory.ContainsKey(itemId) && inventory[itemId] > 0;
    }

    public bool UseItem(string itemId)
    {
        if (HasItem(itemId))
        {
            inventory[itemId]--;
            dbReference.Child("Inventory").Child(userId).Child(itemId).SetValueAsync(inventory[itemId]);
            return true;
        }
        return false;
    }

    public int GetItemQuantity(string itemId)
    {
        return inventory.ContainsKey(itemId) ? inventory[itemId] : 0;
    }

    public Dictionary<string, int> GetAllItems()
    {
        return new Dictionary<string, int>(inventory);
    }
}
