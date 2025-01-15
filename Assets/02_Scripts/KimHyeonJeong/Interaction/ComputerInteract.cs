using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComputerInteract : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject interactionText; // InGameUI에 뜰 상호작용 텍스트
    [SerializeField] GameObject shopUI; // 상점 UI
    [SerializeField] GameObject inGameUI; // 인게임 UI 
    [SerializeField] PlayerController player;

    public string TooltipText => "상호작용 [E]";

    void Start()
    {
        interactionText.SetActive(false); // 처음에 상호작용 텍스트 비활성화
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 2f))
        { // 최대 2m까지 Ray를 쏴서 뭔가를 감지할 경우
            if (hit.collider.CompareTag("Computer"))
            { // 맞은게 컴퓨터라면
                interactionText.SetActive(true); // 텍스트 활성화
                interactionText.GetComponent<TMP_Text>().text = TooltipText; // 알맞은 상호작용 텍스트로 변경

                if (Input.GetKeyDown(KeyCode.E))
                { // 그때 상호작용 E키를 누르면
                    Interact(); // 함수 호출
                }
            }
            else // 맞은게 컴퓨터가 아닐 때
            {
                interactionText.SetActive(false);
            }
        }
        else // Raycast가 아무것도 감지하지 못할 경우
        {
            interactionText.SetActive(false);
        }
    }

    public void Interact()
    {
        //Debug.Log("함수 진입");
        player.canMove = false;
        inGameUI.SetActive(false);
        shopUI.SetActive(true);
    }

    // Gizmo로 Raycast 경로 표시
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(Camera.main.transform.position, Camera.main.transform.position + Camera.main.transform.forward * 2f);
    }
}
