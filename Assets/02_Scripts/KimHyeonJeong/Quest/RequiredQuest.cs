using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VInspector;

public class RequiredQuest : MonoBehaviour
{
    public SerializedDictionary<ItemData, int> questInfo;
    Dictionary<string, int> quests;

    /*Dictionary<string, int> quests = new Dictionary<string, int>
    {
        { "큐브", 3},
        { "스피어", 1},
        { "캡슐", 1 }
    };*/

    [SerializeField] string currentQuest; // 현재 진행중인 퀘스트
    [SerializeField] int totalProgress; // 현재 진행중인 퀘스트 요구사항
    [SerializeField] int currentProgress; // 현재 진행중인 퀘스트 진행도?
    [SerializeField] TMP_Text questText;
    [SerializeField] Transceiver transceiver;

    public bool questCompeleted;

    void Start()
    {
        quests=new Dictionary<string, int>();
        foreach(var quest in questInfo) quests.Add(quest.Key.itemName, quest.Value);
        // 원하는 퀘스트 itemData와 수량을 인스펙터에서 추가하면 퀘스트 리스트에 추가되는 형식

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
        if(currentQuest == itemData.itemName)
        {
            if (checkPoint == "In") currentProgress++; // 전송기에 들어왔을땐 +1
            else currentProgress--; // 전송기에서 나갔을 땐 -1
            QuestProgressTextUpdate(); // 텍스트 업데이트
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
        // 퀘스트 텍스트 업데이트
        questText.text = $"<color=#009006>[필수퀘스트]</color> {currentQuest} {currentProgress}/{totalProgress}";
    }
}
