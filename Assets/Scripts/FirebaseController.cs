using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;
using Firebase.Database;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class FirebaseController : MonoBehaviour 
{
    public GameObject loginPanel, signupPanel, forgetPasswordPanel, notificationPanel;

    public TMP_InputField loginEmail, loginPassword, signupEmail, signupPassword, signupCPassword, signupUserName, forgetPassEmail;

    public TMP_Text notif_Title_Text, notif_Mess_Text;

    public Toggle rememberMe;
    private const string REMEMBER_ME_KEY = "RememberMe";

    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;

    bool isSignIn = false;

    private DatabaseReference databaseReference;
    public static bool shouldSkipAutoLogin = false;

    public class FirebaseUserData 
    {
        public string email;
        public string displayName;
        public int highScore;
        public int coins;
        public int diamonds;
        public string avatarType;
        public string avatarURL;
        public string selectedCharacter;

        public FirebaseUserData(string email, string displayName, int highScore = 0, 
            int coins = 0, int diamonds = 0, string avatarType = "default", 
            string avatarURL = "https://placehold.co/150x150", 
            string selectedCharacter = "default")
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

    void Start()
    {
        // Khôi phục trạng thái toggle
        rememberMe.isOn = PlayerPrefs.GetInt(REMEMBER_ME_KEY, 0) == 1;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

                // Chỉ auto-login nếu RememberMe ON và có user
                if (rememberMe.isOn && auth.CurrentUser != null && !shouldSkipAutoLogin)
                {
                    SceneManager.LoadScene("MainMenu");
                }
                else
                {
                    OpenLoginPanel();
                }
            }
        });
    } 

    public void OpenLoginPanel()
    {
        loginPanel.SetActive(true);
        signupPanel.SetActive(false);
        forgetPasswordPanel.SetActive(false);
    }

    public void OpenSignUpPanel()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(true);
        forgetPasswordPanel.SetActive(false);
    }

    public void OpenForgetPassPanel()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(false);
        forgetPasswordPanel.SetActive(true);
    }

    public void LoginUser()
    {
        if(string.IsNullOrEmpty(loginEmail.text)&&string.IsNullOrEmpty(loginPassword.text))
        {
            showNotificationMess("Thông báo", "Không được để trống");
            return;
        }
        //Login
        SignInUser(loginEmail.text, loginPassword.text);
    }

    public void SignUpUser()
    {
        if(string.IsNullOrEmpty(signupEmail.text)&& string.IsNullOrEmpty(signupPassword.text)&&string.IsNullOrEmpty(signupCPassword.text)&&string.IsNullOrEmpty(signupUserName.text))
        {
            showNotificationMess("Thông báo", "Không để trống");
            return;
        }
        //Signup

        if(signupPassword.text != signupCPassword.text)
        {
            showNotificationMess("Thông báo", "Mật khẩu xác nhận không trùng khớp");
            return;
        }//Confirm password

        CreateUser(signupEmail.text, signupPassword.text, signupUserName.text);
    }

    public void forgetPass(){
        if(string.IsNullOrEmpty(forgetPassEmail.text))
        {
            showNotificationMess("Thông báo","Chưa điền Email");
            return;
        }

        forgetPasswordSubmit(forgetPassEmail.text);
    }

    public void showNotificationMess(string title, string message)
    {
        notif_Title_Text.text = "" + title;
        notif_Mess_Text.text = ""+ message;
        notificationPanel.SetActive(true);
    }

    public void closeNotif_Panel()
    {
        notif_Title_Text.text = "" ;
        notif_Mess_Text.text = "";
        notificationPanel.SetActive(false);
    }

    public void logOut()
    {
        auth.SignOut();
        PlayerPrefs.SetInt(REMEMBER_ME_KEY, 0); // Reset Remember Me
        OpenLoginPanel();
    }

    public void SaveUserData(string userId, string email, string displayName, int highScore = 0, 
                        int coins = 0, int diamonds = 0, string avatarType = "default", 
                        string avatarURL = "https://placehold.co/150x150", 
                        string selectedCharacter = "default")
    {
        // Tạo dictionary chứa toàn bộ dữ liệu người dùng
        Dictionary<string, object> userData = new Dictionary<string, object>();
        userData["email"] = email;
        userData["displayName"] = displayName;
        userData["highScore"] = highScore;
        userData["coins"] = coins;
        userData["diamonds"] = diamonds;
        userData["avatarType"] = avatarType;
        userData["avatarURL"] = avatarURL;
        userData["selectedCharacter"] = selectedCharacter;
        userData["Shop/0001"] = true; // Dữ liệu shop mẫu
        userData["Shop/0002"] = true;

        // Đẩy toàn bộ dữ liệu lên database
        databaseReference.Child("Users").Child(userId).UpdateChildrenAsync(userData)
            .ContinueWithOnMainThread(task => {
                if (task.IsCompleted) {
                    Debug.Log("Dữ liệu người dùng đã được lưu thành công!");
                } else {
                    Debug.LogError("Lỗi khi lưu dữ liệu: " + task.Exception);
                }
            });
    }

    void CreateUser(string email, string password, string UserName)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled) {
                Debug.LogError("Đăng ký thất bại");
                showNotificationMess("Thông báo","Đăng ký thất bại");
                return;
            }

            if (task.IsFaulted) {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);

                foreach(Exception exception in task.Exception.Flatten().InnerExceptions){
                    Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        var errorCode = (AuthError)firebaseEx.ErrorCode;
                        showNotificationMess("Thông báo", GetErrorMessage(errorCode));
                    }      
                }
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);
            
            UpdateUserProfile(UserName);
            OpenLoginPanel();
            showNotificationMess("Chúc mừng","Tạo tài khoản thành công. Hãy đăng nhập");
        });
    }

    public void SignInUser(string email, string password){
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled) {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                showNotificationMess("Thông báo","Đăng nhập thất bại");
                return;
            }
            if (task.IsFaulted) {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                showNotificationMess("Thông báo","Đăng nhập thất bại");

                foreach(Exception exception in task.Exception.Flatten().InnerExceptions){
                    Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        var errorCode = (AuthError)firebaseEx.ErrorCode;
                        Debug.LogError("Firebase Error: " + errorCode.ToString());
                        showNotificationMess("Thông báo", GetErrorMessage(errorCode));
                    }
                    else
                    {
                        Debug.LogError("Lỗi không xác định: " + exception.Message);
                        showNotificationMess("Thông báo", "Có lỗi: " + exception.Message);
                    }
                }
                return;
            }

            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);

            // Lưu thông tin người dùng lên Realtime Database
            SaveUserData(
                userId: result.User.UserId,
                email: result.User.Email,
                displayName: result.User.DisplayName ?? "Player_" + Random.Range(1000, 9999),
                highScore: 0,
                coins: 100,
                diamonds: 10
            );

            PlayerPrefs.SetInt(REMEMBER_ME_KEY, rememberMe.isOn ? 1 : 0);
            SceneManager.LoadScene("MainMenu");
        });
    }

    void InitializeFirebase() {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs) {
        if (auth.CurrentUser != user) {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null
                && auth.CurrentUser.IsValid();
            if (!signedIn && user != null) {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn) {
                Debug.Log("Signed in " + user.UserId);
                isSignIn=true;
            }
        }
    }

    void OnDestroy() {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }

    void UpdateUserProfile(string UserName){
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        if (user != null) {
            Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile {
                DisplayName = UserName,
                PhotoUrl = new System.Uri("https://placehold.co/150x150"),
            };
            user.UpdateUserProfileAsync(profile).ContinueWith(task => {
                if (task.IsCanceled) {
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    return;
                }
                if (task.IsFaulted) {
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("User profile updated successfully.");
                showNotificationMess("Chúc mừng","Tạo tài khoản thành công");
            });
        }
    }

    bool isSigned= false;

    void Update()
    {
        if(isSignIn){
            if(isSigned){
                isSigned = true;
                SceneManager.LoadScene("MainMenu");
            }
        }
    }

    private static string GetErrorMessage(AuthError errorCode)
    {
        var message = "";
        switch (errorCode)
        {
            case AuthError.AccountExistsWithDifferentCredentials:
                message = "Tài khoản không tồn tại";
                break;
            case AuthError.MissingPassword:
                message = "Thiếu mật khẩu";
                break;
            case AuthError.WeakPassword:
                message = "Mật khẩu yếu";
                break;
            case AuthError.WrongPassword:
                message = "Mật khẩu không hợp lệ";
                break;
            case AuthError.EmailAlreadyInUse:
                message = "Email đã tồn tại";
                break;
            case AuthError.InvalidEmail:
                message = "Email không chính xác";
                break;
            case AuthError.MissingEmail:
                message = "Thiếu Email";
                break;
            default:
                message = "Kiểm tra lại email hoặc mật khẩu";
                break;
        }
        return message;
    }

    void forgetPasswordSubmit(string forgetPassEmail){
        auth.SendPasswordResetEmailAsync(forgetPassEmail).ContinueWithOnMainThread(task=>{
            if (task.IsCanceled){
                Debug.LogError("Gửi mật khẩu bị hủy");
            }
            if (task.IsFaulted){
                foreach(Exception exception in task.Exception.Flatten().InnerExceptions){
                    Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        var errorCode = (AuthError)firebaseEx.ErrorCode;
                        Debug.LogError("Firebase Error: " + errorCode.ToString());
                        showNotificationMess("Thông báo", GetErrorMessage(errorCode));
                    }
                }
            }

            OpenLoginPanel();
            showNotificationMess("Thông báo", "Gửi email reset mật khẩu thành công");
        });
    }
}