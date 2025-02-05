using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class M_Murloc : Monster
{
    [SerializeField] int id; // 멀록 그룹 내에서 사용할 ID

    MonsterSight monsterSight; // 시력 클래스
    MonsterHearing monsterhearing; // 청력 클래스

    [SerializeField] protected M_Murloc[] groupMurloc; // 동료 멀록

    // 죽었을 때 M_MurlocGroupHelper 알려줘서 해당 멀록 Null로 밀어주는 용도의 delegate
    public delegate void DeathEventHandler(int no);
    public event DeathEventHandler OnDeath; // 이벤트 작성 문법 [event] [delegate 타입] [이벤트 이름]
    // public event Action OnDeath; // 위 두줄 대신 이렇게도 사용 가능

    protected override void Start()
    {
        base.Start();

        // 시력 세팅
        monsterSight = gameObject.AddComponent<MonsterSight>();
        monsterSight.Initialize(this, headSight, target);

        // 청력 세팅
        monsterhearing = gameObject.AddComponent<MonsterHearing>();
        monsterhearing.Initialize(this);

        // 타게팅(플레이어) 무한루프 코루틴
        StartCoroutine(CoFindTarget());
    }

    protected override void UpdateIdle()
    {
        SetAnimDefault();
        // 타게팅이 되어있을 경우 추적한다. 
        if (target != null)
        {
            State = EState.Moving;
        }
        else
        {
            // N초에 한번 Idle할지 Move할지 선택
            if (nextDecisionTime >= decisionInterval)
            {
                if (false || Random.Range(0, 2) == 1 && isArrivedDestination == true)
                {
                    // Do NOT (IDLE 유지)
                }
                else
                {
                    SetDestination(GetRandomDestination_InNavMesh());

                    foreach (M_Murloc murloc in groupMurloc)
                    {
                        if (murloc != null)
                        {
                            // 멀록이 적당히 흩어질 수 있게 랜덤 오프셋 부여
                            Vector3 nearDestination = destination + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                            murloc.SetDestination(SetNearDestination_InNavMesh(nearDestination));
                        }
                    }
                    State = EState.Moving;
                }
                nextDecisionTime = 0f;
            }
        }
    }

    protected override void UpdateAttack()
    {
        SetAnimFaster();
    }

    protected override void UpdateDeath()
    {
        OnDeath?.Invoke(id); // 리스너 패턴으로 M_EyeGroupHelper에서 이 객체를 null로 밀어줘서 사망처리 해준다.
    }

    IEnumerator CoFindTarget()
    {
        while (true)
        {
            target = monsterSight.FindTargetInSight(); // 시력 우선 탐색

            // 시력에서 못찾았으면 청력으로 넘긴다.
            if (target == null)
            {
                GameObject candidateTarget = monsterhearing.FindTarget();
                if (candidateTarget != null)
                {
                    SetDestination(SetNearDestination_InNavMesh(candidateTarget.transform.position));
                }
            }

            // 타겟이 있으면 그룹 멀록과 공유하고 추적한다.
            if (target != null)
            {
                foreach (var murloc in groupMurloc)
                {
                    if (murloc != null)
                    {
                        murloc.SetDestination(target.transform.position);
                        murloc.SetStateChangeNow();
                    }
                }
            }

            yield return new WaitForSeconds(0.1f);  // N초마다 타겟을 탐색
        }
    }

    // 동료 그룹이 누구누구 있는지 설정해줌 (자신 포함)
    public void SetGroupMurloc(M_Murloc[] _groupMurloc, int _id)
    {
        groupMurloc = _groupMurloc;
        id = _id;
    }

    // 그룹원의 사망을 설정하는 함수
    public void SetMemberDie(int no)
    {
        groupMurloc[no] = null;
    }

    // Json에서 스텟을 가져온다.
    protected override M_Stat SetStat()
    {
        M_Stat _stat;

        if (!MonsterStat.Instance.StatDict.TryGetValue("Murloc", out _stat))
        {
            Debug.LogWarning($"Murloc not found in stat Dictionary");
        }

        return _stat;
    }
}
