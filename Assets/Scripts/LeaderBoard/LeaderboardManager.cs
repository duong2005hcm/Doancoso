using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Auth;
using System.Collections.Generic;

public class LeaderboardSystem : MonoBehaviour
{
    private DatabaseReference usersRef;
    private DatabaseReference leaderboardRef;

    void Start()
    {
        usersRef = FirebaseDatabase.DefaultInstance.GetReference("Users");
        leaderboardRef = FirebaseDatabase.DefaultInstance.GetReference("LeaderBoard");
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