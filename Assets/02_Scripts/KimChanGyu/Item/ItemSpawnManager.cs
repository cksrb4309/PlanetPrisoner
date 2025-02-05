using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

public class ItemSpawnManager : MonoBehaviour
{
    [SerializeField] Transform[] spawnPositions;
    [SerializeField] ItemSpawnData[] itemSpawnDatas;

    public bool isStartSpawn = false;

    private void OnEnable()
    {
        NextDayController.Subscribe(SpawnItem, ActionType.NextDayTransition);
        NextDayController.Subscribe(SpawnItem, ActionType.FirstGameTransition);
    }
    private void OnDisable()
    {
        NextDayController.Unsubscribe(SpawnItem, ActionType.NextDayTransition);
        NextDayController.Unsubscribe(SpawnItem, ActionType.FirstGameTransition);
    }
    private void Start()
    {
        if (isStartSpawn) SpawnItem();
    }
    void SpawnItem()
    {
        List<Transform> spawnPositionList = spawnPositions.ToList();

        foreach (var itemSpawnData in itemSpawnDatas)
        {
            int itemCount = itemSpawnData.GetRandomCount();

            for (; itemCount > 0; itemCount--)
            {
                int spawnPositionIndex = UnityEngine.Random.Range(0, spawnPositionList.Count);

                Item item = Instantiate(itemSpawnData.itemPrefab, spawnPositionList[spawnPositionIndex].position + Vector3.up * 2f, Quaternion.identity, spawnPositionList[spawnPositionIndex]);

                item.Activate();

                spawnPositionList.RemoveAt(spawnPositionIndex);
            }
        }
    }
}

[Serializable]
public struct ItemSpawnData
{
    public Item itemPrefab;

    public int minCount;
    public int maxCount;

    public int GetRandomCount() => (int)Mathf.Lerp(minCount, maxCount, UnityEngine.Random.value);
}