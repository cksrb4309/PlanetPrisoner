using Codice.Client.BaseCommands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class MonsterSpawnManager : MonoBehaviour
{
    static MonsterSpawnManager instance = null;
    public static MonsterSpawnManager Instance
    {
        get => instance;
    }

    [SerializeField] List<Transform> spawnPointList;

    [SerializeField] List<MonsterSpawnInfo> spawnInfoList;

    [SerializeField] GameObject monsterObjectRoot;


    // 주변에 플레이어가 있는 지 확인할 레이어마스크
    LayerMask playerLayerMask;
    [SerializeField] float playerDetectionRange = 10f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        playerLayerMask = LayerMask.GetMask("Player");
    }

    private void Start()
    {
        // 시작과 돵시에 적당히 몬스털르 풀어 놓는다. (일단 1세트씩)
        foreach(MonsterSpawnInfo monster in spawnInfoList)
        {
            Spawn(monster.spawnMonster);
        }

        StartCoroutine(CoCheckMonsterSpawn());
    }
    
    IEnumerator CoCheckMonsterSpawn()
    {
        while(true)
        {
            // 현재 인게임 시간 전달 (초 단위)
            float inGameTime = Time.time;
            OnCheckMonsterSpawn(inGameTime);
            yield return new WaitForSeconds(1f);
        }
    }

    public void OnCheckMonsterSpawn(float inGameTime)
    {
        foreach (MonsterSpawnInfo spawnInfo in spawnInfoList)
        {
            if (spawnInfo.IsSpawnable(inGameTime))
            {
                Spawn(spawnInfo.spawnMonster);
            }
        }
    }

    void Spawn(GameObject monster)
    {
        // 소환할 지역 랜덤으로 가져오기
        Transform spawnPoint = GetRandomSpawnPoint();

        // NavMesh 상으로 소환 가능한 지역으로 보정
        NavMeshHit hit;
        Vector3 spawnPosition = spawnPoint.position;
        if (NavMesh.SamplePosition(spawnPoint.position, out hit, 5, NavMesh.AllAreas))
        {
            spawnPosition = hit.position; 
        }

        // (소환 후보 지점 기준) Player를 찾아서 근처에 플레이어가 있다면 소환하지 않고 리턴
        Collider[] colliders = Physics.OverlapSphere(spawnPosition, playerDetectionRange, playerLayerMask);
        if (colliders.Length > 0)
        {
            return;
        }

        // 몬스터 생성 및 부모 설정
        GameObject spawnedMonster = Instantiate(monster, spawnPosition, Quaternion.identity);
        if (monsterObjectRoot != null)
        {
            spawnedMonster.transform.SetParent(monsterObjectRoot.transform);
        }
        else
        {
            Debug.Log("※ 부모 오브젝트를 설정해 주세요 ");
        }

        // 오디오소스 관리 대상에 추가한다.
        AudioSource audioSource = spawnedMonster.GetComponent<AudioSource>();

        // 오디오소스를 못찾았으면 하위 오브젝트에 AudioSource가 달려 있을 수도 있음
        if (audioSource == null)
        {
            audioSource = spawnedMonster.GetComponentInChildren<AudioSource>();
        }

        if( audioSource ==null)
        {
            // 여기까지 못찾았으면 Hearing클래스가 없는 몬스터
        }
        else
        {
            MonsterHearing.AddAudioSource(audioSource); // 오디오 관리대상에 추가
        }
    }

    Transform GetRandomSpawnPoint() => spawnPointList[Random.Range(0, spawnPointList.Count)];
}
