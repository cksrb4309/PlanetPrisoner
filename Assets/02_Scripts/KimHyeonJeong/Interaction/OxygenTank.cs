using TMPro;
using UnityEngine;

public class OxygenTank : MonoBehaviour, IInteractable
{
    [SerializeField] float oxygen = 200f; // 2L = 2000ml

    [SerializeField] PlayerOxygen playerOxygen;

    [SerializeField] AudioClip audioClip;

    AudioSource audioSource;
    public string TooltipText => $"남은 산소 : { oxygen.ToString("F0") }\n산소충전 [E]";

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public bool PurchaseItem(float price)
    {
        if (oxygen < price) return false;

        oxygen -= price;

        return true;
    }
    public float GetOxygen() // oxygen변수 getter
    {
        return oxygen;
    }
    public void ChangeOxygen(float amount)
    {
        oxygen += amount;
    }
    public void Interact()
    {
        // 산소가 없다면 반환
        if (oxygen <= float.Epsilon)
        {
            NotificationTextUI.Instance.NotificationText("산소가 부족합니다");

            return;
        }

        audioSource.PlayOneShot(audioClip);

        // 산소 필요 요구량 가져오기
        float needFillAmount = playerOxygen.NeedFillOxygen();

        // 필요 산소량이 현재 산소보다 많다면 필요 산소량을 현재 산소로 값을 맞춘다
        if (oxygen < needFillAmount) needFillAmount = oxygen;

        // 필요 산소량만큼 플레이어의 산소에 추가한다
        playerOxygen.RefillOxygen(needFillAmount);

        // 채워준 산소만큼 값을 제거한다
        oxygen -= needFillAmount;
    }
}
