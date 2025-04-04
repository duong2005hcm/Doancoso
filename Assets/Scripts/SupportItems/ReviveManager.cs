using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ReviveManager : MonoBehaviour
{
    public static ReviveManager Instance;
    [SerializeField] private GameObject revivePanel;
    [SerializeField] private Button useReviveButton;
    [SerializeField] private Button declineReviveButton;

    private void Awake()
    {
        Instance = this;
        revivePanel.SetActive(false);

        useReviveButton.onClick.AddListener(OnUseRevive);
        declineReviveButton.onClick.AddListener(OnDeclineRevive);
    }

    public void ShowRevivePanel()
    {
        Time.timeScale = 0;
        revivePanel.SetActive(true);
    }

    private void OnUseRevive()
    {
        revivePanel.SetActive(false);
        InventoryManager.Instance.UseItem("0001");
        StartCoroutine(ReviveSequence());
    }

    private IEnumerator ReviveSequence()
    {
        yield return CountdownManager.Instance.StartCountdownRealtime(3);

        foreach (GameObject entity in GameObject.FindGameObjectsWithTag("Entity"))
        {
            Destroy(entity);
        }

        if (PlayerCollision.Instance != null)
        {
            PlayerCollision.Instance.Revive();
        }
        else
        {
            Debug.LogError("PlayerCollision.Instance is null after waiting.");
        }

        Time.timeScale = 1;
    }

    private void OnDeclineRevive()
    {
        revivePanel.SetActive(false);
        FinishGameManager.Instance.FinishGame();
    }
}
