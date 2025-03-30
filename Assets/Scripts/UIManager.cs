using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Transform leaderboardParent;
    public GameObject leaderboardEntryPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateLeaderBoardUI(List<PlayerData> leaderBoard)
    {
        // Xóa danh sách cũ trước khi cập nhật
        foreach (Transform child in leaderboardParent)
        {
            Destroy(child.gameObject);
        }

        int stt = 1;
        foreach (var player in leaderBoard)
        {
            GameObject entry = Instantiate(leaderboardEntryPrefab, leaderboardParent);
            entry.transform.Find("STT").GetComponent<Text>().text = stt.ToString();
            entry.transform.Find("Name").GetComponent<Text>().text = player.NameUser;
            entry.transform.Find("Score").GetComponent<Text>().text = player.HighScore.ToString();
            stt++;
        }
    }
}
