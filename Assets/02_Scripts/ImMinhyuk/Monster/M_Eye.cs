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
        throw new System.NotImplementedException();
    }

    protected override void UpdateIdle()
    {
        throw new System.NotImplementedException();
    }

    protected override void UpdateMoving()
    {
        throw new System.NotImplementedException();
    }

    protected override void UpdateAttack()
    {
        throw new System.NotImplementedException();
    }
}
