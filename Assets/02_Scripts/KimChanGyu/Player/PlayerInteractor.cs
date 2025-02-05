using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    public InteractionGuideTextUI interactionGuideTextUI;

    public InputActionReference interactionInputAction;

    public Transform cameraTransform;

    public LayerMask layerMask;

    public float range = 1f;

    bool isEnabled = false;

    Ray ray = new Ray();

    IInteractable interactable = null;

    private void OnEnable()
    {
        interactionInputAction.action.Enable();

        NextDayController.Subscribe(EnableInteractor, ActionType.FirstGameFinished);
        NextDayController.Subscribe(EnableInteractor, ActionType.NextDayFinished);
        NextDayController.Subscribe(EnableInteractor, ActionType.SurviveFinished);

        NextDayController.Subscribe(DisableInteractor, ActionType.NextDayReady);
        NextDayController.Subscribe(DisableInteractor, ActionType.OnPlayerDie);
    }
    private void OnDisable()
    {
        interactionInputAction.action.Disable();

        NextDayController.Unsubscribe(EnableInteractor, ActionType.FirstGameFinished);
        NextDayController.Unsubscribe(EnableInteractor, ActionType.NextDayFinished);
        NextDayController.Unsubscribe(EnableInteractor, ActionType.SurviveFinished);

        NextDayController.Unsubscribe(DisableInteractor, ActionType.NextDayReady);
        NextDayController.Unsubscribe(DisableInteractor, ActionType.OnPlayerDie);
    }

    void DisableInteractor()
    {
        if (interactable != null)
        {
            interactable = null;
        }
        interactionGuideTextUI.HideInteractText();

        isEnabled = false;
    }
    void EnableInteractor()
    {
        isEnabled = true;
    }

    private void Update()
    {
        if (!isEnabled) return;

        ray.origin = cameraTransform.position;
        ray.direction = cameraTransform.forward;
        
        if (Physics.Raycast(ray, out RaycastHit hit, range, layerMask))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (this.interactable == null)
            {
                this.interactable = interactable;

                interactionGuideTextUI.ShowInteractText(this.interactable.TooltipText);
            }
            else if (this.interactable != null && this.interactable != interactable)
            {
                this.interactable = interactable;

                interactionGuideTextUI.ShowInteractText(this.interactable.TooltipText);
            }
        }
        else
        {
            if (this.interactable != null)
            {
                this.interactable = null;

                interactionGuideTextUI.HideInteractText();
            }
        }

        
        if (interactionInputAction.action.WasPressedThisFrame() && this.interactable != null)
        {
            interactionGuideTextUI.HideInteractText();

            this.interactable.Interact();

            this.interactable = null;
        }
    }
}
