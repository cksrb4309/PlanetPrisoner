using System;
using System.Collections;
using UnityEngine;

public class PlayerOxygen : MonoBehaviour
{
    [SerializeField] float maxOxygen;
    [SerializeField] float decreaseInterval = 2f;

    [HideInInspector] public float oxygenUsageFactor = 1f;

    Coroutine oxygenReductionCoroutine = null;


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
        }
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

            Oxygen -= oxygenDecreaseValue * oxygenUsageFactor;

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
}
