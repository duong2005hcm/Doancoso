using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject settingsPanel; // Panel chứa cài đặt
    [SerializeField] private GameObject copyrightPanel; // Panel bản quyền
    [SerializeField] private Slider bgmSlider; // Slider điều chỉnh nhạc nền (chưa hoạt động)
    [SerializeField] private Slider sfxSlider; // Slider điều chỉnh âm thanh va chạm (chưa hoạt động)
    [SerializeField] private Button saveButton; // Nút lưu cài đặt
    [SerializeField] private Button returnButton; // Nút quay lại
    [SerializeField] private Button copyrightButton; // Nút mở panel bản quyền
    [SerializeField] private Button copyrightCloseButton; // Nút đóng panel bản quyền

    private void Start()
    {
        LoadSettings(); // Tải cài đặt đã lưu khi mở game

        // Gán sự kiện cho nút
        saveButton.onClick.AddListener(SaveSettings);
        returnButton.onClick.AddListener(CloseSettingsPanel);
        copyrightButton.onClick.AddListener(OpenCopyrightPanel);
        copyrightCloseButton.onClick.AddListener(CloseCopyrightPanel);
    }

    /// <summary>
    /// Hiển thị Panel cài đặt
    /// </summary>
    public void OpenSettingsPanel()
    {
        settingsPanel.SetActive(true);
    }

    /// <summary>
    /// Đóng Panel cài đặt
    /// </summary>
    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
    }

    /// <summary>
    /// Hiển thị Panel bản quyền
    /// </summary>
    public void OpenCopyrightPanel()
    {
        copyrightPanel.SetActive(true);
    }

    /// <summary>
    /// Đóng Panel bản quyền
    /// </summary>
    public void CloseCopyrightPanel()
    {
        copyrightPanel.SetActive(false);
    }

    /// <summary>
    /// Lưu cài đặt vào PlayerPrefs
    /// </summary>
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("BGM_Volume", bgmSlider.value);
        PlayerPrefs.SetFloat("SFX_Volume", sfxSlider.value);
        PlayerPrefs.Save();
        Debug.Log("Settings Saved!");
    }

    /// <summary>
    /// Tải cài đặt từ PlayerPrefs
    /// </summary>
    private void LoadSettings()
    {
        bgmSlider.value = PlayerPrefs.GetFloat("BGM_Volume", 1.0f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFX_Volume", 1.0f);
    }
}
