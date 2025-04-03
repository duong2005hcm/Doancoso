using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Thêm namespace này để quản lý Scene

public class Leaderboard : MonoBehaviour
{
    public Button ButtonBXH; // Nút mở leaderboard
    public Button Return; // Nút quay lại

    private void Start()
    {
        // Gán sự kiện click cho các nút
        ButtonBXH.onClick.AddListener(OpenLeaderboardScene);
        Return.onClick.AddListener(ReturnToPreviousScene);
    }

    public void OpenLeaderboardScene()
    {
        // Tải Scene LeaderBoard
        SceneManager.LoadScene("LeaderBoard",LoadSceneMode.Single);
    }

    public void ReturnToPreviousScene()
    {
        // Quay lại Scene trước đó
        SceneManager.LoadScene("MainMenu");
    }
}