using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class FirebaseManager : MonoBehaviour
{
    private DatabaseReference dbReference;

    void Start()
    {
        // Khởi tạo Firebase Database
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                dbReference = FirebaseDatabase.DefaultInstance.RootReference;
                LoadLeaderBoard();
            }
            else
            {
                Debug.LogError("Không thể kết nối Firebase: " + task.Result);
            }
        });
    }

    // Đọc dữ liệu từ LeaderBoard trong Firebase
    public void LoadLeaderBoard()
    {
        dbReference.Child("LeaderBoard").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Lỗi khi lấy dữ liệu LeaderBoard");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                List<PlayerData> leaderBoard = new List<PlayerData>();

                foreach (DataSnapshot child in snapshot.Children)
                {
                    string id = child.Child("IdUser").Value.ToString();
                    string name = child.Child("NameUser").Value.ToString();
                    int highScore = int.Parse(child.Child("HighCore").Value.ToString());
                    string avatarUrl = child.Child("avatarURL").Value.ToString();

                    leaderBoard.Add(new PlayerData(id, name, highScore, avatarUrl));
                }

                // Sắp xếp danh sách theo điểm cao nhất
                leaderBoard.Sort((a, b) => b.HighScore.CompareTo(a.HighScore));

                // Hiển thị lên bảng xếp hạng
                UIManager.Instance.UpdateLeaderBoardUI(leaderBoard);
            }
        });
    }
}

// Class lưu trữ dữ liệu người chơi
public class PlayerData
{
    public string IdUser;
    public string NameUser;
    public int HighScore;
    public string AvatarUrl;

    public PlayerData(string id, string name, int score, string avatar)
    {
        IdUser = id;
        NameUser = name;
        HighScore = score;
        AvatarUrl = avatar;
    }
}
