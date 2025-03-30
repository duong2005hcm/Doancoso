using UnityEngine;
using Firebase.Database;
using UnityEngine.UI; 
using System.Threading.Tasks;
using TMPro;

public class DatabaseLeaderBoard : MonoBehaviour
{
    [Header("UI scene")]
    public Text Rank;
    public Image Avt;
    public TMP_Text Name;
    public TMP_Text HighScore;

    // Khai bao Firebase
    private FirebaseDatabase data;
    private DatabaseReference dbReference;
    private Firebase.Auth.FirebaseAuth auth;

    // Ham gui diem len Firebase
    public void SubmitScore(string userID, int highScore)
    {
        string key = dbReference.Child("LeaderBoard").Push().Key;
        LeaderboardEntry entry = new LeaderboardEntry(userID, highScore);
        string json = JsonUtility.ToJson(entry);
        dbReference.Child("LeaderBoard").Child(key).SetRawJsonValueAsync(json);
    }

    private void Start()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        data = FirebaseDatabase.DefaultInstance;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    public void GetLeaderboard()
    {
        dbReference.Child("LeaderBoard").OrderByChild("highScore").LimitToLast(10).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Lỗi khi lấy dữ liệu: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;
            foreach (DataSnapshot child in snapshot.Children)
            {
                string json = child.GetRawJsonValue();
                LeaderboardEntry entry = JsonUtility.FromJson<LeaderboardEntry>(json);
                Debug.Log($"Player: {entry.displayName}, Score: {entry.highScore}");
            }
        });
    }

    [System.Serializable]
    public class LeaderboardEntry //  LeaderboardEntry
    {
        public int highScore;
        public string displayName;
        public int WeeklyScore;
        public string avatarURL;

        // Constructor khop voi ten lop
        public LeaderboardEntry(string displayName, int highScore)
        {
            this.displayName = displayName;
            this.highScore = highScore;
            this.WeeklyScore = 0; // Default Value
            this.avatarURL = "";  // Default Value
        }
    }
}