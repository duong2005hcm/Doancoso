using UnityEngine;

public class AvatarManager : MonoBehaviour
{
    [SerializeField] private GameObject avatarPanel; // Panel chứa avatar

    // Mở Avatar Panel
    public void OpenAvatarPanel()
    {
        if (avatarPanel != null)
        {
            avatarPanel.SetActive(true);
        }
    }

    // Đóng Avatar Panel
    public void CloseAvatarPanel()
    {
        if (avatarPanel != null)
        {
            avatarPanel.SetActive(false);
        }
    }
}
