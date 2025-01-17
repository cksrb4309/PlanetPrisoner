using UnityEngine;

public class M_Murloc : Monster
{
    protected override void UpdateDeath()
    {
    }

    protected override void UpdateIdle()
    {
    }

    protected override void UpdateMoving()
    {
    }

    protected override void UpdateAttack()
    {
    }

    protected override Stat SetStat()
    {
        Stat _stat;
        if (MonsterStat.Instance.StatDict.TryGetValue("MUrloc", out _stat))
        {
            Debug.Log(stat.hp);
        }
        else
        {
            Debug.LogWarning($"Golem not found in stat Dictionary");
        }
        return _stat;
    }
}
