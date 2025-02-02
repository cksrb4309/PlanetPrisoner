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
        // 시작과 돵시에 적당히 몬스터를 풀어 놓는다. (일단 1세트씩)
        OnCheckMonsterSpawn((float)1e10);

        // TODO 임시로 여기서 호출하지만 라운드가 시작될 때마다 GameManager 등에서 호출해준다.
        MonsterHearing.InitializeAudioSources();

        //StartCoroutine(TestMonsterSpawn()); // 임민혁 테스트용
    }

    // 임민혁 테스트용, 실제 게임에선 사용되면 안되는 함수.
    IEnumerator TestMonsterSpawn()
    {
        while(true)
        {
            // 현재 인게임 시간 전달 (초 단위)
            float inGameTime = Time.time;
            OnCheckMonsterSpawn(inGameTime);
            // 인게임 타임의 10분주기를 모방한다고 친다.
            yield return new WaitForSeconds(10f); 
        }
    }

    // InGameTime에서 이 함수를 호출할 수 있도록 유도
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

        // (소환 후보 지점 기준) Player를 찾아서 근처에 플레이어가 있다면 다른 후보지역을 탐색한다.
        Collider[] colliders = Physics.OverlapSphere(spawnPosition, playerDetectionRange, playerLayerMask);
        if (colliders.Length > 0)
        {
            Spawn(monster);
            return;
        }

        // 몬스터 생성 및 부모 오브젝트 설정
        GameObject spawnedMonster = Instantiate(monster, spawnPosition, Quaternion.identity);
        if (monsterObjectRoot != null)
        {
            spawnedMonster.transform.SetParent(monsterObjectRoot.transform);
        }
        else
        {
            Debug.Log("※ 부모 오브젝트를 설정해 주세요 ");
        }
    }

    Transform GetRandomSpawnPoint() => spawnPointList[Random.Range(0, spawnPointList.Count)];
}
