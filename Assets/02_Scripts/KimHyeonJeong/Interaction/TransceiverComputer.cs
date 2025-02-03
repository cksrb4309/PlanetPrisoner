using System.Collections;
using UnityEngine;

public class TransceiverComputer : MonoBehaviour, IInteractable
{
    [SerializeField] private Transceiver transceiver;
    [SerializeField] private RequiredQuest requiredQuest;
    [SerializeField] private OxygenTank oxygenTank;

    public string TooltipText => "전송하기 [E]";


    public void Interact()
    {
        StartCoroutine(SendingEffect()); // 전송 이펙트 실행

        GiveReward(); // 아이템 리워드
        
        requiredQuest.QuestCompleted(); // 퀘스트 완료 여부 파악

        transceiver.DestroyObjectInTransceiver(); // 전송기 안에 모든 오브젝트 destroy
    }

    private IEnumerator SendingEffect()
    {
        transceiver.TurnOnEmission(); // Emission 켜기

        yield return new WaitForSeconds(2f); // 2초 대기

        transceiver.TurnOffEmission(); // Emission 끄기
    }

    private void GiveReward()
    {
        foreach (var obj in transceiver.objectInTransceiver)
        {
            Item item = obj.GetComponent<Item>();
            oxygenTank.ChangeOxygen(item.itemData.itemPrice);
        }
    }
}
