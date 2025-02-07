using UnityEngine;

public class DayChangeInteractable : MonoBehaviour, IInteractable
{
    public string TooltipText => "잠 자기 [E]";

    [SerializeField] InGameTime inGameTime;

    [SerializeField] PlayerController playerController;

    bool sleepTimeTrigger = false;
    bool isTriggered = false;
    public void Interact()
    {
        if (!sleepTimeTrigger) 
        {
            NotificationTextUI.Instance.NotificationText("23:00부터 잘 수 있습니다");

            return;
        }


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
