using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip touchSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadSettings();
    }

    private void LoadSettings()
    {
        float bgmVolume = PlayerPrefs.GetFloat("BGM_Volume", 1.0f);
        float sfxVolume = PlayerPrefs.GetFloat("SFX_Volume", 1.0f);

        SetBGMVolume(bgmVolume);
        SetSFXVolume(sfxVolume);
    }

    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

    public void PlayTouchSound()
    {
        if (touchSound != null)
        {
            sfxSource.PlayOneShot(touchSound);
        }
    }
}
