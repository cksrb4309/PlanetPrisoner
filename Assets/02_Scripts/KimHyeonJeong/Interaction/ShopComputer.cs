using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopComputer : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject shopUI; // 상점 UI
    [SerializeField] GameObject inGameUI; // 인게임 UI 

    public string TooltipText => "상호작용 [E]";

    public void Interact()
    {
        inGameUI.SetActive(false);
        shopUI.SetActive(true);
        Debug.Log("커서활성화 직전");
        CursorController.EnableCursor();
        PlayerItemHandler.Instance.GetComponent<PlayerController>().DisableMovement();
    }
}
