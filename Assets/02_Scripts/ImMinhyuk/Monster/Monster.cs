using System.Collections;
using System.Linq;
using UnityEngine;

public abstract class Monster : MonoBehaviour
{
    public int Hp { get; private set; }
    public int Speed { get; private set; } = 3; // TODO : 2는 삭제

    protected GameObject target; // 플레이어
    protected Vector3 destination; // 이동할 목표 지점 

    protected float decisionInterval = 5.0f; // 움직임 여부를 결정하는 시간 간격 (초)
    protected float nextDecisionTime = 0f;

    // 하위 컴포넌트
    protected Animator animator;
    protected MeshCollider collider;
    protected Rigidbody rb;

    #region STAT, TODO : Json으로 빼기
    // Enum으로 몬스터 스텟 관리
    // TODO JSON으로 뺀다.
    protected int[] masterHp = new int[4] { 4, 3, 2, 1 };
    protected int[] masterWalkSpeed = new int[4] { 8, 6, 4, 2 };
    protected int[] masterRunSpeed = new int[4] { 8, 6, 4, 2 };
    protected int[] masterRotationSpeed = new int[4] { 1, 2, 3, 4 };
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
                    animator.CrossFade("Die", 0.1f);
                    break;
                case EState.Idle:
                    animator.CrossFade("Idle", 0.1f);
                    break;
                case EState.Moving:
                    animator.CrossFade("Walk", 0.1f);
                    break;
                case EState.Skill:
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
        collider = GetComponent<MeshCollider>();
        rb = GetComponent<Rigidbody>();
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

    protected abstract void UpdateIdle();
    protected abstract void UpdateMoving();
    protected abstract void UpdateSkill();
    protected abstract void UpdateDie();
}