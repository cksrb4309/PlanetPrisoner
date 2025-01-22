using System.Collections;
using UnityEngine;

public class M_Eye : Monster
{
    MonsterSight monsterSight;
    protected override void Start()
    {
        base.Start();
        monsterSight = gameObject.AddComponent<MonsterSight>();
        monsterSight.Initialize(this, headSight, target);
        StartCoroutine(CoFindTarget()); // 플레이어 탐지는 항상 한다(while(true))
    }

    IEnumerator CoFindTarget()
    {
        while (true)
        {
            target = monsterSight.FindTargetInSight();
            yield return new WaitForSeconds(0.1f);  // N초마다 타겟을 탐색
        }
    }

    protected override M_Stat SetStat()
    {
        // 하위 클래스에서 정의한다.
        // 통과 시켜주기 위해 null 반환.
        return null;
    }
}
