using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Auth;
using UnityEngine.SceneManagement;

public class MainMenuAvatar : MonoBehaviour
{
    void Start()
    {
        // Đảm bảo có Button component
        if (!TryGetComponent<Button>(out var button))
        {
            button = gameObject.AddComponent<Button>();
        }

        // Thiết lập button
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(GoToProfileScene);

        // Tải avatar hiện tại
        LoadCurrentAvatar();
    }

    void GoToProfileScene()
    {
        FirebaseController.shouldSkipAutoLogin = true;

        SceneManager.LoadScene("in4");
    }

    void LoadCurrentAvatar()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        FirebaseDatabase.DefaultInstance
            .GetReference($"users/{userId}/avatar")
            .GetValueAsync().ContinueWith(task => {
                if (task.IsCompleted && task.Result.Exists)
                {
                    string avatarName = task.Result.Value.ToString();
                    GetComponent<Image>().sprite = Resources.Load<Sprite>($"Avatars/{avatarName}");
                }
            });
    }
}