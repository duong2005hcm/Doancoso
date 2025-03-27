using UnityEngine;
using Firebase;
using Firebase.Extensions;

public class FirebaseCheck : MonoBehaviour
{
    void Start()
    {
        Debug.Log("⏳ Đang kiểm tra Firebase...");
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                Debug.Log("✅ Firebase đã khởi tạo thành công!");
            }
            else
            {
                Debug.LogError($"❌ Firebase lỗi: {task.Result}");
            }
        });
    }
}
