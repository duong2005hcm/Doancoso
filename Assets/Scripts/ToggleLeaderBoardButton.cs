using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class LeaderBoardToggle : MonoBehaviour
{
    public GameObject buttonBXH; // Panel chứa BXH
    public Button ToggleLeaderBoardButton; // Nút bấm dấu "+"


    private bool isVisible = true; // Trạng thái mặc định: hiển thị BXH

    void Start()
    {
        if (ToggleLeaderBoardButton != null)
        {
            // Đảm bảo sự kiện onClick được gán đúng
            ToggleLeaderBoardButton.onClick.AddListener(ToggleLeaderboardButton);
        }
        else
        {
            Debug.LogError("⚠️ ToggleLeaderBoardButton chưa được gán trong Inspector!");
        }

        if (buttonBXH != null)
        {
            buttonBXH.SetActive(true); // Mặc định hiển thị BXH
        }
        else
        {
            Debug.LogError("⚠️ ButtonBXH chưa được gán trong Inspector!");
        }
    }

    public void ToggleLeaderboardButton()
    {
        if (buttonBXH != null)
        {
            isVisible = !isVisible; // Đảo trạng thái
            buttonBXH.SetActive(isVisible); // Ẩn/hiện BXH
            Debug.Log("🔘 Trạng thái BXH: " + (isVisible ? "Hiện" : "Ẩn"));
        }
        else
        {
            Debug.LogError("⚠️ ButtonBXH chưa được gán trong Inspector!");
        }
    }
}
