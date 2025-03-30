using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase;

public class FirebaseLoginManager : MonoBehaviour
{
    [Header("Register")]
    public InputField ipRegisterEmail;
    public InputField ipRegisterPassword;
    public Button RegisterButton;

    [Header("Sign In")]
    public InputField ipLoginEmail;
    public InputField ipLoginPassword;
    public Button LoginButton;

    [Header("Sign Out")]
    public Button LogoutButton;

    [Header("Switch form")]
    public Button MoveRegisButton;
    public Button MoveLoginButton;
    public GameObject LoginForm;
    public GameObject RegisterForm;

    private FirebaseAuth auth;
    private DatabaseReference dbReference;
    private FirebaseUser user;

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        
        

        RegisterButton.onClick.AddListener(RegisterAccountWithFirebase);
        LoginButton.onClick.AddListener(SignInAccountWithFirebase);
        //LogoutButton.onClick.AddListener(SignOut);

        MoveLoginButton.onClick.AddListener(SwitchForm);
        MoveRegisButton.onClick.AddListener(SwitchForm);
    }

    public void RegisterAccountWithFirebase()
    {
        string email = ipRegisterEmail.text;
        string password = ipRegisterPassword.text;

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.Log("Đăng ký thất bại!");
                return;
            }

            FirebaseUser newUser = task.Result.User;
            string userId = newUser.UserId;
            SaveNewUser(userId, email);

            Debug.Log("Đăng ký thành công!");
            SceneManager.LoadScene("MainMenu");
        });
    }

    public void SignInAccountWithFirebase()
    {
        string email = ipLoginEmail.text;
        string password = ipLoginPassword.text;

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.Log("Đăng nhập thất bại!");
                return;
            }

            FirebaseUser user = task.Result.User;
            Debug.Log("Đăng nhập thành công!");
            SceneManager.LoadScene("MainMenu");
            user = auth.CurrentUser;
            MetersManager.Instance.InitializeWithUser(user);
        });
    }

    //public void SignOut()
    //{
    //    auth.SignOut();
    //    Debug.Log("Đã đăng xuất!");
    //    SceneManager.LoadScene("LoginScene");
    //}

    public void SwitchForm()
    {
        LoginForm.SetActive(!LoginForm.activeSelf);
        RegisterForm.SetActive(!RegisterForm.activeSelf);
    }

    private void SaveNewUser(string userId, string email)
    {
        User newUser = new (email, "New Player", 0, 0, 0, "default", "default_avatar.png", "char1");
        string json = JsonUtility.ToJson(newUser);
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
