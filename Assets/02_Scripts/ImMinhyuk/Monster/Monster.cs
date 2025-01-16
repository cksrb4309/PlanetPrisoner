using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public abstract class Monster : MonoBehaviour, IMonsterDamagable
{

    protected GameObject target; // 플레이어
    protected Vector3 destination; // 이동할 목표 지점 

    protected float decisionInterval = 5.0f; // 움직임 여부를 결정하는 시간 간격 (초)
    protected float nextDecisionTime = 0f;

    protected bool isArrivedDestination = false;

    // 하위 컴포넌트
    protected Animator animator;
    protected NavMeshAgent agent;

    #region STAT, TODO : Json으로 빼기
    // Enum으로 몬스터 스텟 관리
    // TODO JSON으로 뺀다.
    public int Hp { get; private set; }
    public int AttackPower { get; private set; } = 5; // TODO : 2는 삭제

    protected float sightAngle = 60f;             // 시야각 (degree)
    protected float maxSightDistance = 20f;       // 시야 최대 거리
    protected float minSightDistance = 10f;       // 시야 최소 거리
    protected float attackRange = 5f;

    protected float patrolRange = 50;

    #endregion STAT, TODO : Json으로 빼기
    #region FSM
    // State가 바뀔 때 해당 애니메이션을 재생
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
                case EState.Death:
                    agent.isStopped = false;
                    animator.CrossFade("Death", 0.1f);
                    break;
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
            }
        }
    }
    public enum EState
    {
        Idle,
        Moving,
        Attack,
        Death
    }
    #endregion FSM

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
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
            case EState.Death:
                UpdateDeath();
                break;
        }
    }

    protected Vector3 GetRandomPosition()
    {
        Vector3 randomPosition = Random.insideUnitSphere * patrolRange; // 반경 내 임의의 지점
        randomPosition += transform.position; // 현재 위치 기준으로 계산

        NavMeshHit hit;
        // NavMesh.SamplePosition()은 randomPosition 기준 가장 가깝고 유효한 NavMesh지점을 찾아줌
        if (NavMesh.SamplePosition(randomPosition, out hit, patrolRange, NavMesh.AllAreas))
        {
            return hit.position; // NavMesh 위의 유효한 위치 반환
        }

        return transform.position; // 유효한 위치를 찾지 못하면 현재 위치 유지
    }


    // FSM은 Idle로 부터 시작
    protected virtual void UpdateIdle()
    {
        // 타게팅이 되어있을 경우 트래킹한다. 
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
                    destination = GetRandomPosition();
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
            // TODO : forward 방향 벡터로 탐지하는건 한계가 있음. 고쳐야함
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, attackRange))
            {
                if (hit.collider.tag == "Player")
                {
                    State = EState.Attack;
                    return;
                }
            }

            // 탐지 범위 밖으로 나감 => IDLE 상태로
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance > maxSightDistance)
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
        if (Vector3.Distance(transform.position, destination) < 1f)
        {
            isArrivedDestination = true;
            State = EState.Idle;
        }
        else
        {
            isArrivedDestination = false;
        }
    }

    protected virtual void UpdateAttack()
    {

    }

    protected virtual void UpdateDeath()
    {

    }

    public void Damaged()
    {
        // TODO
    }


    // 공격 애니메이션이 Hit되는 애니메이션 프레임
    // TODO 공격 히트 방식 변경
    void OnAnimAttackHitEvent()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, attackRange))
        {
            if (hit.collider.tag == "Player")
            {
                //TODO 데미지 계산 ex
                TEMPPlayer player = hit.collider.gameObject.GetComponent<TEMPPlayer>();
                if (player != null)
                {
                    player.Damaged(AttackPower);
                }
            }
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

    #region 기즈모
    private void OnDrawGizmos()
    {
        // 최대 시야 거리 원
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxSightDistance);

        // 최소 시야 거리 원
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minSightDistance);

        // 시야각을 그리기 위한 벡터
        Vector3 leftBoundary = Quaternion.Euler(0, -sightAngle / 2, 0) * transform.forward * maxSightDistance;
        Vector3 rightBoundary = Quaternion.Euler(0, sightAngle / 2, 0) * transform.forward * maxSightDistance;

        // 시야각 경계선
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

        // 공격용 직선 레이
        Gizmos.color = Color.green;
        float maxDistance = 5f; // 레이의 최대 길이, TODO 하드코딩 삭제
        Gizmos.DrawRay(transform.position + Vector3.up, transform.forward * maxDistance);
    }

    #endregion 기즈모

}