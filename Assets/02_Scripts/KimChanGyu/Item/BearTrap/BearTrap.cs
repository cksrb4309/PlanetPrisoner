using System.Collections;
using UnityEngine;

public class BearTrap : Item
{
    public override string TooltipText => isUsed ? string.Empty : !isSet ? "Get(E)" : isBeingSet ? "install(E)" : "Danger";

    public MeshRenderer meshRenderer;

    Material material;

    BearTrapHelper helper = null;

    int needInstallCount = 3;
    int currInstallCount = 0;

    bool isSet = false; // 설치되었는지에 대한 트리거 변수
    bool isUsed = false;
    bool isBeingSet = false;

    private void Awake()
    {
        material = meshRenderer.material;

        material.SetFloat("_TrapOpenAmount", 0.5f);

        helper = GetComponentInChildren<BearTrapHelper>();
    }
    public void ActivateTrap()
    {
        // 부모 해제
        transform.parent = null;

        StartCoroutine(DropItemCoroutine());

        // 장착 해제 시의 회전값 초기화
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        // 충돌체 활성화
        collider.enabled = true;

        scanResultDisplay.EnableDisplay();

        if (!isUsed)
        {
            isSet = true;

            isBeingSet = true;

            material.SetFloat("_TrapOpenAmount", 0f);
        }
    }
    public override void ConsumeItem()
    {

    }
    public void CompleteAttack()
    {
        isUsed = true;

        scanResultDisplay.DisableDisplay();

        material.SetFloat("_TrapOpenAmount", 0f);
    }
    public override void Interact()
    {
        if (IsGetItem())
        {
            // 인벤토리로 아이템 전송
            PlayerInventory.Instance.AddItemToInventory(this);

            PlayerItemHandler.Instance.EquipItem();
        }
        else if (isBeingSet)
        {
            currInstallCount++;

            material.SetFloat("_TrapOpenAmount", ((float)currInstallCount / (float)needInstallCount));

            if (currInstallCount == needInstallCount)
            {
                isBeingSet = false;

                helper.ActivateHelper();
            }
        }
    }
    private void OnEnable()
    {
        material.SetFloat("_TrapOpenAmount", 0.5f);

        helper.InitHelper();

        currInstallCount = 0;

        isUsed = false;
        isBeingSet = false;
        isSet = false;
    }
    public override bool IsGetItem() => !isBeingSet && !isSet;
}
