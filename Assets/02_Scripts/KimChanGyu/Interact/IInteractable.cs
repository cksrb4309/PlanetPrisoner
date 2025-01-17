using UnityEngine;

public interface IInteractable // 상호작용 인터페이스
{
    public void Interact(); // 상호작용
    public string TooltipText { get; } // 커서를 가져다 댔을 때 뜰 UI Text
}