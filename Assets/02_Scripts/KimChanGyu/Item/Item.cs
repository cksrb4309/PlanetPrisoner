using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    public virtual string TooltipText => itemData.guideText; // 상호작용 UI 텍스트

    public ItemData itemData = null; // 아이템 정보

    public Transform rayStartPosition;

    new MeshRenderer renderer = null; // 렌더러
    new Collider collider = null; // 콜라이더
    new Rigidbody rigidbody = null; // 강체

    private void Start()
    {
        // GetComponent로 필요한 클래스 참조 가져오기
        collider = GetComponentInChildren<Collider>();
        renderer = GetComponentInChildren<MeshRenderer>();
        rigidbody = GetComponentInChildren<Rigidbody>();
    }
    public void Interact() // 상호작용
    {
        Debug.Log("D");

        // 인벤토리로 아이템 전송
        PlayerInventory.Instance.AddItemToInventory(this);

        PlayerItemHandler.Instance.EquipItem();
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
    public void EnableInHand(Transform parent) // 아이템 손에 들고 있는 상태로 활성화
    {
        // 부모 설정
        transform.parent = parent;

        transform.position = parent.transform.position;

        transform.rotation = parent.transform.rotation;

        // 렌더 보이기
        renderer.enabled = true;

        // 충돌체 비활성화
        collider.enabled = false;

        // 강체 isKinematic 활성화
        rigidbody.isKinematic = false;
    }
    public void Activate() // 활성화
    {
        // 부모 해제
        transform.parent = null;

        StartCoroutine(DropItemCoroutine());

        // 장착 해제 시의 회전값 초기화
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        // 렌더 보이기
        renderer.enabled = true;

        // 충돌체 활성화
        collider.enabled = true;

        // 강체 isKinematic 비활성화
        rigidbody.isKinematic = false;
    }
    IEnumerator DropItemCoroutine()
    {
        LayerMask layerMask = LayerMask.GetMask("Ground");

        while (!Physics.Raycast(rayStartPosition.position, Vector3.down, 0.01f, layerMask))
        {
            transform.position += Time.deltaTime * itemData.itemDropSpeed * Vector3.down;

            yield return null;
        }
    }
}