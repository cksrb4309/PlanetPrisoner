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

    LayerMask playerLayerMask;

    private void Awake()
    {
        instance = this;

        playerLayerMask = LayerMask.GetMask("Player");
    }
    public void OnSpawnCheck(float inGameTime)
    {
        Debug.Log("가져온 시간 : " + inGameTime.ToString());
    }
    Transform GetRandomSpawnPoint() => spawnPointList[Random.Range(0, spawnPointList.Count)];
}
