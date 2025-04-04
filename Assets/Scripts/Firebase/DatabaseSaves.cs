using Firebase.Database;
using UnityEngine;
using Firebase.Extensions;
using System;
using Firebase.Auth;

public class DatabaseSaves : MonoBehaviour
{
    public static DatabaseSaves instance;
    private DatabaseReference databaseReference;
    private FirebaseAuth auth;
    private string currentUserId;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        // Khởi tạo Firebase
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;

        // Kiểm tra người dùng đã đăng nhập
        if (auth.CurrentUser != null)
        {
            currentUserId = auth.CurrentUser.UserId;
        }
    }

    // Phương thức để lưu toàn bộ thông tin người dùng
    public void SaveUserData(User userData)
    {
        if (string.IsNullOrEmpty(currentUserId))
        {
            Debug.LogError("Không thể lưu - chưa có người dùng đăng nhập");
            return;
        }

        string json = JsonUtility.ToJson(userData);
        databaseReference.Child("Users").Child(currentUserId).SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Đã lưu dữ liệu người dùng thành công");
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("Lỗi khi lưu dữ liệu người dùng: " + task.Exception);
                }
            });
    }

    // Phương thức để cập nhật chỉ số MetersTraveled
    // Thêm phương thức cập nhật highScore thông minh
    public void UpdateHighScore(float metersTraveled)
    {
        if (string.IsNullOrEmpty(currentUserId))
        {
            Debug.LogError("Không thể cập nhật - chưa có người dùng đăng nhập");
            return;
        }
        Debug.Log($"Bắt đầu cập nhật highScore với giá trị: {metersTraveled}");

        // Sử dụng transaction với kiểu float
        databaseReference.Child("Users").Child(currentUserId).Child("highScore")
            .RunTransaction(mutableData => {
                float currentHighScore = mutableData.Value != null ? Convert.ToSingle(mutableData.Value) : 0f;

                if (metersTraveled > currentHighScore)
                {
                    mutableData.Value = metersTraveled;
                    Debug.Log($"Đã cập nhật highScore mới: {metersTraveled}m");
                }
                return TransactionResult.Success(mutableData);
            }).ContinueWithOnMainThread(task => {
                if (task.IsFaulted)
                {
                    Debug.LogError("Lỗi khi cập nhật highScore: " + task.Exception);
                }
            });
    }

    // Sửa lại phương thức UpdateMetersTraveled
    public void UpdateMetersTraveled(float metersTraveled)
    {
        if (string.IsNullOrEmpty(currentUserId))
        {
            Debug.LogError("Không thể cập nhật - chưa có người dùng đăng nhập");
            return;
        }

        databaseReference.Child("Users").Child(currentUserId).Child("highScore")
    .RunTransaction(mutableData => {
        float currentHighScore = mutableData.Value != null ? Convert.ToSingle(mutableData.Value) : 0f;
        Debug.Log($"highScore hiện tại trong Firebase: {currentHighScore}");

        if (metersTraveled > currentHighScore)
        {
            mutableData.Value = metersTraveled;
            Debug.Log($"Đang cập nhật highScore mới: {metersTraveled}");
        }
        else
        {
            Debug.Log("Không cập nhật vì điểm mới thấp hơn highScore hiện tại.");
        }

        return TransactionResult.Success(mutableData);
    }).ContinueWithOnMainThread(task => {
        if (task.IsFaulted)
        {
            Debug.LogError("Lỗi khi cập nhật highScore: " + task.Exception);
        }
        else if (task.IsCompleted)
        {
            Debug.Log("Transaction cập nhật highScore thành công.");
        }
    });
    }

    // Thêm phương thức reset highScore
    public void ResetHighScore()
    {
        if (string.IsNullOrEmpty(currentUserId))
        {
            Debug.LogError("Không thể reset - chưa có người dùng đăng nhập");
            return;
        }

        databaseReference.Child("Users").Child(currentUserId).Child("highScore")
            .SetValueAsync(0).ContinueWithOnMainThread(task => {
                if (task.IsCompleted)
                {
                    Debug.Log("Đã reset highScore về 0");
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("Lỗi khi reset highScore: " + task.Exception);
                }
            });
    }

    // Phương thức để lấy dữ liệu người dùng
    public void LoadUserData(Action<User> onComplete)
    {
        if (string.IsNullOrEmpty(currentUserId))
        {
            Debug.LogError("Không thể tải - chưa có người dùng đăng nhập");
            return;
        }

        databaseReference.Child("Users").Child(currentUserId).GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Lỗi khi tải dữ liệu: " + task.Exception);
                    onComplete?.Invoke(null);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        string json = snapshot.GetRawJsonValue();
                        User userData = JsonUtility.FromJson<User>(json);
                        onComplete?.Invoke(userData);
                    }
                    else
                    {
                        Debug.Log("Không tìm thấy dữ liệu người dùng");
                        onComplete?.Invoke(null);
                    }
                }
            });
    }


    // Cập nhật khi có người dùng đăng nhập
    public void SetCurrentUser(FirebaseUser user)
    {
        currentUserId = user.UserId;
    }

    [System.Serializable]
    public class User
    {
        public float MetersTraveled;
        public string email;
        public string displayName;
        public float highScore; // Sử dụng để lưu MetersTraveled
        public int coins;
        public int diamonds;
        public string avatarType;
        public string avatarURL;
        public string selectedCharacter;

        public User(float MetersTraveled, string email, string displayName, float highScore, int coins, int diamonds,
                   string avatarType, string avatarURL, string selectedCharacter)
        {
            this.email = email;
            this.displayName = displayName;
            this.highScore = highScore;
            this.coins = coins;
            this.diamonds = diamonds;
            this.avatarType = avatarType;
            this.avatarURL = avatarURL;
            this.selectedCharacter = selectedCharacter;
            float metersTraveled = this.MetersTraveled;
        }
    }
}