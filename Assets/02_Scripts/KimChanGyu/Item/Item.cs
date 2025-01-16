using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    public virtual string TooltipText => "줍기"; // 상호작용 UI 텍스트

    public ItemData itemData = null; // 아이템 정보

    new MeshRenderer renderer = null; // 렌더러
    new Collider collider = null; // 콜라이더
    new Rigidbody rigidbody = null; // 강체

    private void Start()
    {
        // GetComponent로 필요한 클래스 참조 가져오기
        collider = GetComponent<Collider>();
        renderer = GetComponent<MeshRenderer>();
        rigidbody = GetComponent<Rigidbody>();
    }
    public void Interact() // 상호작용
    {
        // 인벤토리로 아이템 전송
        PlayerInventory.Instance.AddItemToInventory(this);
    }
    public void DisableInHand() // 아이템 숨기기
    {
        // 렌더 숨기기
        renderer.enabled = false;

        // 충돌체 비활성화
        collider.enabled = false;

        // 강체 isKinematic 활성화
        rigidbody.isKinematic = true;
    }
    public void EnableInHand() // 아이템 손에 들고 있는 상태로 활성화
    {
        // 렌더 보이기
        renderer.enabled = true;

        // 충돌체 비활성화
        collider.enabled = false;

        // 강체 isKinematic 활성화
        rigidbody.isKinematic = false;
    }
    public void Activate() // 활성화
    {
        // 렌더 보이기
        renderer.enabled = true;

        // 충돌체 활성화
        collider.enabled = true;

        // 강체 isKinematic 비활성화
        rigidbody.isKinematic = false;
    }
}