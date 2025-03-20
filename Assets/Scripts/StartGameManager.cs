using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class StartGameManager : MonoBehaviour
{
    public TMP_Text countdownText;
    private bool gameStarted = false;

    private void Start()
    {
        countdownText.gameObject.SetActive(false); // Ẩn text từ đầu
        StartCoroutine(GameStartCountdown());
    }

    private IEnumerator GameStartCountdown()
    {
        gameStarted = true;
        countdownText.gameObject.SetActive(true);
        Time.timeScale = 0; // Dừng game khi bắt đầu

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1); // Đếm ngược dù game bị dừng
        }

        countdownText.text = "GO!";
        yield return new WaitForSecondsRealtime(1);

        countdownText.gameObject.SetActive(false);
        Time.timeScale = 1; // Bắt đầu game

        if (SpawnManager.Instance != null)
        {
            SpawnManager.Instance.StartScript();
        }

        if (MetersManager.Instance != null)
        {
            MetersManager.Instance.StartScript();
        }
    }
}
