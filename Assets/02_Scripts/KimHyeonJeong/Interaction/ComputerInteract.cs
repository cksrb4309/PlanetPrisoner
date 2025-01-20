using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComputerInteract : MonoBehaviour, IInteractable
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
    }

    /*// Gizmo로 Raycast 경로 표시
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(Camera.main.transform.position, Camera.main.transform.position + Camera.main.transform.forward * 2f);
    }*/
}
