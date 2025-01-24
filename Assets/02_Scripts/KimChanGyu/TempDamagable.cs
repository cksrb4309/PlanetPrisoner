using UnityEngine;
using UnityEngine.UI;

public class TempDamagable : MonoBehaviour, IDamagable
{
    public float maxHp;
    public Image hpImage;

    float currentHp = 0;
    float Hp
    {
        get
        {
            return currentHp;
        }
        set
        {
            currentHp = value;

            hpImage.fillAmount = currentHp > 0f ? currentHp / maxHp : 0f;
        }
    }
    private void Start()
    {
        Hp = maxHp;
    }
    public void Damaged(float damage)
    {
        Hp -= damage;
    }
}
