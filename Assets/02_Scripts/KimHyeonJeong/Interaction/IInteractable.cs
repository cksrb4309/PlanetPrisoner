using UnityEngine;

public interface IInteractable
{
    public void Interact();
    public string TooltipText { get; }
}