using UnityEngine;
using Firebase.Database;
using System.Collections.Generic;

public class FirebaseItemsManager : MonoBehaviour
{
    private DatabaseReference dbReference;

    private void Start()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        AddItemToDatabase(
            id: "0001",
            name: "Dầu gió",
            description: "Bạn va phải người khác? Bôi dầu gió để giảm đau, giúp bạn tiếp tục chạy sau khi va chạm.",
            type: "supportItem",
            price: 50,
            currency: "coin",
            imageURL: "Image/Items/daugio"
        );

        AddItemToDatabase(
            id: "0002",
            name: "Cafe phin",
            description: "Uống vào giúp tỉnh táo, cảm giác như mọi thứ xung quanh chậm lại trong 10 giây",
            type: "supportItem",
            price: 30,
            currency: "coin",
            imageURL: "Image/Items/cafephin"
        );
    }

    public void AddItemToDatabase(string id, string name, string description, string type, int price, string currency, string imageURL, string characterId = "")
    {
        Dictionary<string, object> itemData = new Dictionary<string, object>
        {
            { "id", id },
            { "name", name },
            { "description", description },
            { "type", type },
            { "price", price },
            { "currency", currency },
            { "imageURL", imageURL }
        };

        if (!string.IsNullOrEmpty(characterId))
        {
            itemData["characterId"] = characterId;
        }

        dbReference.Child("Shop").Child(id).SetValueAsync(itemData).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Item added successfully: " + id);
            }
            else
            {
                Debug.LogError("Failed to add item: " + task.Exception);
            }
        });
    }
}
