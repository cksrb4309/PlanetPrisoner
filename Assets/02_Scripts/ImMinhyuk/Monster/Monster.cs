using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public abstract class Monster : MonoBehaviour
{
    public int Hp { get; private set; }
    public int MoveSpeed { get; private set; } = 5; // TODO : 2는 삭제
    public int RotationSpeed { get; private set; } = 5; // TODO : 2는 삭제

    protected GameObject target; // 플레이어
    protected Vector3 destination; // 이동할 목표 지점 

    protected float decisionInterval = 5.0f; // 움직임 여부를 결정하는 시간 간격 (초)
    protected float nextDecisionTime = 0f;

    // 하위 컴포넌트
    protected Animator animator;
    protected NavMeshAgent agent;

    #region STAT, TODO : Json으로 빼기
    // Enum으로 몬스터 스텟 관리
    // TODO JSON으로 뺀다.
    protected int[] masterHp = new int[4] { 4, 3, 2, 1 };
    protected int[] masterWalkSpeed = new int[4] { 8, 6, 4, 2 };
    protected int[] masterRunSpeed = new int[4] { 8, 6, 4, 2 };
    protected int[] masterRotationSpeed = new int[4] { 1, 2, 3, 4 };

    protected int[] maxSightRange = new int[4] { 50, 59, 58, 57 };

    protected int[] masterAttackPower = new int[4] { 1, 2, 3, 4 };
    #endregion STAT, TODO : Json으로 빼기
    #region FSM
    // State가 바뀔 때 해당 애니메이션을 재생하는 구조 
    protected EState _state = EState.Idle;
    public virtual EState State
    {
        get { return _state; }
        set
        {
            _state = value;

            switch (_state)
            {
                case EState.Die:
                    agent.isStopped = false; 
                    animator.CrossFade("Die", 0.1f);
                    break;
                case EState.Idle:
                    agent.isStopped = true;  
                    animator.CrossFade("Idle", 0.1f);
                    break;
                case EState.Moving:
                    agent.isStopped = false;
                    animator.CrossFade("Walk", 0.1f);
                    break;
                case EState.Skill:
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
        Skill,
        Die
    }
    #endregion FSM

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
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
            case EState.Skill:
                UpdateSkill();
                break;
            case EState.Die:
                UpdateDie();
                break;
        }
    }

    protected Vector3 GetRandomPosition()
    {
        Vector3 randomPosition = Random.insideUnitSphere * maxSightRange[0]; // 반경 내 임의의 지점
        randomPosition += transform.position; // 현재 위치 기준으로 계산

        NavMeshHit hit;
        // NavMesh.SamplePosition()은 randomPosition 기준 가장 가깝고 유효한 NavMesh지점을 찾아줌
        if (NavMesh.SamplePosition(randomPosition, out hit, maxSightRange[0], NavMesh.AllAreas))
        {
            return hit.position; // NavMesh 위의 유효한 위치 반환
        }

        return transform.position; // 유효한 위치를 찾지 못하면 현재 위치 유지
    }

    protected abstract void UpdateIdle();
    protected abstract void UpdateMoving();
    protected abstract void UpdateSkill();
    protected abstract void UpdateDie();
}