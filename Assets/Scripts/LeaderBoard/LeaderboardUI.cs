using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI STT;
    [SerializeField] private Image AVT;
    [SerializeField] private TextMeshProUGUI TT;
    [SerializeField] private TextMeshProUGUI Score;

   

    public void Setup(int rank, string displayName, int highScore, Sprite avatarSprite)
    {
        STT.text = rank.ToString();
        TT.text = displayName;
        
        Score.text = highScore.ToString();
        if (avatarSprite != null)
        {
            AVT.sprite = avatarSprite;
        }
    }
}
