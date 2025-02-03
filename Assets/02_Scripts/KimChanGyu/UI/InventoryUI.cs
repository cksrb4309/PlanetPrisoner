using System.Collections;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    static InventoryUI instance = null;
    public static InventoryUI Instance
    {
        get
        {
            return instance;
        }
    }

    [SerializeField] ItemSlot[] slots;
    [SerializeField] CanvasGroup itemSlotCanvasGroup;

    Coroutine fadeInOutCoroutine = null;

    private void Awake()
    {
        instance = this;

        itemSlotCanvasGroup.alpha = 1f;
    }
    public void SetItem(int index, Item item)
    {
        slots[index].SetItem(item);
    }
    public void RemoveItem(int index)
    {
        slots[index].RemoveItem();
    }
    public void ItemSlotEquipSetting(int index)
    {
        slots[index].Equip();

        if (fadeInOutCoroutine != null)
            StopCoroutine(fadeInOutCoroutine);

        fadeInOutCoroutine = StartCoroutine(FadeInOutCoroutine());
    }
    public void ItemSlotUnEquipSetting(int index)
    {
        slots[index].UnEquip();

        if (fadeInOutCoroutine != null)
            StopCoroutine(fadeInOutCoroutine);

        fadeInOutCoroutine = StartCoroutine(FadeInOutCoroutine());
    }
    IEnumerator FadeInOutCoroutine()
    {
        float alpha = itemSlotCanvasGroup.alpha;

        while (alpha < 1f)
        {
            alpha += Time.deltaTime * 3f;

            itemSlotCanvasGroup.alpha = alpha;

            yield return null;
        }
        alpha = 1f;

        itemSlotCanvasGroup.alpha = alpha;

        yield return new WaitForSeconds(1.5f);

        while (alpha > 0.3f)
        {
            alpha -= Time.deltaTime * 3f;

            itemSlotCanvasGroup.alpha = alpha;

            yield return null;
        }

        itemSlotCanvasGroup.alpha = 0.3f;
    }
}
