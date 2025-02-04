using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        if (instance == null)
        {
            Debug.Log("Awake PlayerController");

            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("싱글톤 패턴에 따라 오브젝트 제거 : " + gameObject.name);

            Destroy(gameObject);
        }

        eventActions = new Action[Enum.GetValues(typeof(ActionType)).Length];

        for (int i = 0; i < eventActions.Length; i++) eventActions[i] = null;

        buttonCanvasGroup.interactable = false;
    }
    private void OnEnable()
    {
        Subscribe(ConnectReadyToNext, ActionType.NextDayReady);
    }
    private void OnDisable()
    {
        Unsubscribe(ConnectReadyToNext, ActionType.NextDayReady);
    }
    void ConnectReadyToNext() => FadeNextDayUI(NextDayOption.Next);
    public static void SceneStart() => instance.FadeNextDayUI(NextDayOption.FirstSceneMove);
    public void OnClickYesOrNo(bool isYes)
    {
        this.isYes = isYes;

        isInputReceived = true;
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
        // 다음날 전환 성공 시작 이벤트 액션 호출
        OnEventInvoke(ActionType.NextDayTransition);

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

        // 다음날 전환 성공 마무리 이벤트 액션 호출
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

        // 2초 지연
        yield return new WaitForSecondsRealtime(2f);

        // 텍스트 입력
        yield return TextTypingCoroutine(mainTextUI, 0.1f, "추가 설명이 필요하십니까?");

        // 입력 대기
        yield return WaitingInputActionCoroutine();

        // 추가 설명이 필요하다면
        if (isYes)
        {
            yield return TextTypingCoroutine(mainTextUI, 0.1f, "플레이어는 죄를 지은 사형수입니다");

            yield return new WaitForSecondsRealtime(1.5f);

            yield return TextTypingCoroutine(mainTextUI, 0.1f, "자원을 수집하여 산소를 공급받지 못하면");

            yield return new WaitForSecondsRealtime(1.5f);

            yield return TextTypingCoroutine(mainTextUI, 0.1f, "질식사로 인해 죽게 됩니다");

            yield return new WaitForSecondsRealtime(1.5f);

            yield return TextTypingCoroutine(mainTextUI, 0.1f, "타 행성에서 위험한 몹들을 조심하며");

            yield return new WaitForSecondsRealtime(1.5f);

            yield return TextTypingCoroutine(mainTextUI, 0.1f, "하루마다 필요한 납품 퀘스트를 수행해야 합니다");

            yield return new WaitForSecondsRealtime(1.5f);

            yield return TextTypingCoroutine(mainTextUI, 0.1f, "납품 퀘스트를 수행하지 않을 경우");

            yield return new WaitForSecondsRealtime(1.5f);

            yield return TextTypingCoroutine(mainTextUI, 0.1f, "산소 호흡기에 패널티를 가하여");

            yield return new WaitForSecondsRealtime(1.5f);

            yield return TextTypingCoroutine(mainTextUI, 0.1f, "숨 쉬는 것이 더욱 어려워집니다");

            yield return new WaitForSecondsRealtime(1.5f);

            yield return TextTypingCoroutine(mainTextUI, 0.1f, "상점을 통해 필요한 도구를 구입할 수도 있습니다");

            yield return new WaitForSecondsRealtime(1.5f);

            yield return TextTypingCoroutine(mainTextUI, 0.1f, $"{remainingDays}일 동안 잘 버티셨다면");

            yield return new WaitForSecondsRealtime(1.5f);

            yield return TextTypingCoroutine(mainTextUI, 0.1f, "지구로 돌려보내줄테니");

            yield return new WaitForSecondsRealtime(1.5f);

            yield return TextTypingCoroutine(mainTextUI, 0.1f, "여러 요소를 활용해 잘 살아남아주세요");
        }

        // 추가 설명이 필요없다면
        else
        {
            yield return TextTypingCoroutine(mainTextUI, 0.1f, "생존하시길 바라겠습니다");
        }

        // 지연
        yield return new WaitForSecondsRealtime(1.5f);

        // 텍스트 비우기
        mainTextUI.text = string.Empty;

        // 텍스트 캔버스 비활성화
        yield return FadeInOutCoroutine(textCanvasGroup, 1f, FadeType.BlinkDecrease);

        // 씬 이동
        yield return SceneMoveCoroutine();

        // 배경 캔버스 비활성화
        yield return FadeInOutCoroutine(backgroundCanvasGroup, 1f, FadeType.LinearDecrease);

        // 화면이 켜지고 게임 시작에 필요한 함수 호출
        OnEventInvoke(ActionType.FirstGameStart);
    }
    IEnumerator SceneMoveCoroutine()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("MainGameScene");

        while (!asyncOperation.isDone) yield return null;
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

    public static void TriggerEventInvoke(ActionType actionType) => instance.OnEventInvoke(actionType);

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
    FirstGameStart,
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