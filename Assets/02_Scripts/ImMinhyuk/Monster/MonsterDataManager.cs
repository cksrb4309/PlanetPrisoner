using System;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStat : MonoBehaviour
{
    public static MonsterStat Instance { get; private set; }

    [SerializeField]
    private TextAsset monsterData;
    public Dictionary<string, Stat> StatDict { get; private set; } = new Dictionary<string, Stat>();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            StatData statData = JsonUtility.FromJson<StatData>(monsterData.text);
            StatDict = statData.MakeDict();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

[Serializable]
public struct Stat
{
    public string name;
    public int hp;
    public float speed;
    public int attackPower;
    public int attackRange;
    public float sightAngle;
    public float maxSightRange;
    public float minSightRange;
    public int patrolRange;
}

[Serializable]
public class StatData
{
    public List<Stat> Monsters = new List<Stat>();

    public Dictionary<string, Stat> MakeDict()
    {
        Dictionary<string, Stat> dict = new Dictionary<string, Stat>();
        foreach (Stat monster in Monsters)
        {
            dict.Add(monster.name, monster);
        }
        return dict;
    }
}
