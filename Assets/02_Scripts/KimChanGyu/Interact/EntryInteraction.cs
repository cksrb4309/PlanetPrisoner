using UnityEngine;

public class EntryInteraction : MonoBehaviour, IInteractable
{
    public string TooltipText => "입장(E)";

    [SerializeField] EntryInteraction pair = null;

    public Transform movePosition;

    public void Interact()
    {
        Debug.Log("현재 : " + gameObject.transform.parent.gameObject.name + " / position : " + movePosition.ToString());
        Debug.Log("다음 : " + pair.movePosition.parent.gameObject.name + " / position : " + pair.movePosition.ToString());

        PlayerItemHandler.Instance.GetComponent<PlayerController>().MovePosition(pair.movePosition);
    }
}
