using UnityEngine;

public class OxygenTank : MonoBehaviour, IInteractable
{
    [SerializeField] float oxygen = 200; // 200L
    [SerializeField] PlayerOxygen player;

    public string TooltipText => "산소충전 [E]";

    void Start()
    {
        
    }

    void Update()
    {
        
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
        // TODO : 플레이어의 산소 충전
    }
}
