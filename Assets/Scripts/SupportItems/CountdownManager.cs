using System.Collections;
using UnityEngine;
using TMPro;

public class CountdownManager : MonoBehaviour
{
    public static CountdownManager Instance;
    [SerializeField] private TextMeshProUGUI countdownText;

    private void Awake() => Instance = this;

    public IEnumerator StartCountdownRealtime(int seconds)
    {
        countdownText.gameObject.SetActive(true);
        for (int i = seconds; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }
        countdownText.text = "Bắt đầu!";
        yield return new WaitForSecondsRealtime(0.5f);
        countdownText.gameObject.SetActive(false);
    }
}
