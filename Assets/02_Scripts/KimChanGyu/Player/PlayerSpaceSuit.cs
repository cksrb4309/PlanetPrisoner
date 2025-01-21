using UnityEngine;

public class PlayerSpaceSuit : MonoBehaviour, IDamagable
{
    // 일반 우주복의 산소 소모량
    [SerializeField] float minOxygenDrain = 0.05f;
    [SerializeField] float maxOxygenDrain = 3f;

    // 강화 우주복의 산소 소모량
    [SerializeField] float minExSuitOxygenDrain = 0.025f;
    [SerializeField] float maxExSuitOxygenDrain = 2f;

    [SerializeField] float maxHp = 3f;

    PlayerOxygen playerOxygen = null;

    float currHp = 0;

    float Hp
    {
        get
        {
            return currHp;
        }
        set
        {
            currHp = value;

            if (currHp < 0) currHp = 0;

            PlayerStateUI.Instance.SetSuitSpaceFillImage(currHp > 0 ? currHp / maxHp : 0);
        }
    }
    private void Start()
    {
        currHp = maxHp;

        playerOxygen = GetComponent<PlayerOxygen>();

        playerOxygen.SetOxygenDecreaseValue(minOxygenDrain);
    }
    public void Hit(float damage)
    {
        if (currHp <= 0) return;

        // 체력 감소
        Hp -= damage;

        playerOxygen.SetOxygenDecreaseValue(
            Mathf.Lerp(
                minOxygenDrain,
                maxOxygenDrain,
                currHp / maxHp));
    }
    public void EquipEnhancedSuit()
    {
        minOxygenDrain = minExSuitOxygenDrain;
        maxOxygenDrain = maxExSuitOxygenDrain;

        playerOxygen.SetOxygenDecreaseValue(
            Mathf.Lerp(
                minOxygenDrain,
                maxOxygenDrain,
                currHp / maxHp));
    }
}
