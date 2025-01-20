using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public abstract class Monster : MonoBehaviour, IMonsterDamagable
{
    protected GameObject target; // 플레이어
    protected Vector3 destination; // 이동할 목표 지점 
    protected bool isArrivedDestination = false; // 목표지점에 도착했는지 토글용

    protected const float decisionInterval = 5.0f; // 움직임 여부를 결정하는 시간 간격 (초)
    protected float nextDecisionTime = 0f;

    [SerializeField] protected CapsuleCollider[] hitRangeColliders; // 공격 Hit 판정용
    [SerializeField] protected GameObject headSight; //헤드 To 플레이어 시력 탐지 레이용

    // 하위 컴포넌트
    protected Animator animator;
    protected NavMeshAgent agent;


    [SerializeField] // 스텟 확인용으로 달아줬음
    protected M_Stat stat; // json으로부터 데이터를 받아온다.

    #region FSM
    public enum EState
    {
        Idle,
        Moving,
        Attack,
        Stun,
        Death
    }

    protected EState _state = EState.Idle;
    public virtual EState State
    {
        get { return _state; }
        set
        {
            // 동일한 상태값이 들어왔으면 아무것도 하지 않는다.
            if (_state == value) return;

            _state = value;

            switch (_state)
            {
                case EState.Idle:
                    agent.isStopped = true;
                    animator.CrossFade("Idle", 0.1f);
                    break;
                case EState.Moving:
                    agent.isStopped = false;
                    animator.CrossFade("Walk", 0.1f);
                    break;
                case EState.Attack:
                    agent.isStopped = true;
                    animator.CrossFade("Attack02", 0.1f, -1, 0);
                    break;
                case EState.Stun:
                    agent.isStopped = true;
                    animator.CrossFade("Stun", 0.1f);
                    break;
                case EState.Death:
                    agent.isStopped = true;
                    animator.CrossFade("Death", 0.1f);
                    break;
            }
        }
    }
    #endregion FSM

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        stat = SetStat();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = stat.speed;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        nextDecisionTime += Time.deltaTime;

        switch (State)
        {
            case EState.Idle:
                UpdateIdle();
                break;
            case EState.Moving:
                UpdateMoving();
                break;
            case EState.Attack:
                UpdateAttack();
                break;
            case EState.Stun:
                UpdateStun();
                break;
            case EState.Death:
                UpdateDeath();
                break;
        }
    }

    // FSM은 Idle로 부터 시작
    protected virtual void UpdateIdle()
    {
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
                    destination = GetRandomDestination();
                    State = EState.Moving;
                }
                nextDecisionTime = 0f;
            }
        }
    }

    protected virtual void UpdateMoving()
    {
        // 타겟팅이 되어 있을 경우 
        if (target != null)
        {
            // 공격 범위 안으로 왔음 => 공격
            Collider[] hitCollidersInMaxSight = Physics.OverlapSphere(transform.position, stat.attackRange); // 공격 범위 내의 hit되는 콜라이더를 모두 탐색한다.

            foreach (Collider hitCollider in hitCollidersInMaxSight)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    // 타겟 방향으로 회전
                    // 네브매쉬랑 같이 사용하고 있어서 부자연스러울지도?? 지금은 괜찮음
                    Vector3 direction = hitCollider.transform.position - transform.position;
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5);

                    State = EState.Attack;
                    return;
                }
            }

            // 탐지 범위 밖으로 나감 => IDLE 상태로
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance > stat.maxSightRange)
            {
                target = null;
                State = EState.Idle;
                return;
            }

            // 타겟으로 접근 위한 목표 지점 설정
            destination = target.transform.position;
        }

        // destination은 타겟이 있다면 타겟의 위치가, 그렇지 않다면 패트롤 위치가 들어 있음
        agent.SetDestination(destination);

        // 도착하면 Idle로 바꿔준다. 타겟이 있다면 알아서 공격상태로 전환될 것이다. (Moving -> Idle -> Moving -> Attack)
        // TODO 이슈 : 내브매쉬 목적지와 Y축 차이가 있으면 도착하지 못하고 해당 지점에서 회전함
        // 좀 더 지켜보고 Y축을 제외하고 보정하던지 해서 개선해야할 듯
        if (Vector3.Distance(transform.position, destination) < 0.1f)
        {
            isArrivedDestination = true;
            State = EState.Idle;
        }
        else
        {
            isArrivedDestination = false;
        }
    }

    protected virtual void UpdateAttack() { }

    protected virtual void UpdateDeath() { }

    protected virtual void UpdateStun()
    {
        if (stat.hp < 0) State = EState.Death;
    }

    protected abstract M_Stat SetStat();

    // 네브매쉬 영역에서 이동할 랜덤 위치를 반환
    protected Vector3 GetRandomDestination()
    {
        Vector3 randomPosition = Random.insideUnitSphere * stat.patrolRange; // 반경 내 임의의 지점
        randomPosition += transform.position; // 현재 위치 기준으로 계산

        NavMeshHit hit;
        // NavMesh.SamplePosition()은 randomPosition 기준 가장 가깝고 유효한 NavMesh지점을 찾아줌
        if (NavMesh.SamplePosition(randomPosition, out hit, stat.patrolRange, NavMesh.AllAreas))
        {
            return hit.position; // NavMesh 위의 유효한 위치 반환
        }

        return transform.position; // 유효한 위치를 찾지 못하면 현재 위치 유지
    }

    public void Damaged(int damage)
    {
        stat.hp -= damage;
        if (stat.hp <0)
        {
            State= EState.Death;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // TODO : 태그와 컴포넌트 이름 맞추기
        if (other.tag == "Trap" && State != EState.Death)
        {
            TMPTrap tmptrap = other.GetComponent<TMPTrap>();
            Damaged(tmptrap.damage);
            State = EState.Stun;
        }

        // TODO : 태그와 컴포넌트 이름 맞추기
        if (other.tag == "Player" && State != EState.Death)
        {
            TEMPlayer tmptrap = other.GetComponent<TEMPlayer>();
            tmptrap.Damaged(stat.attackPower);
        }
    }

    #region 애니메이션 이벤트 영역

    // 공격 애니메이션이 끝나고 다음 행동을 결정하는 애니메이션 프레임
    void OnAnimAttackRangeColliderOn()
    {
        foreach( var hitRangeCollider in hitRangeColliders)
        {
            hitRangeCollider.enabled = true;
        }
    }

    // 공격 애니메이션이 끝나고 다음 행동을 결정하는 애니메이션 프레임
    void OnAnimAttackRangeColliderOff()
    {
        foreach (var hitRangeCollider in hitRangeColliders)
        {
            hitRangeCollider.enabled = false;
        }
    }

    // 공격 애니메이션이 끝나고 다음 행동을 결정하는 애니메이션 프레임
    void OnAnimAttackEndEvent()
    {
        if (target != null)
        {
            State = EState.Moving;
        }
        else
        {
            State = EState.Idle;
        }
    }

    // 스턴 애니메이션이 끝나고 다음 행동을 결정하는 애니메이션 프레임
    void OnAnimStunEndEvent()
    {
        if (State != EState.Death)
        {
            State = EState.Idle;
        }
    }

    #endregion 애니메이션 이벤트 영역

    #region 기즈모
    private void OnDrawGizmos()
    {
        // 최대 시야 거리 원
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stat.maxSightRange);

        // 최소 시야 거리 원
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stat.minSightRange);

        // 시야각을 그리기 위한 벡터
        Vector3 leftBoundary = Quaternion.Euler(0, -stat.sightAngle / 2, 0) * transform.forward * stat.maxSightRange;
        Vector3 rightBoundary = Quaternion.Euler(0, stat.sightAngle / 2, 0) * transform.forward * stat.minSightRange;

        // 시야각 경계선
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }
    #endregion 기즈모
}