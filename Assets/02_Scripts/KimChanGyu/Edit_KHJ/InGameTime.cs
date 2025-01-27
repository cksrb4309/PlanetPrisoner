using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class InGameTime : MonoBehaviour
{
    [SerializeField] GameManager gameManager;

    [SerializeField] Light directionalLight;
    [SerializeField] TMP_Text timeText;

    [SerializeField] float inGameTime;
    [SerializeField] float timeCounter;
    [SerializeField] float morningTime = 25200f; // 07:00

    float updateInterval = 30f;

    [SerializeField] RequiredQuest requiredQuest;

    [SerializeField] CanvasGroup fadeUI;
    [SerializeField] TMP_Text d_dayText;


    [SerializeField] DayChangeInteractable dayChangeInteractable;

    float timeSpeed = 60f;

    void Start()
    {
        inGameTime = morningTime; // 07:00
        timeCounter = 0;
        UpdateTimeText();
    }

    void Update()
    {
        timeCounter += Time.deltaTime;
        inGameTime += Time.deltaTime * timeSpeed;

        if (timeCounter >= updateInterval)
        {
            timeCounter = 0;
            UpdateTimeText();
            SpawnManager.Instance.OnSpawnCheck(inGameTime);
            if (inGameTime >= 79200)
            {
                Debug.Log("79200 확인 : " + inGameTime.ToString());

                dayChangeInteractable.EnableSleepTime();
            }
        }

        if (inGameTime >= 86400) // 24시간 지나면
        {
            inGameTime = 0f; // 시간 초기화
            UpdateTimeText();
            DayChangeSetting();
        }

        UpdateLightPosition();
    }

    void UpdateTimeText()
    {
        // InGameUI속 시간 text 설정
        int hours = Mathf.FloorToInt(inGameTime / 3600);
        int minutes = Mathf.FloorToInt((inGameTime % 3600) / 60);
        timeText.text = string.Format("{0:D2}:{1:D2}", hours, minutes);
    }

    void UpdateLightPosition()
    {
        float dayProgress = (inGameTime % 86400) / 86400f; // 시간 흐름 비율(0~1)
        float rotationAngle = 270f - dayProgress * 360f; // 0도에서 360도까지

        // directionalLight의 x회전값 업데이트
        directionalLight.transform.rotation = Quaternion.Euler(rotationAngle, 0, 0);
    }
    #region 날짜와 퀘스트 셋팅
    public void DayChangeSetting()
    {
        if (gameManager.d_days > 0)
        {
            gameManager.d_days -= 1;
            if (gameManager.d_days == 0)
            {
                d_dayText.text = "D - day";

            }
            else
            {
                d_dayText.text = $"D - {gameManager.d_days}";
            }

            StartCoroutine(FadeInOutCoroutine(1f)); // d-day화면 fadeIn/out
        }
        else
        {
            // TODO : 씬 변경
        }

        if (requiredQuest.questCompeleted) // 퀘스트 성공했는지 여부
        {
            // TODO: 패널티
        }
        requiredQuest.UpdateQuest(); // 퀘스트 업데이트
    }
    #endregion

    public void OnClickedSkipNight()
    {
        inGameTime = 7 * 3600; // 오전 7시로 변경
        UpdateTimeText();
        DayChangeSetting();
    }


    #region 1초 fadeIn 1초 fadeOut 코루틴
    IEnumerator FadeInOutCoroutine(float fadeDuration)
    {
        fadeUI.gameObject.SetActive(true);

        float fadeCount = 0f;
        while (fadeCount < fadeDuration)
        {
            fadeCount += Time.deltaTime; // 시간에 따라 증가
            float alpha = Mathf.Clamp01(fadeCount / fadeDuration); // 0에서 1 사이의 값으로 클램프
            fadeUI.alpha = alpha; // CanvasGroup의 alpha 값을 설정
            yield return null; // 다음 프레임까지 대기
        }
        fadeUI.alpha = 1f;

        yield return new WaitForSeconds(2f); // 2초 대기

        fadeCount = 0f; 
        while (fadeCount < fadeDuration)
        {
            fadeCount += Time.deltaTime;
            float alpha = Mathf.Clamp01(1 - (fadeCount / fadeDuration));
            fadeUI.alpha = alpha;
            yield return null;
        }
        fadeUI.alpha = 0f;

        fadeUI.gameObject.SetActive(false);
    }
    #endregion

    #region 시간 갱신 시의 필요 함수 작성 (김찬규)

    // 스폰 매니저 이벤트 발생
    void OnSpawnManagerEvent()
    {
        SpawnManager.Instance.OnSpawnCheck(inGameTime);
    }

    #endregion
}
