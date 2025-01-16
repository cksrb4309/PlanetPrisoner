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
    private void Awake()
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
    public void SetItem(int index, Item item)
    {
        slots[index].SetItem(item);
    }
    public void RemoveItem(int index)
    {
        slots[index].RemoveItem();
    }
}
