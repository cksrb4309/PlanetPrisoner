using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Murloc : Monster, IMonsterSight
{
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
            target = ((IMonsterSight)this).FindTargetInSight(); // 시력 우선 탐색
            
            // 시력에서 못찾았으면 청력으로 넘긴다.
            if(target==null)
            {
                playingAudioSources = monsterhearing.IsPlayingList();
                Debug.Log($"{playingAudioSources.Count}개 재생중");

                closetAudioCandidate = FindClosetAudioSource();
                if (closetAudioCandidate != null)
                {
                    destination = FindTargetInHearing();
                }
            }

            yield return new WaitForSeconds(0.1f);  // N초마다 타겟을 탐색
        }
    }

    /// <summary>
    /// 1. OverlapSphere로 범위내의 모든 콜라이더를 탐지한다.
    /// 2. 각 콜라이더방향으로 Ray를 쏴서 Player인지 확인한다.
    /// </summary>
    GameObject IMonsterSight.FindTargetInSight()
    {
        Collider[] hitCollidersInMaxSight = Physics.OverlapSphere(transform.position, stat.maxSightRange); // 최대 범위 내의 hit되는 콜라이더를 모두 탐색한다.

        foreach (Collider hitCollider in hitCollidersInMaxSight)
        {
            // 장애물을 고려하지 않고 오버랩 스피어 안에 플레이어가 있음
            if (hitCollider.CompareTag("Player"))
            {
                Vector3 directionToTarget = hitCollider.transform.position - headSight.transform.position; // 몬스터와 플레이어의 방향 벡터
                float distanceToTarget = Vector3.Distance(headSight.transform.position, hitCollider.transform.position); // 거리 계산

                // 이미 타겟팅이 됐을 때는 최대 탐지 범위 내에서 찾아 준다.
                if (target != null && distanceToTarget < stat.maxSightRange)
                {
                    return hitCollider.gameObject;
                }

                // 최소 탐지 범위 내에서는 플레이어를 항상 찾는다.
                if (distanceToTarget < stat.minSightRange)
                {
                    return hitCollider.gameObject;
                }

                // 최대 탐지 거리 안이고 시야각 내에서 플레이어를 찾을 때
                // 벽 따위의 장애물을 고려해서 레이로 다시 체크해준다.
                // 레이 쏴서 플레이어 아니면 Continue
                if (Physics.Raycast(headSight.transform.position, directionToTarget.normalized, out RaycastHit hitInfo, distanceToTarget))
                {
                    if (hitInfo.collider.CompareTag("Player"))
                    {
                        float angleToPlayer = Vector3.Angle(transform.forward, directionToTarget); // 몬스터와 플레이어의 각도
                        if (angleToPlayer <= stat.sightAngle / 2) // 좌, 우 때문에 1/2씩 나눔
                        {
                            return hitCollider.gameObject;
                        }
                    }
                }
            }
        }

        // 여끼까지 왔으면 플레이어를 못 찾았으므로 타겟을 밀어준다.
        return null;
    }


    // 현재 재생되는 오디오 소스만 추출
    GameObject FindClosetAudioSource()
    {
        float candidate = 0; // (볼륨/거리)가 가장 큰 녀석
        GameObject ret = null;
        // playingAudioSources의 모든 오디오 소스에 대해 거리 계산
        for (int i = 0; i < playingAudioSources.Count; i++)
        {
            float distance = Vector3.Distance(playingAudioSources[i].transform.position, transform.position);
            if (distance <= stat.canHearingRange)
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
        if (MonsterStat.Instance.StatDict.TryGetValue("Murloc", out _stat))
        {
            Debug.Log(stat.hp);
        }
        else
        {
            Debug.LogWarning($"Murloc not found in stat Dictionary");
        }
        return _stat;
    }
}
