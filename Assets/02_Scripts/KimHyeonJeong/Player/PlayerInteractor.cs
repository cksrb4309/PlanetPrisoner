using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    public Transform cameraTransform;

    // public LayerMask layerMask;

    public float range = 1f;

    Ray ray = new Ray();

    IInteractable interactable = null;

    [SerializeField] PlayerInputActions playerInput;
    [SerializeField] InputAction inputAction;
    [SerializeField] PlayerController player;

    private void Awake()
    {
        playerInput = new PlayerInputActions(); // PlayerInputActions 초기화
        inputAction = playerInput.Player.Interaction; // "Interaction" 액션 가져오기
    }

    private void OnEnable()
    {
        inputAction.performed += OnInteraction;
        inputAction.Enable();
    }

    private void OnDisable()
    {
        inputAction.performed -= OnInteraction;
        inputAction.Enable();
    }

    private void Update()
    {
        ray.origin = cameraTransform.position;
        ray.direction = cameraTransform.forward;

        // 충돌 O
        if (Physics.Raycast(ray, out RaycastHit hit, range/*, layerMask*/)) // 레이어 마스크 현재 적용 X
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null) 
            {
                if (this.interactable == null || this.interactable != interactable)
                { // 현재 상호작용 중인 오브젝트가 없을 때 or 감지한 오브젝트가 현재 상호작용 중인 오브젝트와 다를 때
                    this.interactable = interactable;

                    //Debug.Log(this.interactable);
                    
                    // UI Tooltip 꺼내기
                    UIManager.Instance.OnShowInteractText(interactable.TooltipText);
                }
            }
            else
            {
                this.interactable = null;
                UIManager.Instance.OnHideInteractText();
            }
        }

        // 충돌 X
        else
        {
            this.interactable = null;
            UIManager.Instance.OnHideInteractText();
        }
    }

    private void OnInteraction(InputAction.CallbackContext context)
    {
        if(interactable != null)
        {
            player.canMove = false;
            interactable.Interact(); // 상호작용 메서드 호출
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(Camera.main.transform.position, Camera.main.transform.position + Camera.main.transform.forward * 2f);
    }
}
