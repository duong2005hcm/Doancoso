using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Firebase.Database;
using Firebase.Auth;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

public class AvatarFunction : MonoBehaviour
{
    public Image img;
    
    private DatabaseReference databaseReference;
    private FirebaseAuth auth;

    void Start()
    {
        InitializeFirebase();
    }

    void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    
    public void OpenImageFile()
    {
        #if UNITY_ANDROID
        string[] fileTypes = new string[] { "image/*" };
        #else
        string[] fileTypes = new string[] { "public.image" };
        #endif

        NativeFilePicker.PickFile((path) =>
        {
            if(path == null)
            {
                Debug.Log("Operation cancelled");
            }
            else
            {
                Debug.Log("Picked file: " + path);
                StartCoroutine(LoadAndSaveImage(path));
            }
        }, fileTypes);
    }

    IEnumerator LoadAndSaveImage(string imagePath)
    {
        byte[] imageBytes = File.ReadAllBytes(imagePath);
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(imageBytes);
        Sprite sprite = Sprite.Create(texture, new Rect(0,0,texture.width, texture.height), new Vector2(0.5f,0.0f), 1.0f);
        img.sprite = sprite;

        string base64Image = System.Convert.ToBase64String(imageBytes);
        SaveAvatarToDatabase(base64Image);

        yield return null;
    }

    private void SaveAvatarToDatabase(string base64Image)
    {
        if (auth.CurrentUser == null) return;

        string userId = auth.CurrentUser.UserId;
        
        Dictionary<string, object> updates = new Dictionary<string, object>();
        updates["avatarBase64"] = base64Image;
        
        // Sử dụng ContinueWith thay vì ContinueWithOnMainThread
        databaseReference.Child("Users").Child(userId).UpdateChildrenAsync(updates)
            .ContinueWith(task => {
                if (task.IsCompleted)
                {
                    Debug.Log("Avatar saved to database successfully");
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("Failed to save avatar: " + task.Exception);
                }
            });
    }
}