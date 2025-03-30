using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [SerializeField] private Button supportItemButton;
    [SerializeField] private Button characterButton;
    [SerializeField] private Button skinButton;
    [SerializeField] private Button exitShopButton;

    private string currentCategory = "supportItem";

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        supportItemButton.onClick.AddListener(() => ChangeCategory("supportItem"));
        characterButton.onClick.AddListener(() => ChangeCategory("character"));
        skinButton.onClick.AddListener(() => ChangeCategory("skin"));
        exitShopButton.onClick.AddListener(ExitShop);
    }

    public void ChangeCategory(string category)
    {
        currentCategory = category;
        FirebaseItemsManager.Instance.LoadShopItems(category);
    }

    public void ExitShop()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
