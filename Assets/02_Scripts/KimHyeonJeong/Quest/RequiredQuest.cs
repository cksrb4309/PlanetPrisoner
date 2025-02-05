using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using VInspector;
public class RequiredQuest : MonoBehaviour
{
    public SerializedDictionary<ItemData, int> questInfo;

    Dictionary<string, int> quests;

    [SerializeField] string currentQuest; // 현재 진행중인 퀘스트
    [SerializeField] int currentProgress; // 현재 진행중인 퀘스트 진행도?
    [SerializeField] int totalProgress; // 현재 진행중인 퀘스트 요구사항
    [SerializeField] TMP_Text questText;
    [SerializeField] Transceiver transceiver;

    public bool questCompeleted;

    void Start()
    {
        quests = new Dictionary<string, int>();

        foreach (var quest in questInfo) quests.Add(quest.Key.itemName, quest.Value);

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
    public void QuestCompleted() // 퀘스트 완료 (퀘스트 물품 얻을 때 호출)
    {
        if (quests[currentQuest] == currentProgress) // 현재 퀘스트 수량 도달
        {
            questCompeleted = true;
        }
    }
    public void CheckQuestItem(ItemData itemData, string checkPoint)
    {
        Debug.Log($"아이템 검사 [Quest:{currentQuest}, {itemData.itemName} = {currentQuest.Equals(itemData.itemName)}]");

        if (currentQuest.Equals(itemData.itemName))
        {
            if (checkPoint == "In") currentProgress++;
            else currentProgress--;
            QuestProgressTextUpdate(); // 텍스트 업데이트
        }
    }


    void QuestProgressTextUpdate()
    {
        questText.text = $"<color=#009006>[필수퀘스트]</color> {currentQuest} {currentProgress}/{totalProgress}";
    }
}
