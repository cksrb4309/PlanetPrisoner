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
    [SerializeField] int totalProgress; // 현재 진행중인 퀘스트 요구사항
    [SerializeField] int currentProgress; // 현재 진행중인 퀘스트 진행도?
    [SerializeField] TMP_Text questText;
    [SerializeField] Transceiver transceiver;

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
                totalProgress = quest.Value;
                QuestProgressTextUpdate();  // 텍스트 업데이트
            }
            i++;
        }
    }

    public void CheckQuestItem(ItemData itemData, string checkPoint)
    {
        if(currentQuest == "큐브")
        {
            if(itemData.itemName == "CubeItem")
            {
                if (checkPoint == "In") currentProgress++;
                else currentProgress--;
                QuestProgressTextUpdate(); // 텍스트 업데이트
            }
        }
        else if(currentQuest == "스피어")
        {

        }
        else if (currentQuest == "캡슐")
        {

        }
    }

    public void QuestCompleted() // 퀘스트 완료 (퀘스트 물품 전송 시 호출)
    {
        if (totalProgress == currentProgress) // 현재 퀘스트 수량 도달
        {
            questCompeleted = true;
        }
    }

    void QuestProgressTextUpdate()
    {
        questText.text = $"<color=#009006>[필수퀘스트]</color> {currentQuest} {currentProgress}/{totalProgress}";
    }
}
