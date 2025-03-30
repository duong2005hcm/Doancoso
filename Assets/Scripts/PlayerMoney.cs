using UnityEngine;
using Firebase.Database;
using Firebase.Auth;

public class PlayerMoney : MonoBehaviour
{
    public static PlayerMoney Instance;

    private int currentMoney;
    private DatabaseReference dbReference;
    private FirebaseUser user;

    private void Awake()
    {
        Instance = this;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        user = FirebaseAuth.DefaultInstance.CurrentUser;

        if (user == null)
        {
            Debug.LogError("Người chơi chưa đăng nhập! Không thể tải dữ liệu.");
            return;
        }

        LoadMoneyFromFirebase();
    }

    private void LoadMoneyFromFirebase()
    {
        dbReference.Child("Users").Child(user.UserId).Child("coins").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                currentMoney = int.Parse(task.Result.Value.ToString());
                Debug.Log("Tải số coin từ Firebase: " + currentMoney);
            }
            else
            {
                Debug.LogError("Không thể lấy dữ liệu coin từ Firebase!");
                currentMoney = 0; // Nếu không có dữ liệu, đặt về 0
            }
        });
    }

    public void AddMoney(int moneyToAdd)
    {
        currentMoney += moneyToAdd;
    }

    public int GetCollectedCoins()
    {
        return currentMoney;
    }

    public void SaveMoney()
    {
        if (user == null)
        {
            Debug.LogError("Người chơi chưa đăng nhập, không thể lưu coin!");
            return;
        }

        dbReference.Child("Users").Child(user.UserId).Child("coins").SetValueAsync(currentMoney).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Coins đã được cập nhật: " + currentMoney);
            }
            else
            {
                Debug.LogError("Lỗi khi lưu coins: " + task.Exception);
            }
        });
    }
}
