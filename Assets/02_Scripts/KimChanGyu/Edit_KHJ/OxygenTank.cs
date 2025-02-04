using TMPro;
using UnityEngine;

public class OxygenTank : MonoBehaviour, IInteractable
{
    [SerializeField] float oxygen = 200f; // 2L = 2000ml

    [SerializeField] PlayerOxygen playerOxygen;

    public string TooltipText => $"���� ��� : { oxygen.ToString("F0") }\n������� [E]";

    public bool PurchaseItem(float price)
    {
        if (oxygen < price) return false;

        oxygen -= price;

        return true;
    }
    public float GetOxygen() // oxygen���� getter
    {
        return oxygen;
    }
    public void ChangeOxygen(float amount)
    {
        oxygen += amount;
    }
    public void Interact()
    {
        // ��Ұ� ���ٸ� ��ȯ
        if (oxygen <= float.Epsilon)
        {
            NotificationTextUI.Instance.NotificationText("��Ұ� �����մϴ�");

            return;
        }

        // ��� �ʿ� �䱸�� ��������
        float needFillAmount = playerOxygen.NeedFillOxygen();

        // �ʿ� ��ҷ��� ���� ��Һ��� ���ٸ� �ʿ� ��ҷ��� ���� ��ҷ� ���� �����
        if (oxygen < needFillAmount) needFillAmount = oxygen;

        // �ʿ� ��ҷ���ŭ �÷��̾��� ��ҿ� �߰��Ѵ�
        playerOxygen.RefillOxygen(needFillAmount);

        // ä���� ��Ҹ�ŭ ���� �����Ѵ�
        oxygen -= needFillAmount;
    }
}
