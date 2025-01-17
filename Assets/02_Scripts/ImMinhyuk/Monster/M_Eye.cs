using UnityEngine;

public class M_Eye : Monster
{
    M_CombatEye combatEye;
    M_PatrolEye patrolEye;
    protected override void Start()
    {
        combatEye = new M_CombatEye();
    }

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
        return stat;
    }
}
