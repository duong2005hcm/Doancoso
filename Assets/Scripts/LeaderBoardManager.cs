using UnityEngine;

public class LeaderBoardManager : MonoBehaviour
{
    public GameObject leaderBoardFullPanel; // Kéo thả LeaderBoardPanel vào đây

    void Start()
    {
        // Đảm bảo bảng xếp hạng ẩn lúc đầu
        leaderBoardFullPanel.SetActive(false);
    }

    public void OpenFullLeaderBoard()
    {
        leaderBoardFullPanel.SetActive(true);
    }

    public void CloseFullLeaderBoard()
    {
        leaderBoardFullPanel.SetActive(false);
    }
}
