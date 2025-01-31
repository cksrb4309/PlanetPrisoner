using System;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStat : MonoBehaviour
{
    public static MonsterStat Instance { get; private set; }

    [SerializeField]
    private TextAsset monsterData;
    public Dictionary<string, M_Stat> StatDict { get; private set; } = new Dictionary<string, M_Stat>();
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
public struct M_Stat
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
    public int canHearingRange;
}

[Serializable]
public class StatData
{
    public List<M_Stat> Monsters = new List<M_Stat>();

    public Dictionary<string, M_Stat> MakeDict()
    {
        Dictionary<string, M_Stat> dict = new Dictionary<string, M_Stat>();
        foreach (M_Stat monster in Monsters)
        {
            dict.Add(monster.name, monster);
        }
        return dict;
    }
}
