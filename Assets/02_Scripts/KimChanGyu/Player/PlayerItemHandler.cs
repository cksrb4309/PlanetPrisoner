using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerItemHandler : MonoBehaviour
{
    static PlayerItemHandler instance = null;
    public static PlayerItemHandler Instance
    {
        get { return instance; }
    }

    [SerializeField] Transform rightHand;

    [SerializeField] InputActionReference scrollWheelInputAction; // 스크롤 입력
    [SerializeField] InputActionReference itemUseInputAction; // 아이템 사용 입력
    [SerializeField] InputActionReference itemDropInputAction; // 아이템 드랍 입력

    [SerializeField] InputActionReference itemSelectHotkey1; // 아이템 선택 단축키 1
    [SerializeField] InputActionReference itemSelectHotkey2; // 아이템 선택 단축키 2
    [SerializeField] InputActionReference itemSelectHotkey3; // 아이템 선택 단축키 3
    [SerializeField] InputActionReference itemSelectHotkey4; // 아이템 선택 단축키 4
    [SerializeField] InputActionReference itemSelectHotkey5; // 아이템 선택 단축키 5

    PlayerInfo playerInfo;

    PlayerAnimator playerAnimator = null; // 플레이어 애니메이터

    Item selectedItem = null; // 선택 중인 아이템

    bool isItemUsing = false; // 아이템 사용 중 트리거 변수

    int currentSelectedIndex = 0; // 현재 선택 중인 아이템
    int beforeSelectedIndex = 0; // 이전에 선택하던 아이템
    int maxSelectedIndex = 5; // 선택할 수 있는 아이템 최대 인덱스

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        playerAnimator = GetComponent<PlayerAnimator>();

        InventoryUI.Instance.ItemSlotEquipSetting(0);

        playerInfo = new PlayerInfo(
            GetComponent<PlayerSpaceSuit>(),
            GetComponent<PlayerController>(),
            GetComponent<PlayerScanner>(),
            this,
            GetComponent<PlayerAttacker>());
    }
    private void OnEnable()
    {
        scrollWheelInputAction.action.Enable();
        itemUseInputAction.action.Enable();
        itemDropInputAction.action.Enable();

        itemSelectHotkey1.action.Enable();
        itemSelectHotkey2.action.Enable();
        itemSelectHotkey3.action.Enable();
        itemSelectHotkey4.action.Enable();
        itemSelectHotkey5.action.Enable();
    }
    private void OnDisable()
    {
        scrollWheelInputAction.action.Disable();
        itemUseInputAction.action.Disable();
        itemDropInputAction.action.Disable();

        itemSelectHotkey1.action.Disable();
        itemSelectHotkey2.action.Disable();
        itemSelectHotkey3.action.Disable();
        itemSelectHotkey4.action.Disable();
        itemSelectHotkey5.action.Disable();
    }
    public void Update()
    {
        #region 아이템 드랍 입력

        // 아이템 사용 중이라면 사용 중인 아이템을 버리지 못하도록 리턴
        if (isItemUsing) return;

        // 아이템 드랍 입력하고, 들고 있는 아이템이 있을 경우
        if (itemDropInputAction.action.WasPressedThisFrame() &&
            selectedItem != null)
        {
            // 인벤토리에서 제거
            PlayerInventory.Instance.RemoveItem(currentSelectedIndex);

            playerAnimator.SetItemChangeTrigger(AnimationParameter.NoItem);

            // 아이템 오브젝트 활성화 !
            selectedItem.Activate();

            // 아이템 연결 끊기
            selectedItem = null;
        }

        #endregion

        #region 아이템 사용 입력

        // 아이템 사용 중이라면 사용 입력이 반복되지 않도록 함수를 반환
        if (isItemUsing) return;

        // 아이템 사용 입력하고, 해당 아이템이 Default 타입이 아닐 경우
        if (itemUseInputAction.action.WasPressedThisFrame() &&
            selectedItem != null &&
            selectedItem.itemData.itemType != ItemType.Default)
        {
            Debug.Log("아이템 사용 시도 !");

            // 아이템 사용 중 트리거 변수 활성화
            isItemUsing = true;

            // 아이템에 따른 사용 애니메이션 트리거 재생
            playerAnimator.SetItemUseTrigger(selectedItem.itemData.useTrigger);
        }

        #endregion

        #region 아이템 변경 입력

        // 아이템을 사용 중이라면 함수를 반환하여
        // 아이템 변경을 막아놓는다
        if (isItemUsing) return;

        // 마우스 휠 스크롤 입력 받기
        Vector2 scrollDelta = scrollWheelInputAction.action.ReadValue<Vector2>();

        // 입력된 값이 있다면
        if (Mathf.Abs(scrollDelta.y) > float.Epsilon)
        {
            // 감소 값을 받았을 때
            if (scrollDelta.y < 0)
            {
                currentSelectedIndex = (currentSelectedIndex + 1) >= maxSelectedIndex ? 0 : currentSelectedIndex + 1;
            }
            // 증가 값을 받았을 때
            else
            {
                currentSelectedIndex = (currentSelectedIndex - 1) < 0 ? maxSelectedIndex - 1 : currentSelectedIndex - 1;
            }
        }

        // 눌른 단축키 Index 저장
        int pressedHotkeyIndex = -1;

        // 아이템 선택 단축키 입력을 확인한다
        if (itemSelectHotkey1.action.WasPressedThisFrame()) pressedHotkeyIndex = 0;
        if (itemSelectHotkey2.action.WasPressedThisFrame()) pressedHotkeyIndex = 1;
        if (itemSelectHotkey3.action.WasPressedThisFrame()) pressedHotkeyIndex = 2;
        if (itemSelectHotkey4.action.WasPressedThisFrame()) pressedHotkeyIndex = 3;
        if (itemSelectHotkey5.action.WasPressedThisFrame()) pressedHotkeyIndex = 4;

        // 누른 버튼이 있다면
        if (pressedHotkeyIndex != -1) currentSelectedIndex = pressedHotkeyIndex;

        // 이전 선택 중인 인덱스와 달라졌을 때
        if (beforeSelectedIndex != currentSelectedIndex)
        {
            InventoryUI.Instance.ItemSlotUnEquipSetting(beforeSelectedIndex);

            beforeSelectedIndex = currentSelectedIndex;

            InventoryUI.Instance.ItemSlotEquipSetting(currentSelectedIndex);

            // 아이템 장착
            EquipItem();
        }

        #endregion
    }
    //public Transform GetHandTransform() => rightHand;
    public Item GetCurrentItem() => selectedItem;
    public void EquipItem() // 아이템 장착
    {
        // 현재 Index의 아이템을 가져온다
        Item targetItem = PlayerInventory.Instance.GetItemFromInventory(currentSelectedIndex);

        // 같은 아이템 선택이 아닐 경우
        if (selectedItem == null ||
            targetItem == null ||
            selectedItem.GetInstanceID() != targetItem.GetInstanceID())
        {
            // 아이템에 따른 손 애니메이션 트리거 재생
            playerAnimator.SetItemChangeTrigger(targetItem == null ? AnimationParameter.NoItem : targetItem.itemData.equipTrigger);

            selectedItem?.DisableInHand();

            selectedItem = targetItem;

            selectedItem?.EnableInHand(rightHand);
        }
    }
    public void OnItemUseComplete() // 아이템 사용 완료 함수
    {
        Debug.Log("아이템 사용 완료 함수 !");

        // 사용 중이지 않은 상태로 전환
        isItemUsing = false;

        // 선택한 아이템의 사용 시 기능을 호출한다
        UnityEvent<PlayerInfo> itemAction = selectedItem.itemData.itemUseAction;

        // 만약 아이템이 사용 아이템일 경우
        if (selectedItem.itemData.itemType == ItemType.Consumable)
        {
            // 현재 인덱스의 아이템 제거
            PlayerInventory.Instance.RemoveItem(currentSelectedIndex);

            itemAction.Invoke(playerInfo);

            selectedItem.ConsumeItem();

            selectedItem = null;

            playerAnimator.SetItemChangeTrigger(AnimationParameter.NoItem);
        }
        else if (selectedItem.itemData.itemType == ItemType.NonConsumable)
        {
            itemAction.Invoke(playerInfo);
        }
    }
    public void OnItemAttack()
    {
        selectedItem.itemData.itemUseAction.Invoke(playerInfo);
    }
    public void OnAttackComplete()
    {
        // 사용 중이지 않은 상태로 전환
        isItemUsing = false;
    }
}
