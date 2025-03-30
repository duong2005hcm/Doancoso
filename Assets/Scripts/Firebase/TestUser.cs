using UnityEngine;
using Firebase.Auth;

public class TestUser : MonoBehaviour
{
    public static TestUser Instance { get; private set; }
    public string UserId { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        UserId = auth.CurrentUser != null ? auth.CurrentUser.UserId : "1GmBi8crvxOqrze0mEBkXXHyVBs1";
    }
}
