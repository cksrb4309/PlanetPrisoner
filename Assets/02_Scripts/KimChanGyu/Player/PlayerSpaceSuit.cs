using UnityEngine;

public class PlayerSpaceSuit : MonoBehaviour, IDamagable
{
    [SerializeField] float minOxygenDecreaseValue = 0.05f;
    [SerializeField] float maxOxygenDecreaseValue = 1f;

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

        playerOxygen.SetOxygenDecreaseValue(minOxygenDecreaseValue);
    }
    public void Hit(float damage)
    {
        if (currHp <= 0) return;

        // 체력 감소
        Hp -= damage;

        playerOxygen.SetOxygenDecreaseValue(
            Mathf.Lerp(
                maxOxygenDecreaseValue,
                minOxygenDecreaseValue,
                currHp / maxHp));
    }
}
