using UnityEngine;
using TMPro;

public class InGameTime : MonoBehaviour
{
    [SerializeField] Light directionalLight;
    [SerializeField] TMP_Text timeText;

    [SerializeField] float inGameTime;
    float updateInterval = 30f;
    [SerializeField] float timeCounter;

    [SerializeField] RequiredQuest requiredQuest;
    
    void Start()
    {
        inGameTime = 7 * 3600; // 07:00
        timeCounter = 0;
        UpdateTimeText();
    }

    void Update()
    {
        timeCounter += Time.deltaTime;
        inGameTime += Time.deltaTime * 60; // 10배 빠르게 시간흐름
        if (timeCounter>= updateInterval)
        {
            timeCounter = 0;
            UpdateTimeText();
        }

        if(inGameTime >= 86400) // 24시간 지나면
        {
            inGameTime = 0f;
            if (requiredQuest.questCompeleted) // 퀘스트 성공했는지 여부
            {
                // TODO: 패널티
            }
            requiredQuest.UpdateQuest(); // 퀘스트 업데이트
        }

        UpdateLightPosition();
    }

    void UpdateTimeText()
    {
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
}
