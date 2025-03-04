using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameManager : MonoBehaviour
{
    private bool gameStarted = false;

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Gameplay" && !gameStarted)
        {
            StartGame();
            gameStarted = true;
        }
    }

    public void StartGame()
    {
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
