using System;
using System.Collections;
using UnityEngine;

public class PlayerOxygen : MonoBehaviour
{
    static PlayerOxygen instance = null;
    public static bool IsAlive
    {
        get
        {
            return instance.currOxygen > 0f;
        }
    }
    public static Vector3 DeadPosition => instance.deadPosition;
    
    Vector3 deadPosition = Vector3.zero;

    [SerializeField] InGameTime inGameTime;

    [SerializeField] float maxOxygen;
    [SerializeField] float decreaseInterval = 2f;

    [HideInInspector] public float oxygenUsageFactor = 1f;

    Coroutine oxygenReductionCoroutine = null;

    float panalty = 1f;

    float currOxygen = 0f;

    float oxygenDecreaseValue = 0;

    bool isDie = false;

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

        Oxygen = maxOxygen;
    }
    private void OnEnable()
    {
        NextDayController.Subscribe(FirstGameFinished, ActionType.FirstGameFinished);
        NextDayController.Subscribe(SurviveTransition, ActionType.SurviveTransition);
        NextDayController.Subscribe(NextDayTransition, ActionType.SurviveFinished);
        NextDayController.Subscribe(NextDayReady, ActionType.NextDayReady);
        NextDayController.Subscribe(NextDayTransition, ActionType.NextDayTransition);
    }
    private void OnDisable()
    {
        NextDayController.Unsubscribe(FirstGameFinished, ActionType.FirstGameFinished);
        NextDayController.Unsubscribe(SurviveTransition, ActionType.SurviveTransition);
        NextDayController.Unsubscribe(NextDayTransition, ActionType.SurviveFinished);
        NextDayController.Unsubscribe(NextDayReady, ActionType.NextDayReady);
        NextDayController.Unsubscribe(NextDayTransition, ActionType.NextDayTransition);
    }
    void FirstGameFinished()
    {
        Oxygen = maxOxygen;

        if (oxygenReductionCoroutine != null) StopCoroutine(oxygenReductionCoroutine);

        oxygenReductionCoroutine = StartCoroutine(ReduceOxygenOverTime());
    }
    void SurviveTransition()
    {
        if (oxygenReductionCoroutine != null) StopCoroutine(oxygenReductionCoroutine);

        Oxygen += 20f;
    }
    void NextDayReady()
    {
        if (oxygenReductionCoroutine != null) StopCoroutine(oxygenReductionCoroutine);
    }
    void NextDayTransition()
    {
        if (oxygenReductionCoroutine != null) StopCoroutine(oxygenReductionCoroutine);

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
        if (isDie) return;

        isDie = true;

        StopCoroutine(oxygenReductionCoroutine);

        deadPosition = transform.position;

        GameManager.chance--;

        if (GameManager.chance != 0)
        {
            inGameTime.SurviveNextFadeInOut();
        }
        else
        {
            NextDayController.TriggerFadeNextDayUI(NextDayOption.GameOver);
        }
    }
    public void RefillOxygen(float oxygen)
    {
        Oxygen += oxygen;
    }
    public float NeedFillOxygen() => maxOxygen - currOxygen;

    public static void SetPenalty(float panalty) => instance.panalty = panalty;
}
