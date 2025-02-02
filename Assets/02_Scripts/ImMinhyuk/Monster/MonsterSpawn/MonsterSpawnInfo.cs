using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnInfo", menuName = "Scriptable Objects/SpawnInfo")]
public class MonsterSpawnInfo : ScriptableObject
{
    // 스폰시킬 몬스터
    public GameObject spawnMonster;

    [SerializeField] private List<SpawnTimerData> spawnTimerDataList;

    // 현재 몬스터 개체 수
    // NonSerialized를 통해 ScriptableObject로 저장되는 것을 방지
    // Q. 몬스터가 죽었을 때, 감소시켜야 하는가?
    [NonSerialized] int currentMonsterCount = 0;

    public bool IsSpawnable(float currentTime)
    {
        // InGameTime 값은 1초 단위기 때문에 인스펙터에서 몬스터의 스폰 시간을 넣을 때는 편의상 1시간 단위로 넣기 위해
        // currentTime 값을 3600으로 나눈다
        currentTime /= 3600;

        // 모든 SpawnTimerData를 확인해서 
        foreach (SpawnTimerData spawnTimerData in spawnTimerDataList)
        {
            // 스폰 확률을 받아온다
            float spawnChance = spawnTimerData.GetSpawnChance(currentTime, currentMonsterCount);

            // 스폰 확률이 존재할 때
            if (spawnChance > 0f)
            {
                // 스폰을 성공했다면
                if (spawnChance > UnityEngine.Random.Range(0f, 100f))
                {
                    currentMonsterCount++; 
                    return true;
                }

                // 스폰을 실패했다면
                else
                {
                    return false;
                }
            }
        }

        return false;
    }
}

[Serializable]
public struct SpawnTimerData
{
    public float targetTime; // 기준 시간
    public float spawnRate;

    public int monsterSpawnLimit; // 기준 시간 이후로 스폰 가능한 개체 수

    // 현재 시간과 몬스터의 수를 통해 스폰이 가능한 시간이라면 스폰 확률을 반환하고,
    // 스폰이 가능하지 않다면 0을 반환한다
    public float GetSpawnChance(float currentTime, int currentMonsterCount)
    {
        // 현재 시간이 기준이 되는 targetTime 시간보다 이전일 경우에 0을 반환한다
        if (targetTime > currentTime) return 0f;

        // 현재 스폰된 몬스터의 수가 최대 스폰가능한 수보다 클 경우에 0을 반환한다
        if (monsterSpawnLimit <= currentMonsterCount) return 0f;

        // 모든 경우를 통과했기 때문에 스폰 확률을 반환한다
        return spawnRate;
    }
}
