using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    static PlayerInventory instance = null;
    public static PlayerInventory Instance
    {
        get 
        {
            return instance;
        }
    }

    public NotificationTextUI notificationTextUI;

    // 가지고 있을 아이템
    Item[] items = new Item[5] {
        null, null, null, null, null
    }; 

    // 아이템 슬롯끼리의 연결 리스트
    List<int>[] linkedItemSlots = new List<int>[5]{ 
        new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>()
    }; 

    int itemCount = 0; // 현재 가지고 있는 아이템의 수
    int itemMaxCount = 5; // 최대 가질 수 있는 아이템의 수

    public void AddItemToInventory(Item item) // 아이템 추가 함수
    {
        // 만약 인벤토리에 공간이 부족할 때
        if (itemCount > itemMaxCount - item.itemData.itemSize)
        {
            // UIManager에 아이템을 못먹는다는 신호를 보냄
            notificationTextUI.NotificationText("인벤토리에 자리가 없습니다");

            return; // 반환
        }

        // 아이템을 추가할 수 있는 지 확인한다
        int index = -1;

        // 아이템을 추가할 수 있을 때까지 반복
        int tmp = 0; // 무한 반복 방지
        while (tmp++ < 500) 
        {
            // 아이템을 추가 못한다면 -1, 할 수 있다면 해당 인덱스
            index = CanAddItem(item.itemData.itemSize);

            // 아이템을 추가할 수 있다면 while 반복 중지
            if (index != -1) break;

            // 추가할 수 없다면 뒤로 옮길 수 있는 것 중에 제일 앞에 있는 아이템을 한칸 이동
            for (int i = 0; i < itemMaxCount - 1; i++)
            {
                // 현재 보고 있는 items[i]가 아이템이 존재하고,
                // 다음 아이템 칸이 비어있을 때
                if (items[i] != null && items[i + 1] == null) 
                {
                    // 아이템 위치 이동
                    items[i + 1] = items[i];
                    items[i] = null;

                    // 아이템 슬롯 참조 이동
                    linkedItemSlots[i + 1] = linkedItemSlots[i];
                    linkedItemSlots[i] = new List<int>();

                    break; // 한칸 이동 시켰다면 for 반복문 중단 !
                }
            }
        }

        #region index 값 오류 체크용도
        if (index == -1)
        {
            Debug.LogWarning("Index가 -1? 너 인벤토리 확인해.");
            return;
        }
        #endregion

        // 아이템 슬롯 배치를 한다
        for (int i = index; i < index + item.itemData.itemSize; i++)
        {
            // 아이템 배열에 넣는다
            items[i] = item;

            // 아이템 슬롯 연결 리스트 셋팅
            for (int j = index; j < index + item.itemData.itemSize; j++)
            {
                // 현재 배치하는 아이템과 다르다면
                if (i != j)
                {
                    // 아이템 슬롯 연결 리스트에 추가한다
                    linkedItemSlots[i].Add(j);
                }
            }
        }

        // 갖고 있는 아이템의 개수 아이템의 크기만큼 늘린다
        itemCount += item.itemData.itemSize;

        // 아이템 슬롯을 업데이트 한다
        UpdateItemSlots();

        // 마지막으로 아이템을 감춘다
        item.DisableInHand();
    }
    public Item GetItemFromInventory(int index) // 아이템 전달 함수
    {
        return items[index];
    }
    public void RemoveItem(int index)
    {
        items[index] = null; // 현재 인덱스 null 전환
        InventoryUI.Instance.RemoveItem(index); // 아이템 제거

        itemCount--;

        // 해당 인덱스와 연결된 슬롯만큼 반복
        for (int i = 0; i < linkedItemSlots[index].Count; i++)
        {
            items[linkedItemSlots[index][i]] = null; // 연결된 슬롯의 아이템 제거

            linkedItemSlots[linkedItemSlots[index][i]].Clear();

            InventoryUI.Instance.RemoveItem(linkedItemSlots[index][i]); // 슬롯 비우기

            itemCount--;
        }

        linkedItemSlots[index].Clear();
    }
    int CanAddItem(int itemSize)
    {
        // 아이템이 최소 들어갈 수 있는 위치만큼 반복
        for (int index = 0; index <= itemMaxCount - itemSize; index++)
        {
            for (int j = 0; j < itemSize; j++)
            {
                // 만약 아이템이 들어있다면
                if (items[index + j] != null)
                {
                    // 현재 반복문을 스킵한다
                    break;
                }
                // 만약 아이템이 들어있지 않고 마지막 반복문이라면
                else if (j == itemSize - 1)
                {
                    return index; // 들어갈 수 있는 공간의 Index를 반환한다
                }
            }
        }

        // 만약 아이템을 넣을 수 있는 공간이 없다면 -1을 반환한다
        return -1;
    }
    void UpdateItemSlots()
    {
        for (int i = 0; i < itemMaxCount; i++)
        {
            if (items[i] != null)
            {
                InventoryUI.Instance.SetItem(i, items[i]);
            }
            else
            {
                InventoryUI.Instance.RemoveItem(i);
            }
        }
    }
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void OnDisable()
    {
        instance = null;

        Destroy(gameObject);
    }
}
