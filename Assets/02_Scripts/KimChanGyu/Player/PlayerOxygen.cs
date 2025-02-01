using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerOxygen : MonoBehaviour
{
    static PlayerOxygen instance = null;

    [SerializeField] float maxOxygen;
    [SerializeField] float decreaseInterval = 2f;

    [HideInInspector] public float oxygenUsageFactor = 1f;

    Coroutine oxygenReductionCoroutine = null;

    float panalty = 1f;

    float currOxygen = 0f;

    float oxygenDecreaseValue = 0;

    float Oxygen
    {
        get
        {
            return currOxygen;
        }
        set
        {
            currOxygen = value;

            if (currOxygen >= maxOxygen)
                currOxygen = maxOxygen;


            if (currOxygen <= 0f)
            {
                currOxygen = 0;

                Die();
            }
            PlayerStateUI.Instance.SetOxygenFillImage(currOxygen > 0 ? currOxygen / maxOxygen : 0);
            ScreenEffectController.TriggerScreenGrayscaleEffect((1 - (currOxygen > 0 ? currOxygen / maxOxygen : 0)) * -100f);
        }
    }
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        currOxygen = maxOxygen;

        oxygenReductionCoroutine = StartCoroutine(ReduceOxygenOverTime());
    }
    public void SetOxygenDecreaseValue(float oxygenDecreaseValue)
    {
        this.oxygenDecreaseValue = oxygenDecreaseValue;
    }
    IEnumerator ReduceOxygenOverTime()
    {
        WaitForSeconds delay = new WaitForSeconds(decreaseInterval);

        while (true)
        {
            yield return delay;

            Oxygen -= oxygenDecreaseValue * oxygenUsageFactor * panalty;

            oxygenUsageFactor = 1f;
        }
    }
    void Die()
    {
        StopCoroutine(oxygenReductionCoroutine);
    }
    public void RefillOxygen(float oxygen)
    {
        Oxygen += oxygen;
    }
    public float NeedFillOxygen() => maxOxygen - currOxygen;

    public static void SetPenalty(float panalty) => instance.panalty = panalty;
}
