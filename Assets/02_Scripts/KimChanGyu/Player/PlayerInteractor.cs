using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    public InteractionGuideTextUI interactionGuideTextUI;

    public Transform cameraTransform;

    public LayerMask layerMask;

    public float range = 1f;

    Ray ray = new Ray();

    IInteractable interactable = null;

    private void Update()
    {
        ray.origin = cameraTransform.position;
        ray.direction = cameraTransform.forward;

        // Ãæµ¹ O
        if (Physics.Raycast(ray, out RaycastHit hit, range, layerMask))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                if (this.interactable == null)
                {
                    this.interactable = interactable;

                    // UI Tooltip ²¨³»±â
                    interactionGuideTextUI.ShowInteractText(interactable.TooltipText);
                }

                else if (this.interactable != interactable)
                {
                    this.interactable = interactable;

                    // UI Tooltip ²¨³»±â
                    interactionGuideTextUI.ShowInteractText(interactable.TooltipText);
                }
            }
            else
            {
                interactionGuideTextUI.HideInteractText();
            }
        }

        // Ãæµ¹ X
        else
        {
            if (interactable != null)
            {
                interactionGuideTextUI.HideInteractText();
            }
        }
    }
}
