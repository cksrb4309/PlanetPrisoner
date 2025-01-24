using System;
using System.Collections;
using UnityEngine;

public class PlayerOxygen : MonoBehaviour
{
    [SerializeField] float maxOxygen;

    Coroutine decreaseCoroutine = null;

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
                currOxygen = maxOxygen;

            if (currOxygen <= 0f)
            {
                currOxygen = 0;

                Die();
            }

            //PlayerStateUI.Instance.SetOxygenFillImage(currOxygen > 0 ? currOxygen / maxOxygen : 0);
        }
    }

    private void Start()
    {
        currOxygen = maxOxygen;

        decreaseCoroutine = StartCoroutine(OxygenDecreaseCoroutine());
    }
    public void SetOxygenDecreaseValue(float oxygenDecreaseValue)
    {
        this.oxygenDecreaseValue = oxygenDecreaseValue;
    }
    IEnumerator OxygenDecreaseCoroutine()
    {
        WaitForSeconds delay = new WaitForSeconds(2f);

        while (true)
        {
            yield return delay;

            Oxygen -= oxygenDecreaseValue;
        }
    }
    void Die()
    {
        StopCoroutine(decreaseCoroutine);
    }
    public void RefillOxygen(float oxygen)
    {
        Oxygen += oxygen;
    }
}
