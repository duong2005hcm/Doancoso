using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class FirebaseLoginManager : MonoBehaviour
{
    public InputField ipRegisteremail;
    public InputField ipRegisterpassword;

    //Quan li dang ki, dang nhap
    public Button Registerbutton;
    private FirebaseAuth auth;
    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;

        Registerbutton.onClick.AddListener(RegisterAccountWithFirebase);
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
            }
            if (task.IsCompleted)
            {
                Debug.Log("Dang ki thanh cong!!!");
            }

        } );
    }
}
