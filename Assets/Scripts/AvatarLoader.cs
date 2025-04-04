using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Auth;
using UnityEngine.SceneManagement;
public class ProfileManager : MonoBehaviour
{
    public Image avatarDisplay; // Kéo thả Image của ProfilePage vào
    public Button changeButton; // Kéo thả nút vào

    public Button backButton;

    void Start()
    {
        backButton.onClick.AddListener(() => {
        SceneManager.LoadScene("MainMenu");
    });
    }

    void ShowAvatarSelection()
    {
        // Hiển thị tất cả avatar từ Resources (ví dụ chọn avatar1)
        UpdateAvatar("avatar1"); // Thay bằng tên file thực tế
    }

    void UpdateAvatar(string avatarName)
    {
        // Cập nhật UI
        avatarDisplay.sprite = Resources.Load<Sprite>($"Avatars/{avatarName}");
        
        // Lưu lên Firebase
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        FirebaseDatabase.DefaultInstance
            .GetReference($"users/{userId}/avatar")
            .SetValueAsync(avatarName);
    }

    void LoadAvatar()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        FirebaseDatabase.DefaultInstance
            .GetReference($"users/{userId}/avatar")
            .GetValueAsync().ContinueWith(task => {
                if (task.Result.Exists)
                {
                    avatarDisplay.sprite = Resources.Load<Sprite>($"Avatars/{task.Result.Value}");
                }
            });
    }

    void ReturnToMainMenu()
{
    // Reset trạng thái khi quay lại
    FirebaseController.shouldSkipAutoLogin = false;
    SceneManager.LoadScene("MainMenu");
}
}