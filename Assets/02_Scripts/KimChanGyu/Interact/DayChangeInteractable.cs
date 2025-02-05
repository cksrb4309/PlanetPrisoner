using UnityEngine;

public class DayChangeInteractable : MonoBehaviour, IInteractable
{
    public string TooltipText => sleepTimeTrigger ? "Àá ÀÚ±â [E]" : "";

    [SerializeField] InGameTime inGameTime;

    [SerializeField] PlayerController playerController;

    bool sleepTimeTrigger = false;
    bool isTriggered = false;
    public void Interact()
    {
        if (!sleepTimeTrigger) return;

        if (isTriggered) return;

        GameManager.d_days--;

        isTriggered = true;

        inGameTime.DayChangeSetting();

        DisableSleepTime();

        playerController.DisableMovement();
    }
    private void OnEnable()
    {
        NextDayController.Subscribe(DisableSleepTime, ActionType.NextDayTransition);
        NextDayController.Subscribe(EnableTrigger, ActionType.SurviveFinished);
        NextDayController.Subscribe(EnableTrigger, ActionType.NextDayFinished);
    }
    private void OnDisable()
    {
        NextDayController.Unsubscribe(DisableSleepTime, ActionType.NextDayTransition);
        NextDayController.Unsubscribe(EnableTrigger, ActionType.SurviveFinished);
        NextDayController.Unsubscribe(EnableTrigger, ActionType.NextDayFinished);
    }
    public void EnableSleepTime() => sleepTimeTrigger = true;
    public void DisableSleepTime() => sleepTimeTrigger = false;
    void EnableTrigger() => isTriggered = false;
}
