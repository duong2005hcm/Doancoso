using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardManager : MonoBehaviour
{
    public GameObject panel; // Panel cần mở/đóng
    public Button  openButton; // Nút mở panel
    public Button returnButton; // Nút đóng panel

    public void OpenPanel()
    {
        panel.SetActive(true);
    }
    public void ClosePanel()
    {
        panel.SetActive(false);
    }
}
