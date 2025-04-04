using System.Collections.Generic;
using UnityEngine;

public class ActiveEffectUI : MonoBehaviour
{
    public static ActiveEffectUI Instance;
    [SerializeField] private Transform effectContainer;  
    [SerializeField] private GameObject effectPrefab;
    private Dictionary<string, GameObject> activeEffects = new Dictionary<string, GameObject>();

    private void Awake() => Instance = this;

    public void ShowEffect(string itemId, float duration)
    {
        if (activeEffects.ContainsKey(itemId))
        {
            activeEffects[itemId].GetComponent<EffectItemUI>().ResetTimer(duration);
        }
        else
        {
            GameObject effectUI = Instantiate(effectPrefab, effectContainer);
            effectUI.GetComponent<EffectItemUI>().Setup(itemId, duration);
            activeEffects[itemId] = effectUI;
        }
    }

    public void RemoveEffect(string itemId)
    {
        if (activeEffects.ContainsKey(itemId))
        {
            Destroy(activeEffects[itemId]);
            activeEffects.Remove(itemId);
        }
    }
}
