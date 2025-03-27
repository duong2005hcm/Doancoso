using UnityEngine;
using Firebase.Database;
using System.Collections.Generic;

public class FirebaseItemsManager : MonoBehaviour
{
    private DatabaseReference dbReference;

    private void Start()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void AddItemToDatabase(string id, string name, string type, int price, string currency, string imageURL, string characterId = "")
    {
        Dictionary<string, object> itemData = new Dictionary<string, object>
        {
            { "id", id },
            { "name", name },
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
