using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button StartGameButton;
    public void StartButtonClicked()
    {
        SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
    }
    //public void ShopButtonClicked()
    //{
    //    SceneManager.LoadScene("Shop", LoadSceneMode.Single);
    //}

}