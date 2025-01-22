using UnityEngine;

public class M_PatrolEye : M_Eye
{
    protected override M_Stat SetStat()
    {
        M_Stat _stat;

        if (!MonsterStat.Instance.StatDict.TryGetValue("PatrolEye", out _stat))
        {
            Debug.LogWarning($"PatrolEye not found in stat Dictionary");
        }

        return _stat;
    }
}
