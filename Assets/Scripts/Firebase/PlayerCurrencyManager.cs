using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using UnityEngine.UI;
using TMPro;

public class PlayerCurrencyManager : MonoBehaviour
{
    private DatabaseReference dbReference;
    private string userId;

    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI diamondText;

    private void Start()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        // Kiểm tra xem người dùng đã đăng nhập chưa
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
            LoadCurrencyData();
        }
        else
        {
            Debug.LogError("User is not logged in!");
        }
    }

    private void LoadCurrencyData()
    {
        dbReference.Child("Users").Child(userId).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                DataSnapshot snapshot = task.Result;

                int coins = snapshot.Child("coins").Exists ? int.Parse(snapshot.Child("coins").Value.ToString()) : 0;
                int diamonds = snapshot.Child("diamonds").Exists ? int.Parse(snapshot.Child("diamonds").Value.ToString()) : 0;

                // Hiển thị trên UI
                UpdateCurrencyUI(coins, diamonds);
            }
            else
            {
                Debug.LogError("Failed to load currency data: " + task.Exception);
            }
        });
    }

    private void UpdateCurrencyUI(int coins, int diamonds)
    {
        if (coinText != null) coinText.text = coins.ToString();
        if (diamondText != null) diamondText.text = diamonds.ToString();
    }
}
