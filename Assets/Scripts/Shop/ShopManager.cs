using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [SerializeField] private Button supportItemButton;
    [SerializeField] private Button characterButton;
    [SerializeField] private Button skinButton;
    [SerializeField] private Button exitShopButton;

    private string currentCategory = "supportItem"; // Mặc định hiển thị vật phẩm hỗ trợ

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        // Gán sự kiện cho các nút danh mục
        supportItemButton.onClick.AddListener(() => ChangeCategory("supportItem"));
        characterButton.onClick.AddListener(() => ChangeCategory("character"));
        skinButton.onClick.AddListener(() => ChangeCategory("skin"));

        // Gán sự kiện cho nút thoát shop
        exitShopButton.onClick.AddListener(ExitShop);

        // Load danh mục mặc định (vật phẩm hỗ trợ)
        ChangeCategory(currentCategory);
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
