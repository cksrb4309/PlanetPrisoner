using UnityEngine;

public class MurlocSpear : MonoBehaviour
{
    // 공격력은 멀록에게 종속된다.
    float attackPower;

    void Start()
    {
        Destroy(gameObject, 5f);
    }

    public void SetAttackPower(float _attackPower)
    {
        attackPower = _attackPower;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<IDamagable>().Damaged(attackPower);
            Destroy(gameObject);
        }
    }
}
