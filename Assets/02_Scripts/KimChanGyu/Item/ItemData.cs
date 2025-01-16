using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ItemData", menuName = "Item/ItemData")]
public class ItemData : ScriptableObject // Item 클래스가 갖고 있을 정보
{
    public string itemName; // 아이템 이름

    public ItemType itemType = ItemType.Default; // 아이템 타입

    public int itemSize = 1; // 인벤토리 차지 공간

    public int itemPrice; // 아이템 가격

    public float itemWeight; // 아이템 무게

    public Sprite itemIconImage; // 아이템 아이콘 이미지

    public string equipTriggerName = string.Empty; // 장착 애니메이션 트리거 이름
    public string useTriggerName = string.Empty; // 사용 애니메이션 트리거 이름

    public UnityEvent<bool> itemUseAction = null; // 아이템 사용 Action
}

public enum ItemType
{
    Default, // 기본 아이템
    Consumable, // 소모성 아이템
    NonConsumable, // 비소모성 아이템
}