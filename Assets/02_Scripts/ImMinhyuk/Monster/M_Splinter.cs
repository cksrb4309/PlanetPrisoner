using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class M_Splinter : Monster
{
    MonsterHearing monsterhearing; // 청력 클래스

    protected override void Start()
    {
        base.Start();

        // 청력 세팅
        monsterhearing = gameObject.AddComponent<MonsterHearing>();
        monsterhearing.Initialize(this);

        // 타게팅(플레이어) 무한루프 코루틴
        StartCoroutine(CoFindTarget());
    }

    IEnumerator CoFindTarget()
    {
        while (true)
        {
            GameObject candidateTarget = monsterhearing.FindTarget();
            if(candidateTarget != null)
            {
                SetDestination(SetNearDestination_InNavMesh(candidateTarget.transform.position));
            }            
            yield return new WaitForSeconds(0.1f);  // N초마다 타겟을 탐색
        }
    }

    protected override M_Stat SetStat()
    {
        M_Stat _stat;

        if (!MonsterStat.Instance.StatDict.TryGetValue("Splinter", out _stat))
        {
            Debug.LogWarning($"Splinter not found in stat Dictionary");
        }

        return _stat;
    }
}
