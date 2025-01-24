using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] Image iconImageUI; // 아이템 아이콘 이미지

    Image slotBackgroundUI; // 아이템 슬롯 배경 이미지

    float unEquipAlpha; // 비장착 알파 값
    float currAlpha; // 현재 Alpha 값
    float equipAlpha; // 장착 Alpha 값
    float alphaSpeed; // 장착 및 장착 해제로 인한 Alpha 증감 속도

    Coroutine equipCoroutine = null; // 장착 코루틴
    Coroutine unEquipCoroutine = null; // 장착 해제 코루틴

    private void Start()
    {
        slotBackgroundUI = GetComponent<Image>();

        unEquipAlpha = slotBackgroundUI.color.a;
        currAlpha = unEquipAlpha;
        equipAlpha = 0.8f;
        alphaSpeed = 3f;
    }
    public void SetItem(Item item)
    {
        iconImageUI.color = new Color(1, 1, 1, 1);

        iconImageUI.sprite = item.itemData.itemIconImage;
    }
    public void RemoveItem()
    {
        iconImageUI.color = new Color(1, 1, 1, 0);

        iconImageUI.sprite = null;
    }
    public void Equip() // 아이템 장착 시
    {
        // 장착 해제 중인 코루틴이 있다면 중지한다
        if (unEquipCoroutine != null)
        {
            StopCoroutine(unEquipCoroutine);
        }
        equipCoroutine = StartCoroutine(EquipCoroutine());
    }
    public void UnEquip() // 아이템 장착 해제 시
    {
        // 장착 중인 코루틴이 있다면 중지한다
        if (equipCoroutine != null)
        {
            StopCoroutine(equipCoroutine);
        }
        unEquipCoroutine = StartCoroutine(UnEquipCoroutine());
    }
    IEnumerator EquipCoroutine()
    {
        Color color = slotBackgroundUI.color;
        for (; currAlpha < 0.8f; currAlpha += Time.deltaTime * alphaSpeed)
        {
            color.a = currAlpha;
            slotBackgroundUI.color = color;

            yield return null;
        }
        currAlpha = equipAlpha;
        color.a = currAlpha;
        slotBackgroundUI.color = color;
    }
    IEnumerator UnEquipCoroutine()
    {
        Color color = slotBackgroundUI.color;

        for (; currAlpha > unEquipAlpha; currAlpha -= Time.deltaTime * alphaSpeed)
        {
            color.a = currAlpha;

            slotBackgroundUI.color = color;

            yield return null;
        }
        currAlpha = unEquipAlpha;

        color.a = currAlpha;

        slotBackgroundUI.color = color;
    }
}
