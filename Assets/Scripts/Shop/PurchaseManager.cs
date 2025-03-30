using UnityEngine;
using Firebase.Database;
using System.Collections.Generic;

public class PurchaseManager : MonoBehaviour
{
    public static PurchaseManager Instance;

    [SerializeField] private GameObject insufficientFundsPanel; // Panel khi không đủ tiền
    [SerializeField] private GameObject purchaseSuccessPanel; // Panel khi mua thành công
    [SerializeField] private GameObject itemDetailPanel; // Panel chi tiết vật phẩm

    private DatabaseReference dbReference;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void TryPurchase(string itemId, string itemType, int price, string currency, int quantity)
    {
        string userId = TestUser.Instance.UserId; // Lấy ID user từ Firebase

        dbReference.Child("Users").Child(userId).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                DataSnapshot snapshot = task.Result;
                int currentCoins = int.Parse(snapshot.Child("coins").Value.ToString());
                int currentDiamonds = int.Parse(snapshot.Child("diamonds").Value.ToString());

                bool isCoin = currency == "coin";
                int currentBalance = isCoin ? currentCoins : currentDiamonds;

                if (currentBalance >= price) // Nếu đủ tiền
                {
                    int newBalance = currentBalance - price;

                    // Cập nhật số tiền mới vào Firebase
                    dbReference.Child("Users").Child(userId).Child(isCoin ? "coins" : "diamonds").SetValueAsync(newBalance);

                    // Lưu vật phẩm vào Inventory
                    SaveToInventory(userId, itemId, itemType, quantity);

                    // Hiển thị panel mua thành công
                    ShowPurchaseSuccess();
                }
                else
                {
                    // Hiển thị panel không đủ tiền
                    ShowInsufficientFunds();
                }
            }
        });
    }

    private void SaveToInventory(string userId, string itemId, string itemType, int quantity)
    {
        DatabaseReference inventoryRef = dbReference.Child("Inventory").Child(userId).Child("items");

        inventoryRef.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Dictionary<string, object> updates = new Dictionary<string, object>();

                if (itemType == "supportItem")
                {
                    int currentQuantity = snapshot.HasChild(itemId) ? int.Parse(snapshot.Child(itemId).Child("quantity").Value.ToString()) : 0;
                    updates[itemId + "/quantity"] = currentQuantity + quantity;
                }
                else
                {
                    updates[itemId] = true; // Nếu là nhân vật hoặc trang phục, chỉ lưu true
                }

                inventoryRef.UpdateChildrenAsync(updates);
            }
        });
    }

    private void ShowInsufficientFunds()
    {
        Debug.Log("Không đủ tiền!");
        insufficientFundsPanel.SetActive(true);
    }

    private void ShowPurchaseSuccess()
    {
        Debug.Log("Mua hàng thành công!");
        purchaseSuccessPanel.SetActive(true);
    }

    public void CloseInsufficientFundsPanel()
    {
        insufficientFundsPanel.SetActive(false);
    }

    public void ClosePurchaseSuccessPanel()
    {
        purchaseSuccessPanel.SetActive(false);
        itemDetailPanel.SetActive(false); // Ẩn panel chi tiết vật phẩm sau khi mua
    }
}
