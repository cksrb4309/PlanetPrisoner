using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSpawnManager : MonoBehaviour
{
    [SerializeField] Transform[] spawnPositions;
    [SerializeField] ItemSpawnData[] itemSpawnDatas;

    private void OnEnable()
    {
        NextDayController.Subscribe(SpawnItem, ActionType.NextDayTransition);
    }
    private void OnDisable()
    {
        NextDayController.Unsubscribe(SpawnItem, ActionType.NextDayTransition);
    }
    void SpawnItem()
    {
        foreach (var itemSpawnData in itemSpawnDatas) SpawnItem(itemSpawnData);
    }
    void SpawnItem(ItemSpawnData itemSpawnData)
    {
        int spawnCount = itemSpawnData.GetRandomCount(UnityEngine.Random.value);

        List<Transform> spawnTransformList = GetRandomTransform(spawnCount);
    }
    List<Transform> GetRandomTransform(int count)
    {
        HashSet<Transform> result = new HashSet<Transform>();

        int temp = 0;

        while (result.Count < count)
        {
            if (!result.Add(spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Length)])) temp++;

            if (temp > 5000f) return result.ToList();
        }

        return result.ToList();
    }
}

[Serializable]
public struct ItemSpawnData
{
    public Item itemPrefab;

    public int minCount;
    public int maxCount;

    public int GetRandomCount(float value) => (int)Mathf.Lerp(minCount, maxCount, value);
}