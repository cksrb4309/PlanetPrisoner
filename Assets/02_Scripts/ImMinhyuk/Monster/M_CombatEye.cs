using UnityEngine;

public class M_CombatEye : M_Eye
{
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
