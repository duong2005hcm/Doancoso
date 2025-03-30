using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Auth;
using TMPro;
using Firebase.Extensions;

public class PurchaseManager : MonoBehaviour
{
    public static PurchaseManager Instance;

    [SerializeField] private GameObject purchaseSuccessPanel;
    [SerializeField] private Button successCloseButton;
    [SerializeField] private GameObject insufficientFundsPanel;
    [SerializeField] private Button insufficientCloseButton;

    private DatabaseReference dbReference;
    private FirebaseAuth auth;
    private string userId;
    private int userCoins;
    private int userDiamonds;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        if (auth.CurrentUser != null)
        {
            userId = auth.CurrentUser.UserId;
        }
        else
        {
            userId = "1GmBi8crvxOqrze0mEBkXXHyVBs1"; // Dùng khi test
            Debug.LogWarning("⚠️ FirebaseAuth user is null, using test user ID.");
        }

        LoadUserCurrency();

        // Ẩn panel ban đầu
        purchaseSuccessPanel.SetActive(false);
        insufficientFundsPanel.SetActive(false);

        // Gán sự kiện cho nút đóng panel
        successCloseButton.onClick.AddListener(ClosePurchaseSuccessPanel);
        insufficientCloseButton.onClick.AddListener(CloseInsufficientFundsPanel);
    }

    private void LoadUserCurrency()
    {
        dbReference.Child("Users").Child(userId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"❌ Lỗi khi tải tiền tệ từ Firebase: {task.Exception}");
                return;
            }

            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    userCoins = snapshot.HasChild("coins") ? int.Parse(snapshot.Child("coins").Value.ToString()) : 0;
                    userDiamonds = snapshot.HasChild("diamonds") ? int.Parse(snapshot.Child("diamonds").Value.ToString()) : 0;
                    Debug.Log($"✅ Tiền hiện tại - Coins: {userCoins}, Diamonds: {userDiamonds}");
                }
                else
                {
                    Debug.LogError($"❌ Không tìm thấy dữ liệu của UserID: {userId}");
                }
            }
        });
    }


    public void TryPurchase(string itemId, string itemType, int price, string currency, int quantity)
    {
        int totalCost = price * quantity;
        int currentBalance = (currency == "coin") ? userCoins : userDiamonds;

        if (currentBalance >= totalCost)
        {
            CompletePurchase(itemId, itemType, price, currency, quantity);
        }
        else
        {
            ShowInsufficientFundsPanel();
        }
    }

    private void CompletePurchase(string itemId, string itemType, int price, string currency, int quantity)
    {
        int totalCost = price * quantity;
        string currencyPath = (currency == "coin") ? "coins" : "diamonds";

        if (currency == "coin")
            userCoins -= totalCost;
        else
            userDiamonds -= totalCost;

        // Cập nhật tiền trên Firebase
        dbReference.Child("Users").Child(userId).Child(currencyPath).SetValueAsync(currency == "coin" ? userCoins : userDiamonds)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log($"✅ Đã cập nhật {currencyPath}: {(currency == "coin" ? userCoins : userDiamonds)}");

                    // Cập nhật hiển thị tiền tệ trong UI
                    PlayerCurrencyManager playerCurrencyManager = FindFirstObjectByType<PlayerCurrencyManager>();
                    if (playerCurrencyManager != null)
                    {
                        playerCurrencyManager.LoadCurrencyData();
                    }
                    else
                    {
                        Debug.LogError("❌ Không tìm thấy PlayerCurrencyManager để cập nhật tiền tệ.");
                    }
                }
                else
                {
                    Debug.LogError("❌ Cập nhật tiền thất bại!");
                }
            });

        // Lưu vật phẩm vào Inventory
        dbReference.Child("Inventory").Child(userId).Child(itemId).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                int currentQuantity = 0;
                if (task.Result.Exists)
                    currentQuantity = int.Parse(task.Result.Value.ToString());

                int newQuantity = currentQuantity + quantity;
                dbReference.Child("Inventory").Child(userId).Child(itemId).SetValueAsync(newQuantity);
                Debug.Log($"📦 Đã thêm {quantity}x {itemId} vào Inventory (Tổng: {newQuantity})");
            }
        });

        // Hiển thị panel mua thành công
        ShowPurchaseSuccessPanel();
    }

    private void ShowPurchaseSuccessPanel()
    {
        purchaseSuccessPanel.SetActive(true);
    }

    private void ClosePurchaseSuccessPanel()
    {
        purchaseSuccessPanel.SetActive(false);
        ItemDetailManager.Instance.ClosePanel(); // Đóng luôn panel chi tiết
    }

    private void ShowInsufficientFundsPanel()
    {
        insufficientFundsPanel.SetActive(true);
    }

    private void CloseInsufficientFundsPanel()
    {
        insufficientFundsPanel.SetActive(false);
    }
}
