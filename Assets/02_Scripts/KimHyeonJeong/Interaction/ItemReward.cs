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
            if (item != null)
            {
                string itemName = item.itemData.itemName;

                switch (itemName)
                {
                    case "목재":
                        oxygenTank.ChangeOxygen(10); // 목재 리워드 = 10L 
                        Debug.Log("목재 리워드 지급");
                        break;

                    case "고철":
                        oxygenTank.ChangeOxygen(25); // 고철 리워드 = 25L
                        Debug.Log("고철 리워드 지급");
                        break;

                    case "뭐할까요":
                        oxygenTank.ChangeOxygen(25); //  리워드 = L
                        Debug.Log(" 리워드 지급");
                        break;
                }
            }
        }
    }
}
