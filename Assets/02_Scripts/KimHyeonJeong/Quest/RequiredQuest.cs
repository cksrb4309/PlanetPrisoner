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
        { "캡슐", 1 }
    };

    [SerializeField] string currentQuest; // 현재 진행중인 퀘스트
    [SerializeField] int currentProgress; // 현재 진행중인 퀘스트 진행도?
    [SerializeField] TMP_Text questText;

    public bool questCompeleted;

    void Start()
    {
        UpdateQuest();
    }


    public void UpdateQuest()
    {
        questCompeleted = false; // 퀘스트 성공 여부 초기화
        currentProgress = 0; // 진행도 0으로 초기화
        int randomIndex = Random.Range(0, quests.Count);
        int i = 0;
        foreach (var quest in quests) // 딕셔너리 하나씩 훑기
        {
            if (i == randomIndex)
            {
                currentQuest = quest.Key;
                questText.text = $"[필수퀘스트] {quest.Key} {currentProgress}/{quest.Value}";
            }
            // TODO: 전송기에 놨을 때 퀘스트 아이템인지 확인하고 맞으면 수량 update하고 등등
            i++;
        }
    }

    void QuestCompleted() // 퀘스트 완료 (퀘스트 물품 얻을 때 호출)
    {
        if (quests[currentQuest] == currentProgress) // 현재 퀘스트 수량 도달
        {
            questCompeleted = true;
        }
    }
}
