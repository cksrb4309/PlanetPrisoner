using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComputerInteract : MonoBehaviour, IInteractable
{
    [SerializeField] PlayerController playerController;
    [SerializeField] GameObject shopUI; // 상점 UI
    [SerializeField] GameObject inGameUI; // 인게임 UI 

    public string TooltipText => "상호작용 [E]";

    public void Interact()
    {
        playerController.DisableMovement();

        inGameUI.SetActive(false);
        shopUI.SetActive(true);

        CursorController.EnableCursor();
    }
}
