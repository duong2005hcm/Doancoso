using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartButtonClicked()
    {
        SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
    }
    public void ShopButtonClicked()
    {
        SceneManager.LoadScene("Shop", LoadSceneMode.Single);
    }

}
