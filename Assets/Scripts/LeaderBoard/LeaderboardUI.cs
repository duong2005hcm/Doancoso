using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private Image avatarImage;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject crownIcon; // Thêm icon cho top 1

    [Header("Rank Colors")]
    [SerializeField] private Color firstPlaceColor = Color.yellow;
    [SerializeField] private Color secondPlaceColor = new Color(0.8f, 0.8f, 0.8f);
    [SerializeField] private Color thirdPlaceColor = new Color(0.8f, 0.5f, 0.2f);
    [SerializeField] private Color defaultColor = Color.white;

    public void Setup(int rank, string displayName, int highScore, Sprite avatarSprite)
    {
        // Set rank text with suffix
        rankText.text = GetRankSuffix(rank);

        // Set player name and score
        playerNameText.text = displayName;
        scoreText.text = highScore.ToString("N0");

        // Set avatar or default
        if (avatarSprite != null)
        {
            avatarImage.sprite = avatarSprite;
            avatarImage.gameObject.SetActive(true);
        }
        else
        {
            avatarImage.gameObject.SetActive(false);
        }

        // Highlight top players
        HighlightRank(rank);
    }

    private void HighlightRank(int rank)
    {
        // Set crown icon for top 1
        if (crownIcon != null)
        {
            crownIcon.SetActive(rank == 1);
        }

        // Set color based on rank
        Color targetColor = defaultColor;
        switch (rank)
        {
            case 1: targetColor = firstPlaceColor; break;
            case 2: targetColor = secondPlaceColor; break;
            case 3: targetColor = thirdPlaceColor; break;
        }

        rankText.color = targetColor;
        playerNameText.color = targetColor;
        scoreText.color = targetColor;
    }

    private string GetRankSuffix(int rank)
    {
        switch (rank)
        {
            case 1: return $"{rank}st";
            case 2: return $"{rank}nd";
            case 3: return $"{rank}rd";
            default: return $"{rank}th";
        }
    }
}