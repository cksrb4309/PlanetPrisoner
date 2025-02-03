using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using VInspector;

public class NextDayController : MonoBehaviour
{
    static NextDayController instance = null;

    [SerializeField] CanvasGroup backgroundCanvasGroup;
    [SerializeField] CanvasGroup textCanvasGroup;
    [SerializeField] CanvasGroup buttonCanvasGroup;

    [SerializeField] SerializedDictionary<FadeType, AnimationCurve> fadeCurves;

    [SerializeField] TMP_Text mainTextUI;

    Action[] eventActions;

    //Coroutine nextDayCoroutine = null;

    int remainingDays; // 게임 클리어까지 남은 일수
    int remainingChances; // 남은 기회

    bool isInputReceived = false;
    bool isYes = false;

    private void Awake()
    {
        instance = this;

        eventActions = new Action[Enum.GetValues(typeof(ActionType)).Length];

        for (int i = 0; i < eventActions.Length; i++) eventActions[i] = null;

        Subscribe(ConnectReadyToNext, ActionType.NextDayReady);

        buttonCanvasGroup.interactable = false;
    }
    void ConnectReadyToNext() => FadeNextDayUI(NextDayOption.Next);
    public void SceneStart() => FadeNextDayUI(NextDayOption.FirstSceneMove);
    public void OnClickYesOrNo(bool isYes)
    {
        this.isYes = isYes;

        isInputReceived = true;
    }
    public void Start()
    {
        backgroundCanvasGroup.alpha = 1f;
        textCanvasGroup.alpha = 0f;

        FadeNextDayUI(NextDayOption.Next);
    }
    public static void TriggerFadeNextDayUI(NextDayOption nextDayOption)
    {
        instance.FadeNextDayUI(nextDayOption);
    }
    void FadeNextDayUI(NextDayOption nextDayOption)
    {
        remainingDays = GameManager.Instance.d_days;
        remainingChances = 3;

        //if (nextDayCoroutine != null) StopCoroutine(nextDayCoroutine);

        IEnumerator executeCoroutine =
            nextDayOption == NextDayOption.Ready ? ReadyCoroutine() :
            nextDayOption == NextDayOption.Cancel ? CancelCoroutine() :
            nextDayOption == NextDayOption.Next ? NextCoroutine() :
            nextDayOption == NextDayOption.Survive ? SurviveCoroutine() :
            nextDayOption == NextDayOption.GameOver ? GameOverCoroutine():
            FirstSceneMoveCoroutine();

        //nextDayCoroutine = StartCoroutine(executeCoroutine);

        StartCoroutine(executeCoroutine);
    }
    IEnumerator ReadyCoroutine()
    {
        // 배경 캔버스 활성화
        yield return FadeInOutCoroutine(backgroundCanvasGroup, 1f, FadeType.LinearIncrease);

        // 다음날 준비 이벤트 활성화
        OnEventInvoke(ActionType.NextDayReady);
    }
    IEnumerator CancelCoroutine()
    {
        // TODO : 취소 코루틴 작성바람
        yield return null;
    }
    IEnumerator NextCoroutine()
    {
        // 텍스트 캔버스 활성화
        yield return FadeInOutCoroutine(textCanvasGroup, 1f, FadeType.LinearIncrease);

        // 텍스트 타이핑
        yield return TextTypingCoroutine(mainTextUI, 0.1f, $"D-Day : {remainingDays}");

        // 1초 지연
        yield return new WaitForSecondsRealtime(1f);

        // 텍스트 캔버스 비활성화
        yield return FadeInOutCoroutine(textCanvasGroup, 1f, FadeType.BlinkDecrease);

        // 텍스트 캔버스 비활성화 후 텍스트 UI text 초기화
        mainTextUI.text = string.Empty;

        // 배경 캔버스 비활성화
        yield return FadeInOutCoroutine(backgroundCanvasGroup, 1f, FadeType.LinearDecrease);

        // 다음날 전환 성공 이벤트 액션 호출
        OnEventInvoke(ActionType.NextDayFinished);
    }
    IEnumerator SurviveCoroutine()
    {
        // 배경 캔버스 활성화
        yield return FadeInOutCoroutine(backgroundCanvasGroup, 1f, FadeType.BlinkIncrease);

        // 1초 지연
        yield return new WaitForSecondsRealtime(1f);

        // 생존 했을 시 이벤트 액션 호출
        OnEventInvoke(ActionType.SurviveTransition);

        // 텍스트 캔버스 활성화
        yield return FadeInOutCoroutine(textCanvasGroup, 1f, FadeType.LinearIncrease);

        // 텍스트 입력
        yield return TextTypingCoroutine(mainTextUI, 0.3f, $"남은 기회 : {remainingChances}");

        // 1초 지연
        yield return new WaitForSecondsRealtime(1f);

        // 텍스트 캔버스 비활성화
        yield return FadeInOutCoroutine(textCanvasGroup, 1f, FadeType.LinearDecrease);

        // 텍스트 초기화
        mainTextUI.text = string.Empty;

        // 배경 캔버스 비활성화
        yield return FadeInOutCoroutine(backgroundCanvasGroup, 1f, FadeType.LinearDecrease);

        // 생존 했을 시 마무리 이벤트 액션 호출
        OnEventInvoke(ActionType.SurviveFinished);
    }
    IEnumerator GameOverCoroutine()
    {
        // 배경 캔버스 활성화
        yield return FadeInOutCoroutine(backgroundCanvasGroup, 3f, FadeType.BlinkIncrease);

        // 1초 지연
        yield return new WaitForSecondsRealtime(1f);

        // 게임오버 전환 이벤트 액션 호출
        OnEventInvoke(ActionType.GameOverTransition);

        // 텍스트 캔버스 활성화
        yield return FadeInOutCoroutine(textCanvasGroup, 3f, FadeType.LinearIncrease);

        // 텍스트 입력
        yield return TextTypingCoroutine(mainTextUI, 0.3f, "G A M E O V E R");

        // 1초 지연
        yield return new WaitForSecondsRealtime(1f);

        // 텍스트 입력
        yield return TextTypingCoroutine(mainTextUI, 0.1f, "살아남지 못하셨습니다");

        // 1초 지연
        yield return new WaitForSecondsRealtime(1f);

        // 게임오버 마무리 이벤트 액션 호출
        OnEventInvoke(ActionType.GameOverFinished);
    }
    IEnumerator FirstSceneMoveCoroutine()
    {
        // 배경 캔버스 활성화
        yield return FadeInOutCoroutine(backgroundCanvasGroup, 3f, FadeType.BlinkIncrease);

        // 1초 지연
        yield return new WaitForSecondsRealtime(1f);

        // 텍스트 캔버스 활성화
        yield return FadeInOutCoroutine(textCanvasGroup, 3f, FadeType.LinearIncrease);

        // 텍스트 입력
        yield return TextTypingCoroutine(mainTextUI, 0.1f, $"{remainingDays}일 동안 살아남아주세요");

        // 1초 지연
        yield return new WaitForSecondsRealtime(1f);

        // 텍스트 입력
        yield return TextTypingCoroutine(mainTextUI, 0.1f, "추가 설명이 필요하십니까?");

        // 입력 대기
        yield return WaitingInputActionCoroutine();

        
    }
    IEnumerator WaitingInputActionCoroutine()
    {
        yield return FadeInOutCoroutine(buttonCanvasGroup, 1f, FadeType.LinearIncrease);

        buttonCanvasGroup.interactable = true;

        while (!isInputReceived) yield return null;

        isInputReceived = false;

        buttonCanvasGroup.interactable = false;

        yield return FadeInOutCoroutine(buttonCanvasGroup, 1f, FadeType.LinearDecrease);
    }
    IEnumerator FadeInOutCoroutine(CanvasGroup target, float fadeSpeed, FadeType fadeType, float startValue = 0f)
    {
        float t = startValue;

        AnimationCurve curve = fadeCurves[fadeType];

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * fadeSpeed;

            if (t > 1f) t = 1f;

            target.alpha = curve.Evaluate(t);

            yield return null;
        }
    }
    IEnumerator TextTypingCoroutine(TMP_Text textUI, float typingDelay, string message)
    {
        StringBuilder sb = new StringBuilder();

        textUI.text = string.Empty;

        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(typingDelay);

        for (int i = 0; i < message.Length; i++)
        {
            sb.Append(message[i]);

            textUI.text = sb.ToString();

            yield return delay;
        }
    }

    void OnEventInvoke(ActionType actionType) => eventActions[(int)actionType]?.Invoke();

    #region 이벤트 액션 구독 및 구독 해제
    public static void Subscribe(Action action, ActionType actionType)
    {
        instance.eventActions[(int)actionType] += action;
    }
    public static void Unsubscribe(Action action, ActionType actionType)
    {
        instance.eventActions[(int)actionType] -= action;
    }
    #endregion
}
public enum FadeType
{
    LinearIncrease,
    LinearDecrease,

    BlinkIncrease,
    BlinkDecrease,
}
public enum ActionType
{
    NextDayReady,
    NextDayTransition,
    NextDayFinished,
    SurviveTransition,
    SurviveFinished,
    GameOverTransition,
    GameOverFinished,
}
public enum NextDayOption
{
    Ready,      // 준비 시키기
    Cancel,     // 취소하기
    Next,       // 다음날로 전환
    Survive,    // 죽었으나 기회가 남아있어서 부활함
    GameOver,   // 죽었는데 기회도 남아있지 않아서 게임오버
    FirstSceneMove, // 타이틀 씬에서 메인 게임 씬 갈 경우
}