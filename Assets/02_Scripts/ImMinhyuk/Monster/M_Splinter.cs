using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class M_Splinter : Monster
{
    // 스플린터의 기본 컨셉은 시야가 없어 Monster 클래스의 target을 지정할 수 없다.

    MonsterHearing monsterhearing;
    List<AudioSource> playingAudioSources = new List<AudioSource>();
    GameObject closetAudioCandidate;

    protected override void Start()
    {
        base.Start();
        monsterhearing = gameObject.AddComponent<MonsterHearing>();
        monsterhearing.Initialize(this);
        StartCoroutine(CoFindTarget());
    }

    IEnumerator CoFindTarget()
    {
        while (true)
        {
            GameObject candidateTarget = monsterhearing.FindTarget();
            if(candidateTarget != null)
            {
                destination = candidateTarget.transform.position;
            }
            
            yield return new WaitForSeconds(0.1f);  // N초마다 타겟을 탐색
        }
    }
    protected override M_Stat SetStat()
    {
        M_Stat _stat;

        if (MonsterStat.Instance.StatDict.TryGetValue("Splinter", out _stat))
        {
            Debug.LogWarning($"Splinter not found in stat Dictionary");
        }

        return _stat;
    }
}
