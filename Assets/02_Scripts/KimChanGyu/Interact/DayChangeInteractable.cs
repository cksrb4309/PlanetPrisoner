using UnityEngine;

public class DayChangeInteractable : MonoBehaviour, IInteractable
{
    public string TooltipText => sleepTimeTrigger ? "Àá ÀÚ±â(E)" : "";

    [SerializeField] InGameTime inGameTime;

    bool sleepTimeTrigger = false;

    public void Interact()
    {
        if (!sleepTimeTrigger) return;

        inGameTime.DayChangeSetting();

        DisableSleepTime();
    }
    private void OnEnable()
    {
        NextDayController.Subscribe(DisableSleepTime, ActionType.NextDayTransition);
    }
    private void OnDisable()
    {
        NextDayController.Unsubscribe(DisableSleepTime, ActionType.NextDayTransition);
    }
    public void EnableSleepTime() => sleepTimeTrigger = true;
    public void DisableSleepTime() => sleepTimeTrigger = false;
}
