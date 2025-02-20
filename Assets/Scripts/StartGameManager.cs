using UnityEngine;

public class StartGameManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) || (Input.touchCount > 0))
        {
            StartGame();
        }
    }
    private void StartGame()
    {
        SpawnManager.Instance.StartScript();
        MetersManager.Instance.StartScript();
        enabled = false;
    }
}

