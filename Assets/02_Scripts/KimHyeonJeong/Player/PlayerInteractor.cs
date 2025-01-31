using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] PlayerController player;

    public UIManager interactionGuideTextUI;
    public InputActionReference interactionInputAction;

    public Transform cameraTransform;

    public LayerMask layerMask;

    public float range = 1f;

    Ray ray = new Ray();

    IInteractable interactable = null;

    private void OnEnable()
    {
        interactionInputAction.action.Enable();
    }
    private void OnDisable()
    {
        interactionInputAction.action.Disable();
    }

    private void Update()
    {
        ray.origin = cameraTransform.position;
        ray.direction = cameraTransform.forward;

        if (Physics.Raycast(ray, out RaycastHit hit, range, layerMask))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (this.interactable == null)
            {
                this.interactable = interactable;

                interactionGuideTextUI.OnShowInteractText(this.interactable.TooltipText);
            }
            else if (this.interactable != null && this.interactable != interactable)
            {
                this.interactable = interactable;

                interactionGuideTextUI.OnShowInteractText(this.interactable.TooltipText);
            }
        }
        else
        {
            if (this.interactable != null)
            {
                this.interactable = null;

                interactionGuideTextUI.OnHideInteractText();
            }
        }


        if (interactionInputAction.action.WasPressedThisFrame() && this.interactable != null)
        {
            interactionGuideTextUI.OnHideInteractText();

            this.interactable.Interact();

            this.interactable = null;
        }
    }
}
