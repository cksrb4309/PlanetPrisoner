using UnityEngine;
using UnityEngine.InputSystem;

public class MainGameSceneMenuUI : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;

    [SerializeField] InputActionReference cancelButtonInputAction;

    bool isStop = false;

    private void OnEnable()
    {
        cancelButtonInputAction.action.Enable();

        isStop = false;

        Pause(isStop);
    }
    private void OnDisable()
    {
        cancelButtonInputAction.action.Disable();
    }

    private void Update()
    {
        if (cancelButtonInputAction.action.WasCompletedThisFrame())
        {
            isStop = !isStop;

            Pause(isStop);
        }
    }
    void Pause(bool isStop)
    {
        canvasGroup.alpha = isStop ? 1f : 0f;
        canvasGroup.blocksRaycasts = isStop;
        Time.timeScale = isStop ? 0f : 1f;

        if (isStop) CursorController.EnableCursor();
        else CursorController.DisableCursor();
    }
    public void OnResumeButton()
    {
        isStop = false;

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        Time.timeScale = 1f;

        CursorController.DisableCursor();
    }
    public void OnQuitButton()
    {
        Application.Quit();
    }
}
