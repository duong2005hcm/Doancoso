using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EffectItemUI : MonoBehaviour
{
    [SerializeField] private Image effectIcon;
    [SerializeField] private TextMeshProUGUI timerText;

    private float remainingTime;
    private string effectId;

    public void Setup(string itemId, float duration)
    {
        effectId = itemId;
        remainingTime = duration;
        effectIcon.sprite = GetEffectIcon(itemId);
        StartCoroutine(TimerCountdown());
    }

    private IEnumerator TimerCountdown()
    {
        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            timerText.text = Mathf.Ceil(remainingTime).ToString();
            yield return null;
        }

        ActiveEffectUI.Instance.RemoveEffect(effectId);
    }

    public void ResetTimer(float duration)
    {
        remainingTime = duration;
    }

    private Sprite GetEffectIcon(string itemId)
    {
        switch (itemId)
        {
            case "0002": return Resources.Load<Sprite>("Images/EffectItems/SlowPeople");
            case "0003": return Resources.Load<Sprite>("Images/EffectItems/DoubleCoin");
            case "0004": return Resources.Load<Sprite>("Images/EffectItems/TripleCoin");
            case "0005": return Resources.Load<Sprite>("Images/EffectItems/Magnet");
            default:
                Debug.LogError("Không tìm thấy icon cho hiệu ứng với ID: " + itemId);
                return null;
        }
    }
}
