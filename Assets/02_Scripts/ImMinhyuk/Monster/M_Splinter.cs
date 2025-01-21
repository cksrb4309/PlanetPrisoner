using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class M_Splinter : Monster
{
    // 스플린터의 기본 컨셉은 시야가 없어 Monster 클래스의 target을 지정할 수 없다.

    private float moveChanceLow = 0.1f; // 작은 소리를 들을 때 트래킹 확률 (낮은 확률)
    private float moveChanceHigh = 0.5f; // 큰 소리일 들을 때 트래킹 확률 (높은 확률)

    MonsterHearing monsterhearing;
    List<AudioSource> playingAudioSources = new List<AudioSource>();
    GameObject closetAudioCandidate;

    protected override void Start()
    {
        base.Start();
        monsterhearing = new MonsterHearing(this);
        StartCoroutine(CoFindTarget());
    }

    IEnumerator CoFindTarget()
    {
        while (true)
        {
            playingAudioSources = monsterhearing.IsPlayingList();
            Debug.Log($"{playingAudioSources.Count}개 재생중");

            closetAudioCandidate = FindClosetAudioSource();
            if (closetAudioCandidate != null)
            {
                destination = FindTargetInHearing();
            }

            yield return new WaitForSeconds(0.1f);  // N초마다 타겟을 탐색
        }
    }

    // 현재 재생되는 오디오 소스만 추출
    GameObject FindClosetAudioSource()
    {
        float candidate=0; // (볼륨/거리)가 가장 큰 녀석
        GameObject ret = null;
        // playingAudioSources의 모든 오디오 소스에 대해 거리 계산
        for (int i = 0; i < playingAudioSources.Count; i++)
        {
            float distance = Vector3.Distance(playingAudioSources[i].transform.position, transform.position);
            if(distance <= stat.canHearingRange)
            {
                if (candidate < (playingAudioSources[i].volume / distance))
                {
                    ret = playingAudioSources[i].gameObject;
                }
            }
        }
        return ret;
    }

    Vector3 FindTargetInHearing()
    {
        // 죽었다면 아무것도 하지 않는다.
        if (State == EState.Death) return destination;

        // 플레이어와 몬스터 간의 거리 계산
        float distanceToPlayer = Vector3.Distance(transform.position, closetAudioCandidate.transform.position);

        // 감청 범위 내에서만 유효
        if (distanceToPlayer <= stat.canHearingRange)
        {
            // 거리와 비례한 소리 크기 추출 0 ~ 1
            float volumeLevel = Mathf.Clamp01(closetAudioCandidate.GetComponent<AudioSource>().volume / distanceToPlayer);
            Debug.Log($"소리 레밸 {volumeLevel}");

            // 소리를 죽이면 아무 행동도 하지 않는다.
            // TODO? 삭제?
            if (volumeLevel < 0.01) return destination;

            // 가까운 거리에서 소리를 들으면 해당 '방향으로' 공격한다.
            if (distanceToPlayer < stat.attackRange)
            {
                // 중복 상태 세팅 방지
                if (State != EState.Attack)
                {
                    // 소리난 방향으로 회전
                    // 네브매쉬랑 같이 사용하고 있어서 부자연스러울지도?? 지금은 괜찮음
                    Vector3 direction = closetAudioCandidate.transform.position - transform.position;
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5);

                    // 공격
                    State = EState.Attack;
                }
            }

            // 큰 소리 : 높은 확률로 이동, 작은 소리 : 작은 확률로 이동
            float moveChance = (volumeLevel > 0.1f) ? moveChanceHigh : moveChanceLow;
            if (Random.value < moveChance)
            {
                return closetAudioCandidate.transform.position;
            }
        }

        // 플레이어가 낸 소리가 재생 범위 밖이라면 기존 이동 목표 지점 리턴
        return destination;
    }

    protected override M_Stat SetStat()
    {
        M_Stat _stat;
        if (MonsterStat.Instance.StatDict.TryGetValue("Splinter", out _stat))
        {
            Debug.Log(stat.hp);
        }
        else
        {
            Debug.LogWarning($"Splinter not found in stat Dictionary");
        }
        return _stat;
    }
}
