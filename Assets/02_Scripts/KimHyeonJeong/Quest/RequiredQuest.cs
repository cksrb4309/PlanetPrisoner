using PlasticGui.WorkspaceWindow.QueryViews;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RequiredQuest : MonoBehaviour
{
    Dictionary<string, int> quests = new Dictionary<string, int>
    {
        { "큐브", 3},
        { "스피어", 1},
        { "큐브", 1 }
    };

    [SerializeField] string currentQuest; // 현재 진행중인 퀘스트
    [SerializeField] int currentProgress; // 현재 진행중인 퀘스트 진행도?
    [SerializeField] TMP_Text questText;

    void Start()
    {
        UpdateQuest();
    }

    public void UpdateQuest()
    {
        currentProgress = 0; // 진행도 0으로 초기화
        int randomIndex=Random.Range(0,quests.Count);
        int i = 0;
        foreach (var quest in quests) // 딕셔너리 하나씩 훑기
        {
            if (i == randomIndex)
            {
                currentQuest = $"[필수퀘스트] {quest.Key} currentProgress/{quest.Value}";
                questText.text=currentQuest;
            }
            i++;
        }
    }
}
