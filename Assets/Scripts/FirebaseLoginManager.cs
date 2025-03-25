using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase;

public class FirebaseLoginManager : MonoBehaviour
{
    //Quan li dang ki
    [Header("Register")]
    public InputField ipRegisteremail;
    public InputField ipRegisterpassword;

    public Button Registerbutton;

    //Quan li dang nhap
    [Header("Sign In")]
    public InputField ipLoginEmail;
    public InputField ipLoginPassword;

    public Button LoginButton;

    // Firebase Authentication
    private FirebaseAuth auth;
    private DatabaseReference dbReference;
    private FirebaseUser user;

    //Chuyen doi dang ki va dang nhap
    [Header("Switch form")]
    public Button MoveRegisButton;
    public Button MoveLoginButton;

    public GameObject LoginForm;
    public GameObject RegisterForm;
    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        FirebaseDatabase database = FirebaseDatabase.DefaultInstance;
        dbReference = database.RootReference;

        Registerbutton.onClick.AddListener(RegisterAccountWithFirebase);
        LoginButton.onClick.AddListener(SignInAccountWithFirebase);

        MoveLoginButton.onClick.AddListener(SwitchForm);
        MoveRegisButton.onClick.AddListener(SwitchForm);
    }
    public void RegisterAccountWithFirebase()
    {
        string email = ipRegisteremail.text;
        string password = ipRegisteremail.text;
        SaveNewUser(email, password);

        auth.CreateUserWithEmailAndPasswordAsync(email,password).ContinueWithOnMainThread(task => 
        { 
            if (task.IsCanceled)
            {
                Debug.Log("Dang ki bi huy!!");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.Log("Dang ki that bai!!");
                return;
            }
            if (task.IsCompleted)
            {
                Debug.Log("Dang ki thanh cong!!!");
                return;
            }

        } );
    }
    public void SignInAccountWithFirebase()
    {
        string email = ipLoginEmail.text;
        string password = ipLoginPassword.text;

        auth.SignInWithEmailAndPasswordAsync(email,password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.Log("Dang nhap bi huy!!");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.Log("Dang nhap that bai!!");
                return;
            }
            if (task.IsCompleted)
            {
                Debug.Log("Dang nhap thanh cong!!!");
                FirebaseUser user = task.Result.User;

                SceneManager.LoadScene("MainMenu");
                return;
            }

        });
    }
    public void SwitchForm()
    {
        LoginForm.SetActive(!LoginForm.activeSelf);
        RegisterForm.SetActive(!RegisterForm.activeSelf);
    }
    private void SaveNewUser(string userId, string email)
    {
        var user = new User(email, "New Player", 0, 0, 0, "default", "default_avatar.png", "char1");

        string json = JsonUtility.ToJson(user);
        dbReference.Child("Users").Child(userId).SetRawJsonValueAsync(json);
    }
}

[System.Serializable]
public class User
{
    public string email;
    public string displayName;
    public int highScore;
    public int coins;
    public int diamonds;
    public string avatarType;
    public string avatarURL;
    public string selectedCharacter;

    public User(string email, string displayName, int highScore, int coins, int diamonds, string avatarType, string avatarURL, string selectedCharacter)
    {
        this.email = email;
        this.displayName = displayName;
        this.highScore = highScore;
        this.coins = coins;
        this.diamonds = diamonds;
        this.avatarType = avatarType;
        this.avatarURL = avatarURL;
        this.selectedCharacter = selectedCharacter;
    }
}
