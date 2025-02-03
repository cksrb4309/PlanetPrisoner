using UnityEngine;

public class ItemReward : MonoBehaviour
{
    [SerializeField] OxygenTank oxygenTank;
    [SerializeField] Transceiver transceiver;

    public void GiveReward()
    {
        foreach (var obj in transceiver.objectInTransceiver)
        {
            Item item = obj.GetComponent<Item>();

            if (item != null) oxygenTank.ChangeOxygen(item.itemData.itemPrice);
        }
    }
}
