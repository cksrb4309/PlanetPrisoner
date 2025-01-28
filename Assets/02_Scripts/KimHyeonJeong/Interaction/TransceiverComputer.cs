using System.Collections;
using UnityEngine;

public class TransceiverComputer : MonoBehaviour, IInteractable
{
    [SerializeField] private Transceiver transceiver;

    public string TooltipText => "전송하기 [E]";


    public void Interact()
    {
        StartCoroutine(SendingEffect()); // 전송 이펙트 실행
    }

    private IEnumerator SendingEffect()
    {
        transceiver.TurnOnEmission(); // Emission 켜기

        yield return new WaitForSeconds(2f); // 2초 대기

        transceiver.TurnOffEmission(); // Emission 끄기

        transceiver.DestroyObjectInTransceiver(); // 전송기 안에 모든 오브젝트 destroy
    }
}
