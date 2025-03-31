using UnityEngine;
using Firebase.Database;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using Firebase.Extensions;

public class FirebaseItemsManager : MonoBehaviour
{
    public static FirebaseItemsManager Instance;

    [SerializeField] private Transform itemContainer; // Content của ScrollView
    [SerializeField] private GameObject itemPrefab;   // Prefab của item

    private DatabaseReference dbReference;
    private List<GameObject> itemObjects = new List<GameObject>();
    private string currentCategory = "";
    private bool isLoading = false; // 🛑 Chặn load trùng lặp

    private void Awake()
    {
        Instance = this;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    private void Start()
    {
        LoadShopItems("supportItem"); // Mặc định tải danh mục đầu tiên
    }

    public void LoadShopItems(string category)
    {
        if (isLoading) return; // 🛑 Chặn load lại nếu chưa hoàn tất
        if (currentCategory == category) return; // 🛑 Chặn load lại danh mục đang hiển thị

        isLoading = true;
        currentCategory = category;

        Debug.Log($"🔄 Đang tải danh mục: {category}");

        // 🛑 Xóa tất cả vật phẩm cũ
        foreach (var item in itemObjects)
        {
            Destroy(item);
        }
        itemObjects.Clear(); // ✅ Đảm bảo danh sách trống hoàn toàn

        dbReference.Child("Shop").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log($"📊 Đọc dữ liệu Firebase, có {snapshot.ChildrenCount} vật phẩm.");

                foreach (var item in snapshot.Children)
                {
                    string itemId = item.Key;
                    string itemType = item.Child("type").Value.ToString();
                    if (itemType != category) continue;

                    string itemName = item.Child("name").Value.ToString();
                    string currency = item.Child("currency").Value.ToString();
                    string description = item.Child("description").Value.ToString();
                    int price = int.Parse(item.Child("price").Value.ToString());
                    string imageName = item.Child("imageURL").Value.ToString();

                    CreateItemUI(itemId, itemName, itemType, price, currency, description, imageName);
                }

                isLoading = false; // ✅ Đánh dấu load xong
            }
        });
    }

    private void CreateItemUI(string id, string name, string type, int price, string currency, string description, string imageName)
    {
        Sprite itemSprite = Resources.Load<Sprite>($"Images/Items/{imageName}");
        if (itemSprite == null)
        {
            Debug.LogWarning($"Không tìm thấy ảnh: {imageName}");
            return;
        }

        GameObject itemObject = Instantiate(itemPrefab, itemContainer);
        ItemUI itemUI = itemObject.GetComponent<ItemUI>();

        if (itemUI != null)
        {
            itemUI.Setup(id, name, type, price, currency, description, itemSprite);
        }

        itemObjects.Add(itemObject); // ✅ Lưu vào danh sách để xóa sau này
    }
}
