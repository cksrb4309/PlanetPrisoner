using System.Collections.Generic;
using UnityEngine;

public class BearTrapHelper : MonoBehaviour
{
    List<IDamagable> targets = new List<IDamagable>();

    bool isSet = false;
    public void DeactivateHelper()
    {
        isSet = false;

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
        foreach (IDamagable damagable in targets)
        {
            damagable.Hit(1f);
        }

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
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IDamagable damagable))
        {
            targets.Remove(damagable);
        }
    }
}
