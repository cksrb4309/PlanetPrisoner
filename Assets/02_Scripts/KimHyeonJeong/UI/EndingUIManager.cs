using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndingUIManager : MonoBehaviour
{
    public TMP_Text targetText;
    public AudioSource typingSound; // 타이핑 소리
    public AudioSource stampSound; // 스탬프 소리
    private float delay = 0.125f;
    private float cursorBlinkDelay = 0.4f; // 커서 깜박임 간격
    private string cursor = "ㅣ"; // 커서 문자
    public Image approvedImg; // 승인 도장 이미지

    [SerializeField] Animator spaceShipAnimator;

    private string[] messages = {
        "\n>> 귀환 허가서 제출",
        "\n>> 귀환 허가서 검토중 . . .",
        "\n>> 우주선 이륙 준비중 . . .",
        "\n>> 승인 완료"
    };

    void Start()
    {
        // 투명하게 설정
        Color color = approvedImg.color;
        color.a = 0f;
        approvedImg.color = color; 

        // 기본 텍스트 설정
        targetText.text = "Government\n--------------------------------------";
        StartCoroutine(PrintMessages());
    }

    IEnumerator PrintMessages()
    {
        foreach (string message in messages)
        {
            yield return StartCoroutine(TypeText(message));
            yield return StartCoroutine(BlinkCursor()); // 커서 깜박임
            yield return new WaitForSeconds(1f); // 메시지 간의 간격
        }
        // 소리와 싱크 맞추기 위해서 SetActive대신 알파값 조정으로 사용
        stampSound.Play();
        yield return new WaitForSeconds(0.3f);
        Color color = approvedImg.color;
        color.a = 255f;
        approvedImg.color = color;

        yield return new WaitForSeconds(3f);
        spaceShipAnimator.SetTrigger("GoHome");
    }

    IEnumerator BlinkCursor()
    {
        // 커서 깜박임 효과
        float elapsedTime = 0f;
        while (elapsedTime < 2f) // 2초 동안 깜박이기
        {
            targetText.text += cursor; // 커서 추가
            yield return new WaitForSeconds(cursorBlinkDelay);
            targetText.text = targetText.text.TrimEnd(cursor.ToCharArray()); // 커서 제거
            yield return new WaitForSeconds(cursorBlinkDelay);
            elapsedTime += cursorBlinkDelay * 2; // 깜박인 시간 추가
        }
    }

    IEnumerator TypeText(string message)
    {
        // 텍스트 추가
        targetText.text += "\n";
        string textToType = message.Substring(0, 3); // ">> " 부분 추가
        targetText.text += textToType; // ">> " 먼저 출력

        // ">> " 이후의 텍스트를 한 글자씩 출력
        for (int i = 3; i < message.Length; i++)
        {
            targetText.text += message[i];
            typingSound.Play();
            yield return new WaitForSeconds(delay);
        }
    }
}