using UnityEngine;

public class CraftMachine : MonoBehaviour, IInteractable
{
    [SerializeField] Transceiver transceiver;

    // 전송기에 오브젝트가 들어있다면 제작하기 UI가 뜨게하고 아니면 아무것도 뜨지 않도록
    public string TooltipText => transceiver.objectInTransceiver.Count > 0 ? "제작하기 [E]" : "";

    private bool canUseCraftMachine;

    public void Interact()
    {
        
    }

}
