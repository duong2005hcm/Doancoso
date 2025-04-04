using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectManager : MonoBehaviour
{
    public static ItemEffectManager Instance;

    private void Awake() => Instance = this;

    private bool isDoubleCoinActive = false;
    private bool isTripleCoinActive = false;
    private bool isMagnetActive = false;

    private Dictionary<string, Coroutine> activeEffects = new Dictionary<string, Coroutine>();

    public void ActivateItem(string itemId)
    {
        switch (itemId)
        {
            case "0001":
                ReviveManager.Instance.ShowRevivePanel();
                break;

            case "0002":
                ApplyEffect(itemId, 10f, ActivateSlowPeopleEffect, ResetSlowPeopleEffect);
                break;

            case "0003":
                if (isTripleCoinActive) return;
                ApplyEffect(itemId, 10f, () => isDoubleCoinActive = true, () => isDoubleCoinActive = false);
                break;

            case "0004":
                if (isDoubleCoinActive) return;
                ApplyEffect(itemId, 8f, () => isTripleCoinActive = true, () => isTripleCoinActive = false);
                break;

            case "0005":
                ApplyEffect(itemId, 10f, () => isMagnetActive = true, () => isMagnetActive = false);
                break;
        }
    }

    private void ApplyEffect(string effectId, float duration, System.Action startEffect, System.Action endEffect)
    {
        if (activeEffects.ContainsKey(effectId))
        {
            StopCoroutine(activeEffects[effectId]);
        }
        else
        {
            startEffect();
            ActiveEffectUI.Instance.ShowEffect(effectId, duration);
        }

        activeEffects[effectId] = StartCoroutine(EffectTimer(effectId, duration, endEffect));
    }

    private IEnumerator EffectTimer(string effectId, float duration, System.Action endEffect)
    {
        yield return new WaitForSeconds(duration);
        endEffect();
        activeEffects.Remove(effectId);
        ActiveEffectUI.Instance.RemoveEffect(effectId);
    }

    private void ActivateSlowPeopleEffect()
    {
        PeopleManager.Instance.ModifyPeopleSpeed(0.5f);
        SpawnManager.Instance.ModifySpawnRate(1.5f);
    }

    private void ResetSlowPeopleEffect()
    {
        PeopleManager.Instance.ModifyPeopleSpeed(2f);
        SpawnManager.Instance.ModifySpawnRate(0.66f);
    }

    public bool IsMagnetActive() => isMagnetActive;
    public bool IsDoubleCoinActive() => isDoubleCoinActive;
    public bool IsTripleCoinActive() => isTripleCoinActive;
}
