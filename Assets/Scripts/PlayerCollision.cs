using UnityEngine;
using Firebase.Database;
using Firebase.Auth;

public class PlayerCollision : MonoBehaviour
{
    private DatabaseReference dbReference;
    private bool gameFinished = false; // Tránh gọi FinishGame nhiều lần

    private void Start()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Entity"))
        {
            EntityType entity = collision.GetComponent<EntityType>();
            if (entity == null) return; // Tránh lỗi nếu object không có EntityType

            switch (entity.entityType)
            {
                case EntityType.EntityTypes.Coin:
                    PlayerMoney.Instance.AddMoney(1);
                    CoinManager.Instance.AddCoin(1); // Cập nhật UI
                    Destroy(collision.gameObject);
                    break;

                case EntityType.EntityTypes.People:
                    if (!gameFinished)
                    {
                        gameFinished = true;
                        FinishGame();
                    }
                    break;
            }
        }
    }

    private void FinishGame()
    {
        FirebaseUser user = Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser;

        if (user == null)
        {
            Debug.LogError("Người chơi chưa đăng nhập! Không thể lưu dữ liệu.");
            FinishGameManager.Instance?.FinishGame();
            return;
        }

        string userId = user.UserId;
        int collectedCoins = PlayerMoney.Instance?.GetCollectedCoins() ?? 0;

        dbReference.Child("Users").Child(userId).Child("coins").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted || task.Result == null || task.Result.Value == null)
            {
                Debug.LogError("Lỗi khi lấy dữ liệu từ Firebase hoặc dữ liệu rỗng!");
                return;
            }

            int currentCoins = int.Parse(task.Result.Value.ToString());
            int newTotalCoins = currentCoins + collectedCoins;

            dbReference.Child("Users").Child(userId).Child("coins").SetValueAsync(newTotalCoins).ContinueWith(setTask =>
            {
                if (setTask.IsCompleted)
                {
                    Debug.Log("Coins updated successfully: " + newTotalCoins);
                }
                else if (setTask.IsFaulted)
                {
                    Debug.LogError("Lỗi khi cập nhật coins!");
                }
            });
        });

        FinishGameManager.Instance?.FinishGame();
    }

}
