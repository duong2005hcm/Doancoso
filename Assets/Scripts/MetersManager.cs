using TMPro;
using UnityEngine;

public class MetersManager : MonoBehaviour
{
    public static MetersManager Instance;
    [SerializeField] private TextMeshProUGUI MetersText;
    private float MetersTraveled;
    private bool isTraveling;

    private void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        if (!isTraveling)
            return;

        MetersTraveled += Time.deltaTime * 5;
        MetersText.text = (int)MetersTraveled + " m";
    }

    public void StartScript()
    {
        isTraveling = true;
    }

}
