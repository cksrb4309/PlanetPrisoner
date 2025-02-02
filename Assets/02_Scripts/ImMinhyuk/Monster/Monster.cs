using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using static Codice.Client.Common.EventTracking.TrackFeatureUseEvent.Features.DesktopGUI.Filters;

public abstract class Monster : MonoBehaviour, IMonsterDamagable, ITrapable
{
    protected GameObject target; // 플레이어
    protected GameObject preFrameTarget; // 이전 프레임의 타겟
    [SerializeField] protected Vector3 destination; // 이동할 목표 지점 
    protected bool isArrivedDestination = false; // 목표지점에 도착했는지 토글용

    protected const float decisionInterval = 5.0f; // 움직임 여부를 결정하는 시간 간격 (초)
    protected float nextDecisionTime = 0f;

    [SerializeField] protected CapsuleCollider[] hitRangeColliders; // 공격 Hit 판정용
    [SerializeField] protected GameObject headSight; //헤드 To 플레이어 시력 탐지 레이용

    [SerializeField] AudioClip[] basicSounds;              // Idle, Move 소리들
    [SerializeField] AudioClip[] attackSounds;             // 공격 소리들
    [SerializeField] AudioClip[] dieSounds;                // 사망 소리들

    LayerMask playerLayerMask;    // 주변에 플레이어가 있는 지 확인할 레이어마스크

    // 하위 컴포넌트
    protected Animator animator;
    protected NavMeshAgent agent;
    protected AudioSource audio;
    public MonsterAnimEvent monsterAnimEvent;

    [SerializeField] // 스텟 확인용으로 달아줬음
    public M_Stat Stat { get; private set; } // json으로부터 데이터를 받아온다.

    #region FSM
    public enum EState
    {
        Idle,
        Moving,
        Alert,
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
                case EState.Alert: // Eye 확장용 테스트로 추가
                    agent.isStopped = true;
                    animator.CrossFade("Alert", 0.1f);
                    break;
                case EState.Attack:
                    agent.isStopped = true;
                    animator.CrossFade("Attack02", 0.1f);
                    break;
                case EState.Stun:
                    agent.isStopped = true;
                    animator.CrossFade("Stun", 0.1f);
                    break;
                case EState.Death:
                    MonsterHearing.RemoveAudioSource(audio);
                    agent.isStopped = true;
                    animator.CrossFade("Death", 0.1f);
                    break;
            }
        }
    }
    #endregion FSM

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        Stat = SetStat();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = Stat.speed;

        MonsterHearing.AddAudioSource(audio);

        playerLayerMask = LayerMask.GetMask("Player");
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
            case EState.Alert:
                UpdateAlert();
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
                    SetDestination(GetRandomDestination_InNavMesh());
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
            // 공격 범위 내의 hit되는 Player 레이어마스크 콜라이더를 모두 탐색한다.
            Collider[] hitCollidersInMaxSight = Physics.OverlapSphere(transform.position, Stat.attackRange, playerLayerMask);

            foreach (Collider hitCollider in hitCollidersInMaxSight)
            {
                // 타겟 방향으로 회전
                // 네브매쉬랑 같이 사용하고 있어서 부자연스러울지도?? 지금은 괜찮음
                Vector3 direction = hitCollider.transform.position - transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 90);

                State = EState.Attack;
                return;
            }

            // 탐지 범위 밖으로 나감 => IDLE 상태로
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance > Stat.maxSightRange)
            {
                target = null;
                preFrameTarget = null;
                State = EState.Idle;
                return;
            }

            // 타겟으로 접근 위한 목표 지점 설정
            SetDestination(SetNearDestination_InNavMesh(target.transform.position));
        }

        // destination은 타겟이 있다면 타겟의 위치가, 그렇지 않다면 패트롤 목표 위치가 들어 있음
        agent.SetDestination(destination);

        // 도착하면 Idle로 바꿔준다. 타겟이 있다면 알아서 공격상태로 전환될 것이다. (Moving -> Idle -> Moving -> Attack)
        // y축 오차는 고려하지 않기 위해 한번 컨버팅 
        Vector3 flatPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 flatDestination = new Vector3(destination.x, 0, destination.z);

        if (Vector3.Distance(flatPosition, flatDestination) < 0.2f)
        {
            isArrivedDestination = true;
            State = EState.Idle;
        }
        else
        {
            isArrivedDestination = false;
        }
    }

    protected virtual void UpdateAlert() 
    {
        State = EState.Idle;
    }

    protected virtual void UpdateAttack() { }

    protected virtual void UpdateDeath() { }

    protected virtual void UpdateStun()
    {
        if (Stat.hp < 0) State = EState.Death;
    }

    protected abstract M_Stat SetStat();

    // 네브매쉬 영역에서 이동할 랜덤 위치를 반환
    protected Vector3 GetRandomDestination_InNavMesh()
    {
        Vector3 randomPosition = Random.insideUnitSphere * Stat.patrolRange; // 반경 내 임의의 지점
        randomPosition += transform.position; // 현재 위치 기준으로 계산

        NavMeshHit hit;
        // NavMesh.SamplePosition()은 randomPosition 기준 가장 가깝고 유효한 NavMesh지점을 찾아줌
        if (NavMesh.SamplePosition(randomPosition, out hit, Stat.patrolRange, NavMesh.AllAreas))
        {
            return hit.position; // NavMesh 위의 유효한 위치 반환
        }

        return transform.position; // 유효한 위치를 찾지 못하면 현재 위치 유지
    }

    protected Vector3 SetNearDestination_InNavMesh(Vector3 _destination)
    {
        NavMeshHit hit;
        // NavMesh.SamplePosition()은 _destination 기준 가장 가깝고 유효한 NavMesh지점을 찾아줌
        if (NavMesh.SamplePosition(_destination, out hit, Stat.patrolRange, NavMesh.AllAreas))
        {
            return hit.position; // NavMesh 위의 유효한 위치 반환
        }

        return transform.position; // 유효한 위치를 찾지 못하면 현재 위치 유지
    }

    protected void SetDestination(Vector3 _destination)
    {
        destination = _destination;
    }

    protected void SetStateChangeNow()
    {
        nextDecisionTime = decisionInterval;
    }

    // 공격하는 쪽에서 호출할 수 있도록 public으로 뚫어줌
    public void Damaged(int damage)
    {
        Stat.hp -= damage;
        if (Stat.hp < 0)
        {
            State = EState.Death;
        }
    }

    public void TrapTrigger()
    {
        // TODO : 트랩 대미지는 N? 1? 0?
        // 아 이걸 협의해야한다고 했던거구나. 일단 1
        Damaged(1);
        State = EState.Stun;
    }

    private void OnTriggerEnter(Collider other)
    {
        // TODO : 태그와 컴포넌트 이름 맞추기
        if (other.tag == "Player" && State != EState.Death)
        {
            // TODO : 플레이어에게 접근해서 산소통 데미지 등의 효과 
            TMPlayer tmptrap = other.GetComponent<TMPlayer>();
            tmptrap.Damaged(Stat.attackPower);
        }
    }

    #region MonsterAnimEvent 초기화 함수
    public CapsuleCollider[] GetHitRangeColliders()
    {
        return hitRangeColliders;
    }

    public AudioSource GetAudioSoruce()
    {
        return audio;
    }

    public AudioClip[] GetoBasicAudioClip()
    {
        return basicSounds;
    }

    public AudioClip[] GetAttackAudioClip()
    {
        return attackSounds;
    }

    public AudioClip[] GetDieAudioClip()
    {
        return dieSounds;
    }
    #endregion MonsterAnimEvent 초기화 함수

    #region 기즈모
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        // 최대 시야 거리 원
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Stat.maxSightRange);

        // 최소 시야 거리 원
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Stat.minSightRange);

        // 시야각을 그리기 위한 벡터
        Vector3 leftBoundary = Quaternion.Euler(0, -Stat.sightAngle / 2, 0) * transform.forward * Stat.maxSightRange;
        Vector3 rightBoundary = Quaternion.Euler(0, Stat.sightAngle / 2, 0) * transform.forward * Stat.minSightRange;

        // 시야각 경계선
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }
    #endregion 기즈모

    #region 몬스터 피격 헬퍼 버튼

    [CustomEditor(typeof(M_Golem))]
    public class M_GolemDamagedButton : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector(); // 기존 인스펙터 UI 유지

            M_Golem targetObject = (M_Golem)target;

            if (targetObject == null)
            {
                EditorGUILayout.HelpBox("No valid target selected.", MessageType.Warning);
                return;
            }

            if (GUILayout.Button("몬스터 체력 감소"))
            {
                targetObject.Damaged(1);
            }
        }
    }
    [CustomEditor(typeof(M_Murloc))]
    public class M_MurlocDamagedButton : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector(); // 기존 인스펙터 UI 유지

            M_Murloc targetObject = (M_Murloc)target;

            if (targetObject == null)
            {
                EditorGUILayout.HelpBox("No valid target selected.", MessageType.Warning);
                return;
            }

            if (GUILayout.Button("몬스터 체력 감소"))
            {
                targetObject.Damaged(1);
            }
        }
    }
    [CustomEditor(typeof(M_Splinter))]
    public class M_SplinterDamagedButton : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector(); // 기존 인스펙터 UI 유지

            M_Splinter targetObject = (M_Splinter)target;

            if (targetObject == null)
            {
                EditorGUILayout.HelpBox("No valid target selected.", MessageType.Warning);
                return;
            }

            if (GUILayout.Button("몬스터 체력 감소"))
            {
                targetObject.Damaged(1);
            }
        }
    }
    [CustomEditor(typeof(M_EyeCombat))]
    public class M_EyeCombatDamagedButton : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector(); // 기존 인스펙터 UI 유지

            M_EyeCombat targetObject = (M_EyeCombat)target;

            if (targetObject == null)
            {
                EditorGUILayout.HelpBox("No valid target selected.", MessageType.Warning);
                return;
            }

            if (GUILayout.Button("몬스터 체력 감소"))
            {
                targetObject.Damaged(1);
            }
        }
    }

    [CustomEditor(typeof(M_EyePatrol))]
    public class M_EyePatrolDamagedButton : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector(); // 기존 인스펙터 UI 유지

            M_EyePatrol targetObject = (M_EyePatrol)target;

            if (targetObject == null)
            {
                EditorGUILayout.HelpBox("No valid target selected.", MessageType.Warning);
                return;
            }

            if (GUILayout.Button("몬스터 체력 감소"))
            {
                targetObject.Damaged(1);
            }
        }
    }
    #endregion 몬스터 피격 헬퍼 버튼
}