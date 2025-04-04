using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using System.Linq;
using System;

public class LeaderboardDetail : MonoBehaviour
{
    public static LeaderboardDetail Instance;

    [Header("UI References")]
    [SerializeField] private Transform contentTransform;
    [SerializeField] private GameObject leaderboardEntryPrefab;

    private DatabaseReference dbReference;
    private List<GameObject> activeEntries = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
        dbReference = FirebaseDatabase.DefaultInstance.GetReference("LeaderBoard");
    }

    private void Start()
    {
        LoadLeaderboard();
    }

    public void LoadLeaderboard()
    {
        if (!CheckDependencies()) return;

        ClearExistingEntries();

        dbReference.OrderByChild("highScore").LimitToLast(10).GetValueAsync()
        .ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Firebase error: " + task.Exception);
                return;
            }

            ProcessLeaderboardData(task.Result);
        });
    }

    private bool CheckDependencies()
    {
        if (contentTransform == null)
        {
            Debug.LogError("Missing content transform reference!");
            return false;
        }

        if (leaderboardEntryPrefab == null)
        {
            Debug.LogError("Missing leaderboard entry prefab!");
            return false;
        }

        if (dbReference == null)
        {
            Debug.LogError("Firebase not initialized!");
            return false;
        }

        return true;
    }

    private void ClearExistingEntries()
    {
        foreach (var entry in activeEntries)
        {
            Destroy(entry);
        }
        activeEntries.Clear();
    }

    private void ProcessLeaderboardData(DataSnapshot snapshot)
    {
        var players = new List<LeaderboardPlayer>();

        foreach (DataSnapshot userSnapshot in snapshot.Children)
        {
            var player = ParsePlayerData(userSnapshot);
            if (player != null) players.Add(player);
        }

        DisplaySortedPlayers(players.OrderByDescending(p => p.score).ToList());
    }

    private LeaderboardPlayer ParsePlayerData(DataSnapshot snapshot)
    {
        try
        {
            return new LeaderboardPlayer(
                snapshot.Child("displayName").Value.ToString(),
                Convert.ToSingle(snapshot.Child("highScore").Value),
                snapshot.Child("avatarURL").Value.ToString()
            );
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Failed to parse player data: {e.Message}");
            return null;
        }
    }

    private void DisplaySortedPlayers(List<LeaderboardPlayer> sortedPlayers)
    {
        for (int i = 0; i < sortedPlayers.Count; i++)
        {
            CreateLeaderboardEntry(i + 1, sortedPlayers[i]);
        }
        StartCoroutine(RefreshLayout());
    }

    private void CreateLeaderboardEntry(int rank, LeaderboardPlayer player)
    {
        var entry = Instantiate(leaderboardEntryPrefab, contentTransform);
        activeEntries.Add(entry);

        StartCoroutine(LoadAndSetupEntry(entry.GetComponent<LeaderboardUI>(), rank, player));
    }

    private IEnumerator LoadAndSetupEntry(LeaderboardUI entryUI, int rank, LeaderboardPlayer player)
    {
        if (entryUI == null) yield break;

        Sprite avatarSprite = null;

        if (!string.IsNullOrEmpty(player.avatarURL))
        {
            yield return LoadAvatar(player.avatarURL, sprite => avatarSprite = sprite);
        }

        entryUI.Setup(rank, player.name, Mathf.RoundToInt(player.score), avatarSprite);
    }

    private IEnumerator LoadAvatar(string url, System.Action<Sprite> callback)
    {
        using (var request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                callback?.Invoke(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero));
            }
            else
            {
                Debug.LogWarning($"Failed to load avatar: {request.error}");
                callback?.Invoke(null);
            }
        }
    }

    private IEnumerator RefreshLayout()
    {
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentTransform.GetComponent<RectTransform>());
    }
}

[System.Serializable]
public class LeaderboardPlayer
{
    public string name;
    public float score;
    public string avatarURL;

    public LeaderboardPlayer(string name, float score, string avatarURL)
    {
        this.name = name;
        this.score = score;
        this.avatarURL = avatarURL;
    }
}