using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    //Chuyen doi dang ki va dang nhap
    [Header("Switch form")]
    public Button MoveRegisButton;
    public Button MoveLoginButton;

    public GameObject LoginForm;
    public GameObject RegisterForm;
    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;

        Registerbutton.onClick.AddListener(RegisterAccountWithFirebase);
        LoginButton.onClick.AddListener(SignInAccountWithFirebase);

        MoveLoginButton.onClick.AddListener(SwitchForm);
        MoveRegisButton.onClick.AddListener(SwitchForm);
    }
    public void RegisterAccountWithFirebase()
    {
        string email = ipRegisteremail.text;
        string password = ipRegisteremail.text;

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => 
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

                //SceneManager.LoadScene("MainMenu");
                return;
            }

        });
    }
    public void SwitchForm()
    {
        LoginForm.SetActive(!LoginForm.activeSelf);
        RegisterForm.SetActive(!RegisterForm.activeSelf);
    }
}
