using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public Transform itemContainer;
    public GameObject itemPrefab;
    public Button returnButton;
    public Button supportItemsButton;
    public Button charactersButton;
    public Button skinsButton;

    private DatabaseReference dbReference;
    private string selectedCategory = "supportItem"; // Mặc định hiển thị vật phẩm hỗ trợ

    private void Start()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        returnButton.onClick.AddListener(ReturnToMainMenu);
        supportItemsButton.onClick.AddListener(() => LoadShopItems("supportItem"));
        charactersButton.onClick.AddListener(() => LoadShopItems("character"));
        skinsButton.onClick.AddListener(() => LoadShopItems("skin"));

        LoadShopItems(selectedCategory);
    }

    private void LoadShopItems(string category)
    {
        selectedCategory = category;
        ClearShopItems();

        dbReference.Child("Shop").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                foreach (var child in task.Result.Children)
                {
                    string type = child.Child("type").Value.ToString();
                    if (type == category)
                    {
                        string itemId = child.Key;
                        string itemName = child.Child("name").Value.ToString();
                        int itemPrice = int.Parse(child.Child("price").Value.ToString());
                        string currency = child.Child("currency").Value.ToString();
                        string imageName = child.Child("image").Value.ToString();

                        CreateShopItem(itemId, itemName, itemPrice, currency, imageName);
                    }
                }
            }
        });
    }

    private void CreateShopItem(string id, string name, int price, string currency, string imageName)
    {
        GameObject newItem = Instantiate(itemPrefab, itemContainer);
        newItem.transform.Find("ItemName").GetComponent<Text>().text = name;
        newItem.transform.Find("ItemPrice").GetComponent<Text>().text = price.ToString();
        newItem.transform.Find("CurrencyIcon").GetComponent<Image>().sprite = LoadCurrencyIcon(currency);

        ImageLoader imageLoader = newItem.GetComponent<ImageLoader>();
        if (imageLoader != null)
        {
            imageLoader.LoadImage(imageName);
        }

        newItem.GetComponent<Button>().onClick.AddListener(() => OpenItemDetail(id, name, price, currency, imageName));
    }

    private void OpenItemDetail(string id, string name, int price, string currency, string imageName)
    {
        ItemDetailManager.Instance.ShowItemDetails(id, name, price, currency, imageName);
    }

    private void ClearShopItems()
    {
        foreach (Transform child in itemContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private void ReturnToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    private Sprite LoadCurrencyIcon(string currency)
    {
        return Resources.Load<Sprite>("Icons/" + currency);
    }
}
