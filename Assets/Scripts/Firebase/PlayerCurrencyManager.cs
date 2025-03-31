using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using TMPro;
using Firebase.Extensions;

public class PlayerCurrencyManager : MonoBehaviour
{
    private DatabaseReference dbReference;
    private string userId;

    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI diamondText;

    private void Start()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        // Kiểm tra người dùng đã đăng nhập chưa
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        }
        else
        {
            userId = "1GmBi8crvxOqrze0mEBkXXHyVBs1"; // Đặt UserID test nếu chưa đăng nhập
            Debug.LogWarning("FirebaseAuth user is null, using test user ID.");
        }

        LoadCurrencyData();
    }

    public void LoadCurrencyData()
    {
        dbReference.Child("Users").Child(userId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Lỗi khi tải dữ liệu tiền tệ: " + task.Exception);
                return;
            }

            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    int coins = snapshot.HasChild("coins") ? int.Parse(snapshot.Child("coins").Value.ToString()) : 0;
                    int diamonds = snapshot.HasChild("diamonds") ? int.Parse(snapshot.Child("diamonds").Value.ToString()) : 0;

                    // Cập nhật UI trên main thread
                    UpdateCurrencyUI(coins, diamonds);
                }
                else
                {
                    Debug.LogWarning("Không tìm thấy dữ liệu tiền tệ của user.");
                }
            }
        });
    }

    private void UpdateCurrencyUI(int coins, int diamonds)
    {
        if (coinText != null) coinText.text = coins.ToString();
        if (diamondText != null) diamondText.text = diamonds.ToString();
    }
}
