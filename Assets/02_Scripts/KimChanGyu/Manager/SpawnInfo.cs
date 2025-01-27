using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnInfo", menuName = "Spawn/SpawnInfo")]
public class SpawnInfo : ScriptableObject
{
    [SerializeField] Monster
    private SpawnTimerData spawnTimerData;
}

[Serializable]
public struct SpawnTimerData
{
    public float time;

    public int count;
}
