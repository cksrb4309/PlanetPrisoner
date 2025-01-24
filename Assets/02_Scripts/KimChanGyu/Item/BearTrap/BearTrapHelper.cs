using System.Collections.Generic;
using UnityEngine;

public class BearTrapHelper : MonoBehaviour
{
    List<IDamagable> targets = new List<IDamagable>();

    bool isSet = false;
    bool isAttacked = false;
    public void InitHelper()
    {
        isSet = false;
        isAttacked = false;

        targets.Clear();
    }
    public void ActivateHelper()
    {
        if (targets.Count > 0)
        {
            Attack();
        }
        else
        {
            isSet = true;
        }
    }
    void Attack()
    {
        if (isAttacked) return;

        foreach (IDamagable damagable in targets)
        {
            damagable.Damaged(1f);
        }

        isAttacked = true;

        GetComponentInParent<BearTrap>().CompleteAttack();

        targets.Clear();
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamagable damagable))
        {
            targets.Add(damagable);

            if (isSet)
            {
                Attack();
            }

            Debug.Log("ADD Count : " + targets.Count);
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IDamagable damagable))
        {
            targets.Remove(damagable);

            Debug.Log("REMOVE Count : " + targets.Count);
        }
    }
}
