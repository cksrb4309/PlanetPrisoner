using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    static SpawnManager instance = null;
    public static SpawnManager Instance
    {
        get => instance;
    }

    [SerializeField] List<Transform> spawnPointList;

    [SerializeField] List<SpawnInfo> spawnInfoList;

    // 주변에 플레이어가 있는 지 확인할 레이어마스크
    LayerMask playerLayerMask;

    private void Awake()
    {
        instance = this;

        playerLayerMask = LayerMask.GetMask("Player");
    }
    public void OnSpawnCheck(float inGameTime)
    {
        foreach (SpawnInfo spawnInfo in spawnInfoList)
        {
            if (spawnInfo.IsSpawnable(inGameTime))
            {
                Spawn(spawnInfo.spawnMonster);
            }
        }
    }
    void Spawn(Monster monster)
    {
        /// TODO : 실제 스폰 구현
        /// GetRandomSpawnPoint()로 랜덤한 스폰 포인트를 가져와서
        /// 해당 스폰 포인트와 플레이어가 너무 가까운지 확인한 후
        /// 가깝지 않다면 실제 몬스터를 생성해서 넣는 작업
    }
    Transform GetRandomSpawnPoint() => spawnPointList[Random.Range(0, spawnPointList.Count)];
}
