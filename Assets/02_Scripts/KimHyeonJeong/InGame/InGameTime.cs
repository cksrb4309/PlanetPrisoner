using UnityEngine;
using UnityEngine.UI;

public class InGameTime : MonoBehaviour
{
    [SerializeField]
    Light directionalLight;
    [SerializeField]
    Text timeText;

    public float inGameTime;
    float updateInterval = 60f;
    public float timeCounter;

    
    void Start()
    {
        inGameTime = 7 * 3600; // 07:00
        timeCounter = 0;
        UpdateTimeText();
    }

    void Update()
    {
        timeCounter += Time.deltaTime;
        inGameTime += Time.deltaTime * 10; // 10배 빠르게 시간흐름
        if (timeCounter>= updateInterval)
        {
            timeCounter = 0;
            UpdateTimeText();
        }

        if(inGameTime >= 86400) // 24시간 지나면
        {
            inGameTime = 0f;
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
        if (inGameTime >= 330*60 && inGameTime < 1170*60) // 05:30 (330분)부터 19:30 (1170분)까지
        {
            directionalLight.enabled = true;

            float t = (inGameTime - 330 * 60) / (1170*60 - 330 * 60); // 0에서 1 사이의 값
            float angle = t * 360; // 전체 원을 그리기 위해 각도 계산
            float radius = 10f; // 원의 반지름
            float x = radius * Mathf.Cos(angle * Mathf.Deg2Rad); // x좌표
            float z = radius * Mathf.Sin(angle * Mathf.Deg2Rad); // z좌표

            // 조명 위치를 원을 그리며 이동
            directionalLight.transform.position = new Vector3(x, 10, z);
        }
        else
        {
            directionalLight.enabled = false; 
        }
    }
}
