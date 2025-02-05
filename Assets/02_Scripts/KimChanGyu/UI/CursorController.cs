using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CursorController : MonoBehaviour
{
    static CursorController instance = null;

    [SerializeField] Image cursorImage;

    [SerializeField] CanvasGroup cursorCanvasGroup;

    [SerializeField] InputActionReference mousePositionInputAction;

    Vector2 position;
    Vector2 screenSize;

    bool isView = false;
    private void OnEnable()
    {
        mousePositionInputAction.action.Enable();
    }
    private void OnDisable()
    {
        mousePositionInputAction.action.Disable();
    }
    private void Awake()
    {
        instance = this;

        isView = false;

        screenSize = new Vector2((float)Screen.width / 2f, (float)Screen.height / 2f);

        DisableCursor();
    }
    private void LateUpdate()
    {
        if (isView)
        {
            position = mousePositionInputAction.action.ReadValue<Vector2>();

            cursorImage.rectTransform.localPosition = position - screenSize;
        }
    }
    public static void DisableCursor()
    {
        instance.cursorCanvasGroup.alpha = 0f;
        instance.isView = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public static void EnableCursor()
    {
        instance.cursorCanvasGroup.alpha = 1f;
        instance.isView = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }
}
