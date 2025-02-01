using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopComputer : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject shopUI; // 상점 UI
    [SerializeField] GameObject inGameUI; // 인게임 UI 

    public string TooltipText => "상호작용 [E]";

    void Start()
    {
    
    }

    public void Interact()
    {
        //Debug.Log("함수 진입");
        //player.canMove = false;
        inGameUI.SetActive(false);
        shopUI.SetActive(true);

        CursorController.EnableCursor();
        PlayerItemHandler.Instance.GetComponent<PlayerController>().DisableMovement();
    }

    
}
