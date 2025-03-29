using UnityEngine;
using Firebase.Database;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class FirebaseItemsManager : MonoBehaviour
{
    public static FirebaseItemsManager Instance;

    [SerializeField] private Transform itemContainer; // Gán Content của ScrollView
    [SerializeField] private GameObject itemPrefab; // Gán prefab item

    private DatabaseReference dbReference;
    private List<GameObject> itemObjects = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    private void Start()
    {
        LoadShopItems();
    }

    public void LoadShopItems(string category = "supportItem")
    {
        Debug.Log($"🔄 Đang tải vật phẩm trong danh mục: {category}");

        dbReference.Child("Shop").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log($"✅ Số vật phẩm trong Shop: {snapshot.ChildrenCount}");

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
                    string imageName = itemData.Child("imageURL").Value.ToString(); // Đảm bảo chỉ lưu tên file

                    Debug.Log($"🛒 Đọc vật phẩm: {name}, ID: {id}, Ảnh: {imageName}");

                    LoadItemImage(id, name, type, price, currency, description, imageName);
                }
            }
            else
            {
                Debug.LogError("❌ Lỗi khi lấy dữ liệu Shop từ Firebase!");
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
        Debug.Log($"🛠️ Đang tạo Item: {name}");  // Log kiểm tra
        Sprite itemSprite = Resources.Load<Sprite>($"Images/Items/{imageName}");

        if (itemSprite == null)
        {
            Debug.LogWarning($"❌ Không tìm thấy ảnh: {imageName} trong Resources/Images/Items/");
            return;
        }

        GameObject itemObject = Instantiate(itemPrefab, itemContainer);
        Debug.Log($"✅ Tạo thành công: {name} - Gán vào {itemContainer.name}"); // Log kiểm tra
        itemObject.GetComponent<ItemUI>().Setup(id, name, type, price, currency, description, itemSprite);
    }
}
