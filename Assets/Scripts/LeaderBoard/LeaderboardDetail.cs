using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using System.Linq;

public class LeaderboardDetail : MonoBehaviour
{
    private DatabaseReference dbReference;
    public Transform contentTransform; // Content của ScrollView
    public GameObject leaderboardEntryPrefab; // Prefab entry

    private void ClearContent()
    {
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }
    }
    private void Start()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        Debug.Log("🚀 Starting Firebase Test...");

        if (FirebaseDatabase.DefaultInstance == null)
        {
            Debug.LogError("❌ FirebaseDatabase not initialized!");
        }
        else
        {
            Debug.Log("✅ FirebaseDatabase is ready.");
            LoadLeaderboard();
        }
    }

    public void SubmitScore(string userId, string name, int highScore, string avatarURL)
    {
        User userData = new User(name, highScore, avatarURL);
        string json = JsonUtility.ToJson(userData);
        dbReference.Child("LeaderBoard").Child(userId).SetRawJsonValueAsync(json);
    }

    public void LoadLeaderboard()
    {
        Debug.Log("📡 Fetching leaderboard data...");
        ClearContent(); // Xóa nội dung cũ trước khi tải mới

        dbReference.Child("LeaderBoard").OrderByChild("highScore").LimitToLast(10)
        .GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("❌ Load leaderboard failed: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;
            Debug.Log("📥 Data received from Firebase: " + snapshot.ChildrenCount);

            if (snapshot.ChildrenCount == 0)
            {
                Debug.LogWarning("⚠️ No leaderboard data found!");
                return;
            }

            // Tạo danh sách người chơi và sắp xếp
            List<KeyValuePair<string, User>> players = new List<KeyValuePair<string, User>>();

            foreach (DataSnapshot data in snapshot.Children)
            {
                string json = data.GetRawJsonValue();
                User user = JsonUtility.FromJson<User>(json);
                players.Add(new KeyValuePair<string, User>(data.Key, user));
            }

            // Sắp xếp giảm dần theo điểm
            players.Sort((a, b) => b.Value.highScore.CompareTo(a.Value.highScore));

            // Tạo UI entry cho mỗi người chơi
            for (int i = 0; i < players.Count; i++)
            {
                var player = players[i];
                StartCoroutine(CreateLeaderboardEntry(
                    i + 1, // Rank
                    player.Value.displayName,
                    player.Value.highScore,
                    player.Value.avatarType
                ));
            }
        });
    }

    private IEnumerator RefreshScrollView()
    {
        yield return new WaitForEndOfFrame(); // Đợi 1 frame để giao diện cập nhật
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentTransform.GetComponent<RectTransform>());
    }


    private IEnumerator CreateLeaderboardEntry(int rank, string name, int score, string avatarURL)
    {
        GameObject entry = Instantiate(leaderboardEntryPrefab, contentTransform);
        LeaderboardUI entryUI = entry.GetComponent<LeaderboardUI>();

        if (entryUI != null)
        {
            Sprite avatarSprite = null;
            if (!string.IsNullOrEmpty(avatarURL))
            {
                using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(avatarURL))
                {
                    yield return request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                        avatarSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    }
                }
            }

            entryUI.Setup(rank, name, score, avatarSprite);
        }
    }


}

[System.Serializable]
public class User
{
    public string displayName;
    public int highScore;
    public string avatarType; // Đổi tên từ avatarURL nếu cần thống nhất

    public User(string name, int highScore, string avatarType)
    {
        this.displayName = name;
        this.highScore = highScore;
        this.avatarType = avatarType;
    }
}