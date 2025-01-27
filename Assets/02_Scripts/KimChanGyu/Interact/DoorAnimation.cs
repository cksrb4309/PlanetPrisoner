using System.Collections;
using UnityEngine;

public class DoorAnimation : MonoBehaviour, IInteractable
{
    public string TooltipText => isOpen == true ? "´Ý±â(E)" : "¿­±â(E)";

    public DoorAnimation pair = null;

    [SerializeField] Transform doorTransform;

    [SerializeField] Transform closePosition;
    [SerializeField] Transform openPosition;

    [SerializeField] float speed = 1f;

    bool isOpen = false;

    [HideInInspector] public float value = 1f;

    float Value
    {
        get => value;
        set
        {
            this.value = value;

            pair.value = value;
        }
    }
    bool IsOpen
    {
        get => isOpen;
        set
        {
            isOpen = value;

            pair.isOpen = value;
        }
    }

    Coroutine coroutine = null;
    private void Awake()
    {
        IsOpen = Vector3.Magnitude(doorTransform.position - closePosition.position) > Vector3.Magnitude(doorTransform.position - openPosition.position);
    }
    public void Interact()
    {
        if (coroutine != null) StopCoroutine(coroutine);

        if (IsOpen) coroutine = StartCoroutine(CloseCoroutine());

        else coroutine = StartCoroutine(OpenCoroutine());

        IsOpen = !IsOpen;
    }
    IEnumerator OpenCoroutine()
    {
        for (; Value > 0f; Value -= Time.deltaTime * speed)
        {
            doorTransform.position = Vector3.Lerp(openPosition.position, closePosition.position, value);

            yield return null;
        }
        doorTransform.position = openPosition.position;
    }
    IEnumerator CloseCoroutine()
    {
        for (; Value < 1f; Value += Time.deltaTime * speed)
        {
            doorTransform.position = Vector3.Lerp(openPosition.position, closePosition.position, Value);

            yield return null;
        }
        doorTransform.position = closePosition.position;
    }
}
