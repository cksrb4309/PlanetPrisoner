using UnityEngine;

public class M_EyeCombat : M_Eye
{
    // Json에서 스텟을 가져온다.
    protected override M_Stat SetStat()
    {
        M_Stat _stat;

        if (!MonsterStat.Instance.StatDict.TryGetValue("CombatEye", out _stat))
        {
            Debug.LogWarning($"CombatEye not found in stat Dictionary");
        }

        return _stat;
    }
}
