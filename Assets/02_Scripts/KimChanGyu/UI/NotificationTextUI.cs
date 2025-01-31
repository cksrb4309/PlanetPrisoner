using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class NotificationTextUI : MonoBehaviour
{
    public static NotificationTextUI Instance => instance;

    static NotificationTextUI instance = null;

    public TextSet[] notificationTexts;

    public TMP_ColorGradient[] colorGradients;

    Coroutine[] coroutines = new Coroutine[3] { null, null, null };

    int index = 0;

    private void Awake()
    {
        instance = this;
    }

    public void NotificationText(string message, bool isWarning = true) // 경고성 메세지 띄우기
    {
        notificationTexts[index].Set(message, colorGradients[isWarning ? 0 : 1]);

        // 내가 속한 자식계층 구조에서 가장 하단에 배치
        notificationTexts[index].SetAsLastSibling();

        // 오브젝트 활성화
        notificationTexts[index].SetActive(true);

        if (coroutines[index] != null) StopCoroutine(coroutines[index]);

        coroutines[index] = StartCoroutine(DelayAndDisableCoroutine(index));

        index++;

        if (index == notificationTexts.Length) index = 0;
    }

    IEnumerator DelayAndDisableCoroutine(int index)
    {
        yield return new WaitForSecondsRealtime(2f);

        notificationTexts[index].SetActive(false);
    }
}
[Serializable]
public struct TextSet
{
    public TMP_Text front;
    public TMP_Text back;

    public void Set(string message, TMP_ColorGradient colorGradient)
    {
        front.text = message;
        back.text = message;

        front.colorGradientPreset = colorGradient;
    }
    public void Set(string message)
    {
        front.text = message;
        back.text = message;
    }
    public void SetActive(bool active) => back.gameObject.SetActive(active);
    public void SetAsLastSibling() => back.transform.SetAsLastSibling();
}