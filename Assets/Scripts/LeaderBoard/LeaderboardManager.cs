using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Auth;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class LeaderboardSystem : MonoBehaviour
{
    [SerializeField] private LeaderboardUI leaderboardUIPrefab;
    [SerializeField] private Transform leaderboardContainer;

    private DatabaseReference usersRef;
    private DatabaseReference leaderboardRef;

    void Start()
    {
        usersRef = FirebaseDatabase.DefaultInstance.GetReference("Users");
        leaderboardRef = FirebaseDatabase.DefaultInstance.GetReference("LeaderBoard");
        
    }

    public void LoadAndDisplayLeaderboard()
    {
        // Xóa các item cũ trước khi tải mới
        foreach (Transform child in leaderboardContainer)
        {
            Destroy(child.gameObject);
        }

        leaderboardRef.OrderByChild("highScore").LimitToLast(10).GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Lỗi tải leaderboard: " + task.Exception);
                    return;
                }

                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

                    // Lấy dữ liệu và sắp xếp theo điểm cao nhất
                    foreach (DataSnapshot userSnapshot in snapshot.Children)
                    {
                        entries.Add(new LeaderboardEntry(
                            userSnapshot.Key,
                            userSnapshot.Child("displayName").Value.ToString(),
                            int.Parse(userSnapshot.Child("highScore").Value.ToString()),
                            userSnapshot.Child("avatarURL").Value.ToString()
                        ));
                    }

                    // Sắp xếp giảm dần theo điểm
                    var sortedEntries = entries.OrderByDescending(e => e.highScore).ToList();

                    // Hiển thị lên UI
                    for (int i = 0; i < sortedEntries.Count; i++)
                    {
                        var entry = sortedEntries[i];
                        LeaderboardUI item = Instantiate(leaderboardUIPrefab, leaderboardContainer);

                        // Tải avatar từ URL (cần thêm hàm hỗ trợ tải ảnh)
                        StartCoroutine(LoadAvatar(entry.avatarURL, (sprite) =>
                        {
                            item.Setup(i + 1, entry.displayName, entry.highScore, sprite);
                        }));
                    }
                }
            });
    }

    private IEnumerator LoadAvatar(string url, System.Action<Sprite> callback)
    {
        if (string.IsNullOrEmpty(url))
        {
            callback(null);
            yield break;
        }

        using (WWW www = new WWW(url))
        {
            yield return www;

            if (string.IsNullOrEmpty(www.error))
            {
                Texture2D texture = www.texture;
                Sprite sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    Vector2.zero
                );
                callback(sprite);
            }
            else
            {
                Debug.LogError("Lỗi tải avatar: " + www.error);
                callback(null);
            }
        }
    }


    // Hàm chuyển dữ liệu từ User → LeaderBoard
    public void UpdateLeaderboard(string userId)
    {
        usersRef.Child(userId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Lỗi đọc dữ liệu User: " + task.Exception);
                return;
            }

            DataSnapshot userData = task.Result;
            if (userData.Exists)
            {
                // Lấy thông tin từ bảng User
                string displayName = userData.Child("displayName").Value.ToString();
                string avatarURL = userData.Child("avatarURL").Value.ToString();
                int currentScore = int.Parse(userData.Child("score").Value.ToString());

                // Kiểm tra điểm cao nhất trên LeaderBoard
                leaderboardRef.Child(userId).Child("highScore").GetValueAsync().ContinueWithOnMainThread(leaderboardTask =>
                {
                    int currentHighScore = 0;
                    if (leaderboardTask.Result.Exists)
                    {
                        currentHighScore = int.Parse(leaderboardTask.Result.Value.ToString());
                    }

                    // Chỉ cập nhật nếu điểm mới cao hơn
                    if (currentScore > currentHighScore)
                    {
                        Dictionary<string, object> leaderboardEntry = new Dictionary<string, object>
                        {
                            { "displayName", displayName },
                            { "highScore", currentScore },
                            { "avatarURL", avatarURL },
                            { "lastUpdated", ServerValue.Timestamp } // Thời gian Firebase
                        };

                        leaderboardRef.Child(userId).UpdateChildrenAsync(leaderboardEntry)
                            .ContinueWithOnMainThread(updateTask =>
                            {
                                Debug.Log(updateTask.IsCompleted ?
                                    "✅ Đã cập nhật LeaderBoard!" :
                                    "❌ Lỗi: " + updateTask.Exception);
                            });
                    }
                });
            }
        });
    }
}
public class LeaderboardEntry
{
    public string userId;
    public string displayName;
    public int highScore;
    public string avatarURL;

    public LeaderboardEntry(string userId, string displayName, int highScore, string avatarURL)
    {
        this.userId = userId;
        this.displayName = displayName;
        this.highScore = highScore;
        this.avatarURL = avatarURL;
    }
}