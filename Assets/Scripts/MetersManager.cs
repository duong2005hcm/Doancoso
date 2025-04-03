using TMPro;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using System.Collections;
using System.Net.NetworkInformation;
using Firebase.Extensions;
using System.Collections.Generic;

public class MetersManager : MonoBehaviour
{
    public static MetersManager Instance;
    [SerializeField] private TextMeshProUGUI MetersText;
    private float MetersTraveled;
    private bool isTraveling;
    private DatabaseReference databaseReference;
    private FirebaseAuth auth;
    private string userId; // Sẽ được gán sau khi đăng nhập
    private float saveInterval = 5f;
    private float lastSaveTime;
    private DatabaseSaves databaseSaves;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Khởi tạo Firebase
            databaseSaves = DatabaseSaves.instance;
            auth = FirebaseAuth.DefaultInstance;
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (auth.CurrentUser != null)
            {
                InitializeWithUser(auth.CurrentUser);
            }
            else
            {
                Debug.LogWarning("Chưa có người dùng đăng nhập");
                // Có thể thêm logic đăng nhập tự động hoặc chuyển đến màn hình đăng nhập
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Hàm này được gọi sau khi đăng nhập thành công
    public void InitializeWithUser(FirebaseUser user)
    {
        userId = user.UserId;
        Debug.Log($"Đã khởi tạo với người dùng: {user.Email} (UID: {userId})");

        // Tải dữ liệu đã lưu
        StartCoroutine(LoadSavedData());
    }

    IEnumerator LoadSavedData()
    {
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("UserID chưa được thiết lập");
            yield break;
        }

        var task = databaseReference.Child("Users").Child(userId).Child("MetersTraveled").GetValueAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Debug.LogError("Lỗi khi tải dữ liệu: " + task.Exception);
        }
        else if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                MetersTraveled = float.Parse(snapshot.Value.ToString());
                UpdateMetersDisplay();
                Debug.Log($"Đã tải dữ liệu: {MetersTraveled}m");
            }
            else
            {
                Debug.Log("Không tìm thấy dữ liệu đã lưu, bắt đầu từ 0");
            }
        }
    }

    void Update()
    {
        if (!isTraveling || string.IsNullOrEmpty(userId))
            return;

            MetersTraveled += Time.deltaTime * 5;
            UpdateMetersDisplay();

        if (Time.time - lastSaveTime > saveInterval)
        {
            SaveToFirebase();
            lastSaveTime = Time.time;
        }
    }

    private void UpdateMetersDisplay()
    {
        MetersText.text = (int)MetersTraveled + " m";
    }

    // Thêm vào phương thức StartScript
    public void StartScript()
    {
        if (string.IsNullOrEmpty(userId) || databaseReference == null )
        {
            Debug.LogError("Không thể bắt đầu - chưa có người dùng đăng nhập");
            return;
        }

        isTraveling = true;
        lastSaveTime = Time.time;
        ResetMeters();

        // Reset highScore khi bắt đầu game mới (nếu cần)
        // DatabaseSaves.Instance.ResetHighScore();
    }

    // Sửa phương thức ResetMeters
    public void ResetMeters()
    {
        if (string.IsNullOrEmpty(userId) || databaseReference == null)
        {
            Debug.LogError("Chưa sẵn sàng để reset - userId hoặc databaseReference null");
            return;
        }
        MetersTraveled = 0f;
        UpdateMetersDisplay();
        SaveToFirebase();

        // Nếu muốn reset cả highScore khi reset meters
        // DatabaseSaves.instance.ResetHighScore();
    }

    public void StopScript()
    {
        isTraveling = false;
        Debug.Log("Dừng di chuyển, kiểm tra cập nhật highScore...");
        SaveToFirebase();
    }

    public float GetMetersTraveled()
    {
        return MetersTraveled;
    }

    private void SaveToFirebase()
    {
        if (string.IsNullOrEmpty(userId) || databaseReference == null)
        {
            Debug.LogError("Không thể lưu - chưa có người dùng đăng nhập");
            return;
        }

        databaseReference.Child("Users").Child(userId).Child("MetersTraveled")
            .SetValueAsync(MetersTraveled)
            .ContinueWithOnMainThread(task => {
                if (task.IsCompleted)
                {
                    Debug.Log("Đã lưu MetersTraveled: " + MetersTraveled + "m");

                    // Cập nhật luôn highScore nếu điểm mới cao hơn
                    databaseReference.Child("Users").Child(userId).Child("highScore").GetValueAsync()
                        .ContinueWithOnMainThread(scoreTask => {
                            if (scoreTask.IsCompleted && scoreTask.Result.Exists)
                            {
                                float currentHighScore = float.Parse(scoreTask.Result.Value.ToString());
                                if (MetersTraveled > currentHighScore)
                                {
                                    databaseReference.Child("Users").Child(userId).Child("highScore").SetValueAsync(MetersTraveled);
                                    Debug.Log("highScore mới được cập nhật!");
                                    // Đây là lệnh đồng bộ quan trọng nhất
                                    SyncToLeaderboard(MetersTraveled);
                                }
                            }
                            else
                            {
                                databaseReference.Child("Users").Child(userId).Child("highScore").SetValueAsync(MetersTraveled);
                                Debug.Log("highScore mới được tạo!");
                            }
                        });
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("Lỗi khi lưu MetersTraveled: " + task.Exception);
                }
            });
    }

    private void SyncToLeaderboard(float meters)
    {
        // Kiểm tra 1: Biến 'userId'
        if (string.IsNullOrEmpty(userId)) // ← Biến này được khai báo ở class level
        {
            Debug.LogError("Không thể đồng bộ - chưa có userId");
            return;
        }

        // Kiểm tra 2: Biến 'databaseReference'
        databaseReference.Child("Users").Child(userId).GetValueAsync() // ← Biến này cũng được khai báo ở class level
            .ContinueWithOnMainThread(task =>
            {
                // Các biến trong callback:
                // - 'task' (tự động tạo bởi ContinueWith)
                // - 'userData' (tạo từ task.Result)

                if (task.IsFaulted)
                {
                    Debug.LogError("Lỗi đọc dữ liệu User: " + task.Exception);
                    return;
                }

                DataSnapshot userData = task.Result;
                if (userData.Exists)
                {
                    // Biến cục bộ:
                    Dictionary<string, object> leaderboardEntry = new Dictionary<string, object>();

                    // Gán các giá trị:
                    leaderboardEntry["highScore"] = meters; // ← Tham số đầu vào

                    if (userData.HasChild("displayName"))
                        leaderboardEntry["displayName"] = userData.Child("displayName").Value.ToString();

                    if (userData.HasChild("avatarURL"))
                        leaderboardEntry["avatarURL"] = userData.Child("avatarURL").Value.ToString();

                    leaderboardEntry["lastUpdated"] = ServerValue.Timestamp; // ← Firebase API

                    // Sử dụng lại 'databaseReference' và 'userId'
                    databaseReference.Child("LeaderBoard").Child(userId)
                        .UpdateChildrenAsync(leaderboardEntry)
                        .ContinueWithOnMainThread(updateTask =>
                        {
                            Debug.Log(updateTask.IsCompleted ?
                                "✅ Đã cập nhật LeaderBoard!" :
                                "❌ Lỗi LeaderBoard: " + updateTask.Exception);
                        });
                }
            });
    }

    private void OnApplicationQuit()
    {
        SaveToFirebase();
    }
    
}