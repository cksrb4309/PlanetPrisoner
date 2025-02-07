using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    public virtual string TooltipText => itemData.GuideText; // 상호작용 UI 텍스트

    public ItemData itemData = null; // 아이템 정보

    [SerializeField] Transform rayStartPosition;

    [SerializeField] float offsetY;

    protected new MeshRenderer renderer = null; // 렌더러
    protected new Collider collider = null; // 콜라이더

    protected ScanResultDisplay scanResultDisplay = null;

    protected bool isSubscribed = false;

    bool isInitialized = false;
    private void Start()
    {
        Init();
    }
    void Init()
    {
        if (isInitialized) return;

        isInitialized = true;

        // GetComponent로 필요한 클래스 참조 가져오기
        collider = GetComponent<Collider>();
        if (collider == null) collider = GetComponentInChildren<Collider>();

        renderer = GetComponentInChildren<MeshRenderer>();

        scanResultDisplay = GetComponentInChildren<ScanResultDisplay>();
    }
    public virtual void Interact() // 상호작용
    {
        if (!IsGetItem()) return;

        if (isSubscribed)
        {
            NextDayController.Unsubscribe(ItemDestroy, ActionType.NextDayTransition);

            isSubscribed = false;
        }

        // 인벤토리로 아이템 전송
        PlayerInventory.Instance.AddItemToInventory(this);

        PlayerItemHandler.Instance.EquipItem();
    }
    public virtual void DisableInHand() // 아이템 숨기기
    {
        // 렌더 숨기기
        renderer.enabled = false;

        // 충돌체 비활성화
        collider.enabled = false;

        scanResultDisplay.DisableDisplay();
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

        scanResultDisplay.DisableDisplay();
    }
    public virtual void ConsumeItem()
    {
        //renderer.enabled = false;
        //collider.enabled = false;
        //transform.parent = null;

        //scanResultDisplay.DisableDisplay();

        Destroy(gameObject);
    }
    public virtual void Activate() // 활성화
    {
        Init();

        // 부모 해제
        transform.parent = null;

        StartCoroutine(DropItemCoroutine());

        // 장착 해제 시의 회전값 초기화
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        // 렌더 보이기
        renderer.enabled = true;

        // 충돌체 활성화
        collider.enabled = true;

        // 스캔 가능 상태로 전환
        scanResultDisplay.EnableDisplay();

        NextSceneSetting();
    }
    protected IEnumerator DropItemCoroutine()
    {
        LayerMask layerMask = LayerMask.GetMask("Ground");
        layerMask |= LayerMask.GetMask("Default");

        Ray ray = new Ray(rayStartPosition.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, layerMask))
        {
            float goal = hitInfo.point.y - offsetY;

            float positionY = transform.position.y;

            Vector3 position = new Vector3(transform.position.x, positionY, transform.position.z);

            while (positionY > goal)
            {
                yield return null;

                positionY -= Time.deltaTime * itemData.itemDropSpeed;

                position.y = positionY;

                transform.position = position;
            }
            position.y = goal;

            transform.position = position;
        }
        else
        {

        }
    }
    public virtual bool IsGetItem() => true;
    void NextSceneSetting()
    {
        if (!HomeColliderChecker.Instance.IsObjectInsideBox(transform.position))
        {
            NextDayController.Subscribe(ItemDestroy, ActionType.NextDayTransition);

            isSubscribed = true;
        }
    }
    void ItemDestroy()
    {
        Destroy(gameObject);
    }
    private void OnDisable()
    {
        if (isSubscribed)
        {
            isSubscribed = false;

            NextDayController.Unsubscribe(ItemDestroy, ActionType.NextDayTransition);
        }
    }
}