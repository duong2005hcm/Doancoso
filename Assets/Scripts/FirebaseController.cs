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

public class FirebaseController : MonoBehaviour 
{
    public GameObject loginPanel, signupPanel, profilePanel, forgetPasswordPanel, notificationPanel;

    public TMP_InputField loginEmail, loginPassword, signupEmail, signupPassword, signupCPassword, signupUserName, forgetPassEmail;

    public TMP_Text notif_Title_Text, notif_Mess_Text, pro5User_Text, pro5Email_Text;

    public Toggle rememberMe;

    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;

    bool isSignIn = false;



    void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                InitializeFirebase();

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            } else {
                UnityEngine.Debug.LogError(System.String.Format(
                "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }


    public void OpenLoginPanel()
    {
        loginPanel.SetActive(true);
        signupPanel.SetActive(false);
        profilePanel.SetActive(false);
        forgetPasswordPanel.SetActive(false);
    }

    public void OpenSignUpPanel()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(true);
        profilePanel.SetActive(false);
        forgetPasswordPanel.SetActive(false);
    }

    public void OpenProfilePanel()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(false);
        profilePanel.SetActive(true);
        forgetPasswordPanel.SetActive(false);
    }


    public void OpenForgetPassPanel()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(false);
        profilePanel.SetActive(false);
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
        pro5User_Text.text= "" ;
        pro5Email_Text.text="";
        OpenLoginPanel();

    }

    void CreateUser( string email, string password, string UserName )
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
            } //In ra lỗi và dừng lại

  // Firebase user has been created.
        Firebase.Auth.AuthResult result = task.Result;
        Debug.LogFormat("Firebase user created successfully: {0} ({1})",
            result.User.DisplayName, result.User.UserId);//kiểm tra xem tài khoản đã được tạo thành công hay chưa
        
        UpdateUserProfile(UserName);
        /*pro5User_Text.text=""+ user.DisplayName;
        pro5Email_Text.text=""+ user.Email;*/
        OpenLoginPanel();
        showNotificationMess("Chúc mừng","Tạo tài khoản thành công. Hãy đăng nhập");
});
    }

    public void SignInUser( string email, string password){
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
                        Debug.LogError("Firebase Error: " + errorCode.ToString()); // In ra lỗi cụ thể
                        showNotificationMess("Thông báo", GetErrorMessage(errorCode));
                    }
                    else
                    {
                    Debug.LogError("Lỗi không xác định: " + exception.Message);
                    showNotificationMess("Thông báo", "Có lỗi: " + exception.Message); // Hiển thị lỗi thực tế
                }
                }
                return;
            }

        Firebase.Auth.AuthResult result = task.Result;
        Debug.LogFormat("User signed in successfully: {0} ({1})",
            result.User.DisplayName, result.User.UserId);

        pro5User_Text.text=""+ user.DisplayName;
        pro5Email_Text.text=""+ user.Email;
                                                //result.
        OpenProfilePanel();
});
    }

    void InitializeFirebase() {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
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

    void UpdateUserProfile( string UserName){
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
                pro5User_Text.text=""+ user.DisplayName;
                pro5Email_Text.text=""+ user.Email;
                OpenProfilePanel();
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
            message = "Có lỗi";
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
                        Debug.LogError("Firebase Error: " + errorCode.ToString()); // In ra lỗi cụ thể
                        showNotificationMess("Thông báo", GetErrorMessage(errorCode));
                    }
                    
                }
            }

            OpenLoginPanel();
            showNotificationMess("Thông báo", "Gửi email reset mật khẩu thành công");
        });
    }
}
