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
        
        // 시력 세팅
        monsterSight = gameObject.AddComponent<MonsterSight>();
        monsterSight.Initialize(this, headSight, target);

        // 타게팅(플레이어) 무한 루프 코루틴
        StartCoroutine(CoFindTarget()); 
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
