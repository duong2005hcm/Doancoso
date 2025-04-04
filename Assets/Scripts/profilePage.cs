using UnityEngine;
using TMPro;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class SimpleProfilePage : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_Text displayNameText;
    public TMP_Text emailText;
    public Image avatarImage; // Thêm reference tới Image component
    
    [Header("Navigation Buttons")]
    public Button backButton;
    public Button logoutButton;

    private FirebaseAuth auth;
    private DatabaseReference databaseReference; // Thêm database reference

    void Start()
    {
        // Initialize Firebase
        auth = FirebaseAuth.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        
        // Setup button listeners
        backButton.onClick.AddListener(GoBackToMainMenu);
        logoutButton.onClick.AddListener(LogoutUser);
        
        // Load user info
        LoadUserProfile();
        LoadAvatarFromDatabase();
    }

    void LoadUserProfile()
    {
        if (auth.CurrentUser != null)
        {
            displayNameText.text = auth.CurrentUser.DisplayName ?? "No Name Set";
            emailText.text = auth.CurrentUser.Email ?? "No Email Available";
        }
        else
        {
            SceneManager.LoadScene("LoginScene");
        }
    }

    async void LoadAvatarFromDatabase()
    {
        if (auth.CurrentUser == null || avatarImage == null) return;

        try
        {
            DataSnapshot snapshot = await databaseReference.Child("Users")
                .Child(auth.CurrentUser.UserId)
                .Child("avatarBase64")
                .GetValueAsync();

            if (snapshot.Exists)
            {
                string base64Image = snapshot.Value.ToString();
                byte[] imageBytes = System.Convert.FromBase64String(base64Image);
                Texture2D texture = new Texture2D(2, 2);
                
                if (texture.LoadImage(imageBytes))
                {
                    avatarImage.sprite = Sprite.Create(texture, 
                        new Rect(0, 0, texture.width, texture.height), 
                        new Vector2(0.5f, 0.5f));
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error loading avatar: " + e.Message);
        }
    }

    void GoBackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void LogoutUser()
    {
        auth.SignOut();
        SceneManager.LoadScene("LoginScene");
    }
}