using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class M_Golem : Monster
{
    MonsterSight monsterSight;

    protected override void Start()
    {
        base.Start();
        monsterSight = gameObject.AddComponent<MonsterSight>();
        monsterSight.Initialize(this, headSight, target);
        StartCoroutine(CoFindTarget()); // 플레이어 탐지는 항상 한다(while(true))
    }

    IEnumerator CoFindTarget()
    {
        while (true)
        {
            target = monsterSight.FindTargetInSight();
            yield return new WaitForSeconds(0.1f);  // N초마다 타겟을 탐색
        }
    }

    protected override M_Stat SetStat()
    {
        M_Stat _stat;

        if (!MonsterStat.Instance.StatDict.TryGetValue("Golem", out _stat))
        {
            Debug.LogWarning($"Golem not found in stat Dictionary");
        }

        return _stat;
    }
}
