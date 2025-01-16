using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class M_Splinter : Monster, IMonsterHearing
{
    private GameObject player; // 오디오소스를 감지하기 위한 오브젝트로, Monster 클래스의 target이랑은 엄연히 구분할 것
    private AudioSource playerAudioSource;

    private float moveChanceLow = 0.1f; // 작은 소리를 들을 때 트래킹 확률 (낮은 확률)
    private float moveChanceHigh = 0.5f; // 큰 소리일 들을 때 트래킹 확률 (높은 확률)

    // 스플린터의 기본 컨셉은 시야가 없어 Monster 크래스의 target을 지정할 수 없다.

    protected override void Start()
    {
        base.Start(); 
        SetPlayer();
        StartCoroutine(CoFindTarget());
    }

    /// <summary>
    /// Start()할 때 추적 대상(플레이어)를 세팅
    /// </summary>
    void SetPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 0)
        {
            player = players[0]; // 싱글플레이어 기준 기본 0번
            playerAudioSource = player.GetComponent<AudioSource>();
            if (playerAudioSource == null)
            {
                Debug.Log("스플린터 : 플레이어 태그를 가진 오브젝트에 오디오소스 컴포넌트가 없습니다.");
            }
        }
        else
        {
            Debug.Log("스플린터 : 플레이어 태그를 가진 오브젝트가 없습니다");
        }
    }

    IEnumerator CoFindTarget()
    {
        while (true)
        {
            destination = FindTargetInHearing();
            yield return new WaitForSeconds(0.1f);  // N초마다 타겟을 탐색
        }
    }

    public Vector3 FindTargetInHearing()
    {
        // 플레이어가 소리를 내지 않는다면 기존 이동 목표 지점 리턴
        if (playerAudioSource.isPlaying == false) return destination; 

        // 플레이어와 몬스터 간의 거리 계산
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // 감지 범위 내에서만 유효
        if (distanceToPlayer <= patrolRange)
        {
            // 거리와 비례한 소리 크기 추출 0 ~ 1
            float volumeLevel = Mathf.Clamp01(playerAudioSource.volume / distanceToPlayer);
            Debug.Log($"소리 레밸 {volumeLevel}");

            // 소리를 죽이면 아무 행동도 하지 않는다.
            if (volumeLevel < 0.01) return destination;

            // 가까운 거리에서 소리를 들으면 해당 '방향으로' 공격한다.
            if (distanceToPlayer < attackRange)
            {
                // 중복 상태 세팅 방지
                // TODO 프로퍼티 단에서 처리할까?
                if (State != EState.Attack)
                {
                    // 소리난 방향으로 회전
                    // 네브매쉬랑 같이 사용하고 있어서 부자연스러울지도?? 지금은 괜찮음
                    Vector3 direction = player.transform.position - transform.position;
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
                return player.transform.position;
            }
        }

        // 플레이어가 낸 소리가 재생 범위 밖이라면 기존 이동 목표 지점 리턴
        return destination;
    }
}
