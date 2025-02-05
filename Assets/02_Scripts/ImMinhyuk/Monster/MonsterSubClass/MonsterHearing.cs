using System.Collections.Generic;
using UnityEngine;
using static Monster;

public class MonsterHearing : MonoBehaviour
{
    // 이 스크립트를 갖고 있는 몬스터는 누구인가?
    Monster monster;

    // 필드에 있는 모든 오디오 소스들
    public static List<AudioSource> audioSources = new List<AudioSource>();
    // 몬스터가 지금 위치에서 들을 수 있는 오디오 소스들
    List<AudioSource> playingAudioSources;
    // (들을 수 있다면) 가장 가까운 오디오 소스를 가지고 있는 오브젝트
    GameObject closetAudioCandidate;


    ////////////////////////////////
    ///디버그용 
    [SerializeField] private List<AudioSource> debugAudioSources; // Inspector에서 확인용
    private void Update()
    {
        debugAudioSources = new List<AudioSource>(audioSources); // 매 프레임 복사
    }
    /// 디버그용

    private float moveChanceLow = 0.2f; // 작은 소리를 들을 때 트래킹 확률 (낮은 확률)
    private float moveChanceHigh = 0.75f; // 큰 소리일 들을 때 트래킹 확률 (높은 확률)

    public void Initialize(Monster _monster)
    {
        monster = _monster;       
    }

    public GameObject FindTarget()
    {
        GameObject ret = null;

        playingAudioSources = IsPlayingList();
        closetAudioCandidate = FindClosetAudioSource();

        if (closetAudioCandidate != null)
        {
            ret = CanITargeting();
        }

        return ret;
    }

    // 몬스터가 지금 위치에서 들을 수 있는 오디오 소스 중 가장 가까운 오디오 소스의 오브젝트 반환
    GameObject FindClosetAudioSource()
    {
        float candidate = 0; // (볼륨/거리)가 가장 큰 녀석
        GameObject ret = null;
        // playingAudioSources의 모든 오디오 소스에 대해 거리 계산
        for (int i = 0; i < playingAudioSources.Count; i++)
        {
            if (playingAudioSources[i].gameObject.tag == "Monster") continue; // 몬스터 테그를 제외하지 않으면 자기 자신도 포함해서 일단 패스

            float distance = Vector3.Distance(playingAudioSources[i].transform.position, transform.position);
            if (distance <= monster.Stat.canHearingRange)
            {
                if (candidate < (playingAudioSources[i].volume / distance))
                {
                    ret = playingAudioSources[i].gameObject;
                }
            }
        }
        return ret;
    }

    // (플레이어를 타게팅 할 수 있다면) 타게팅
    GameObject CanITargeting()
    {
        // 죽었다면 아무것도 하지 않는다.
        if (monster.State == EState.Death) return null;

        // 몬스터와 소리나는 대상의 거리 계산
        float distanceToPlayer = Vector3.Distance(transform.position, closetAudioCandidate.transform.position);

        // 감청 범위 내에서만 유효
        if (distanceToPlayer <= monster.Stat.canHearingRange)
        {
            // 거리와 비례한 소리 크기 추출 0 ~ 1
            float volumeLevel = Mathf.Clamp01(closetAudioCandidate.GetComponent<AudioSource>().volume / distanceToPlayer);
            //Debug.Log($"소리 레밸 {volumeLevel}");

            // 소리를 죽이면 아무 행동도 하지 않는다.
            // TODO? 삭제?
            if (volumeLevel < 0.01) return null;

            // 가까운 거리에서 소리를 들으면 해당 '방향으로' 공격한다.
            if (distanceToPlayer < monster.Stat.attackRange)
            {
                // 로직상 Moving 중일 때만 공격 가능
                if (monster.State == EState.Moving)
                {
                    // 소리난 방향으로 회전
                    // 네브매쉬랑 같이 사용하고 있어서 부자연스러울지도?? 지금은 괜찮음
                    Vector3 direction = closetAudioCandidate.transform.position - transform.position;
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 90);

                    // 공격
                    monster.State = EState.Attack;
                }
            }

            // 큰 소리 : 높은 확률로 이동, 작은 소리 : 작은 확률로 이동
            float moveChance = (volumeLevel > 0.1f) ? moveChanceHigh : moveChanceLow;
            if (Random.value < moveChance)
            {
                return closetAudioCandidate;
            }
        }

        // 플레이어가 낸 소리가 재생 범위 밖이라면 널 리턴
        return null;
    }

    // AudioSource를 관리하는 함수
    // TODO : GameManager 등에서 라운드를 시작할 때 마다 호출
    static public void InitializeAudioSources()
    {
        // 기존의 리스트를 밀어준다.
        RemoveAllAudioSource();

        // 모든 AudioSource 긁어오기
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (var audioSource in allAudioSources)
        {
            audioSources.Add(audioSource);
        }
    }

    // 필드에서 재생중인 모든 AudioSource 반환  
    public List<AudioSource> IsPlayingList()
    {
        List<AudioSource> playingSources = new List<AudioSource>();
        foreach (var source in audioSources)
        {
            if (source.isPlaying) // 재생 중인지 확인
            {
                playingSources.Add(source);
            }
        }
        return playingSources;
    }

    // AudioSource 객체를 갖고 있는 오브젝트가 생기면 이 함수를 호출해줘야 한다.
    public static void AddAudioSource(AudioSource audioSource)
    {
        if (!audioSources.Contains(audioSource))
        {
            audioSources.Add(audioSource);
        }
    }

    // AudioSource 객체를 갖고 있는 오브젝트가 제거되면 이 함수를 호출해줘야 한다.
    public static void RemoveAudioSource(AudioSource audioSource)
    {
        if (audioSources.Contains(audioSource))
        {
            audioSources.Remove(audioSource);
        }
    }

    // 모든 AudioSource를 밀어줌
    public static void RemoveAllAudioSource()
    {
        audioSources.Clear();
    }
}
