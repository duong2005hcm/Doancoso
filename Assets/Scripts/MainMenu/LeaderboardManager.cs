using UnityEngine;
using UnityEngine.UI;

public class Leaderboad : MonoBehaviour
{
    public GameObject BXHPanel; // Panel cần mở/đóng
    public Button ButtonBXH; // Nút mở panel
    public Button Return; // Nút đóng panel

        public void OpenPanel()
    {
        BXHPanel.SetActive(true);
    }
    public void ClosePanel()
    {
        BXHPanel.SetActive(false);
    }
}
