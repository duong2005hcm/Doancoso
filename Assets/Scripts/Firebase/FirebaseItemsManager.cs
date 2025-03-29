using UnityEngine;
using Firebase.Database;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class FirebaseItemsManager : MonoBehaviour
{
    public static FirebaseItemsManager Instance;

    [SerializeField] private Transform itemContainer; // Vùng hiển thị vật phẩm
    [SerializeField] private GameObject itemPrefab; // Prefab của vật phẩm trong shop

    private DatabaseReference dbReference;
    private List<GameObject> itemObjects = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    private void Start()
    {
        LoadShopItems("supportItem"); // Mặc định hiển thị vật phẩm hỗ trợ
    }

    public void LoadShopItems(string category)
    {
        dbReference.Child("Shop").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                ClearShopUI();

                foreach (DataSnapshot itemData in snapshot.Children)
                {
                    string type = itemData.Child("type").Value.ToString();
                    if (type != category) continue;

                    string id = itemData.Key;
                    string name = itemData.Child("name").Value.ToString();
                    string currency = itemData.Child("currency").Value.ToString();
                    string description = itemData.Child("description").Value.ToString();
                    int price = int.Parse(itemData.Child("price").Value.ToString());
                    string imageName = itemData.Child("imageName").Value.ToString(); // Chỉ lưu tên file

                    LoadItemImage(id, name, type, price, currency, description, imageName);
                }
            }
        });
    }

    private void ClearShopUI()
    {
        foreach (Transform child in itemContainer)
        {
            Destroy(child.gameObject);
        }
        itemObjects.Clear();
    }

    private void LoadItemImage(string id, string name, string type, int price, string currency, string description, string imageName)
    {
        // Load ảnh từ Resources
        Sprite itemSprite = Resources.Load<Sprite>($"Images/Items/{imageName}");
        if (itemSprite == null)
        {
            Debug.LogWarning($"Không tìm thấy ảnh: {imageName} trong Resources/Images/Items/");
            return;
        }

        GameObject itemObject = Instantiate(itemPrefab, itemContainer);
        itemObject.GetComponent<ItemUI>().Setup(id, name, type, price, currency, description, itemSprite);
        itemObjects.Add(itemObject);
    }
}
