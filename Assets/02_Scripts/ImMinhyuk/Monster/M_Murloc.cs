using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Murloc : Monster
{
    private float moveChanceLow = 0.1f; // 작은 소리를 들을 때 트래킹 확률 (낮은 확률)
    private float moveChanceHigh = 0.5f; // 큰 소리일 들을 때 트래킹 확률 (높은 확률)

    MonsterSight monsterSight;
    MonsterHearing monsterhearing;
    List<AudioSource> playingAudioSources = new List<AudioSource>();
    GameObject closetAudioCandidate;

    protected override void Start()
    {
        base.Start();
        monsterSight = gameObject.AddComponent<MonsterSight>();
        monsterSight.Initialize(this, headSight, target);
        monsterhearing = gameObject.AddComponent<MonsterHearing>();
        monsterhearing.Initialize(this);
        StartCoroutine(CoFindTarget());
    }

    IEnumerator CoFindTarget()
    {
        while (true)
        {
            target = monsterSight.FindTargetInSight(); // 시력 우선 탐색
            
            // 시력에서 못찾았으면 청력으로 넘긴다.
            if(target==null)
            {
                GameObject candidateTarget = monsterhearing.FindTarget();
                if (candidateTarget != null)
                {
                    destination = candidateTarget.transform.position;
                }
            }

            yield return new WaitForSeconds(0.1f);  // N초마다 타겟을 탐색
        }
    }

    protected override M_Stat SetStat()
    {
        M_Stat _stat;

        if (!MonsterStat.Instance.StatDict.TryGetValue("Murloc", out _stat))
        {
            Debug.LogWarning($"Murloc not found in stat Dictionary");
        }

        return _stat;
    }
}
