using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using TMPro;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private Image coinIcon;

    private int totalCoins;
    private int collectedCoinsThisRun;
    private DatabaseReference dbReference;
    private string userId;
    private bool isGameOver = false;

    private void Awake()
    {
        Instance = this;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        userId = user != null ? user.UserId : "1GmBi8crvxOqrze0mEBkXXHyVBs1";

        LoadMoneyFromFirebase();
    }

    private void LoadMoneyFromFirebase()
    {
        dbReference.Child("Users").Child(userId).Child("coins").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                totalCoins = int.Parse(task.Result.Value.ToString());
            }
            else
            {
                totalCoins = 0;
            }
        });

        collectedCoinsThisRun = 0;
        UpdateCoinUI();
    }

    public void AddMoney(int moneyToAdd)
    {
        collectedCoinsThisRun += moneyToAdd;
        UpdateCoinUI();
    }

    public int GetCollectedCoinsThisRun()
    {
        return collectedCoinsThisRun;
    }

    public void SaveMoney()
    {
        totalCoins += collectedCoinsThisRun;
        dbReference.Child("Users").Child(userId).Child("coins").SetValueAsync(totalCoins);

        isGameOver = true;
    }

    public void ResetCoins()
    {
        if (!isGameOver) return;
        collectedCoinsThisRun = 0;
        isGameOver = false;
        UpdateCoinUI();
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = collectedCoinsThisRun.ToString();
    }
}
