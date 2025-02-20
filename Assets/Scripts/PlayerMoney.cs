using UnityEngine;
using TMPro;

public class PlayerMoney : MonoBehaviour
{
    public static PlayerMoney Instance;

    [SerializeField] private int currentMoney;
    public const string prefMoney = "prefMoney";

    private void Awake()
    {
        Instance = this;
        currentMoney = PlayerPrefs.GetInt("prefMoney");
    }

    public void AddMoney(int moneyToAdd)
    {
        currentMoney += moneyToAdd;
    }    

    public void SaveMoney()
    {
        PlayerPrefs.SetInt("money", currentMoney);
    }    
}
