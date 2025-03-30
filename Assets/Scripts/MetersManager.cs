using TMPro;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using System.Collections;

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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Khởi tạo Firebase
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
        if (string.IsNullOrEmpty(userId))
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
    private void ResetMeters()
    {
        MetersTraveled = 0f;
        UpdateMetersDisplay();
        SaveToFirebase();

        // Nếu muốn reset cả highScore khi reset meters
        // DatabaseSaves.Instance.ResetHighScore();
    }

    public void StopScript()
    {
        isTraveling = false;
        SaveToFirebase();
    }

    public float GetMetersTraveled()
    {
        return MetersTraveled;
    }

    private void SaveToFirebase()
    {
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("Không thể lưu - chưa có người dùng đăng nhập");
            return;
        }

        databaseReference.Child("Users").Child(userId).Child("MetersTraveled").SetValueAsync(MetersTraveled)
            .ContinueWith(task => {
                if (task.IsCompleted)
                {
                    Debug.Log("Đã lưu quãng đường: " + MetersTraveled + "m");
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("Lỗi khi lưu dữ liệu: " + task.Exception);
                }
            });
    }

    private void OnApplicationQuit()
    {
        SaveToFirebase();
    }
    //private void ResetMeters ()
    //{
    //    MetersTraveled = 0f;
    //    UpdateMetersDisplay();
    //    SaveToFirebase();
    //}
}