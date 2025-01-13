using System.Linq;
using UnityEngine;


// Enum으로 몬스터 스텟 관리
enum MonsterName
{
    Golem,
    Eye,
    Murloc,
    Splinter
}

public class Monster : MonoBehaviour
{
    public int Hp { get; private set; }
    public int Speed { get; private set; }

    private GameObject target; // 플레이어
    private Vector3 destination; // 이동할 목표 지점 

    // Enum으로 몬스터 스텟 관리
    protected int[] masterHp = new int[4] { 4, 3, 2, 1 };
    protected int[] masterWalkSpeed = new int[4] { 8, 6, 4, 2 };
    protected int[] masterRunSpeed = new int[4] { 8, 6, 4, 2 };
    protected int[] masterRotationSpeed = new int[4] { 1, 2, 3, 4 };

    private int tempSpeed = 2;

    // 하위 컴포넌트
    private Animator animator;
    private MeshCollider collider;
    private Rigidbody rb;

    private float sightAngle = 60f;    // 시야각 (degree)
    private float maxSightDistance = 20f; // 시야 최대 거리
    private float minSightDistance = 10f;      // 시야 최소 거리

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        collider = GetComponent<MeshCollider>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        FindTarget();
        SelectState();
    }
    private void FixedUpdate()
    {
        Move();

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            float groundY = hit.point.y;  // 지면의 Y축 위치
            Vector3 position = transform.position;

            // Y값을 지면 위치로 맞추기
            position.y = Mathf.Max(position.y, groundY);  // 지면을 기준으로 Y축 위치를 고정
            transform.position = position;
        }
    }

    void FindTarget()
    {
        // TODO : 각 몬스터마다 시력과 청력에 따라 다르므로 하위 스크립트에서 재정의 한다.
        // 나중에 Abstract 따위로 수정하기
        // 아래는 테스트용으로 시력 구현만 해놓자.

        Collider[] hitCollidersInMaxSight = Physics.OverlapSphere(transform.position, maxSightDistance); // 최대 범위로 구를 그려 hit되는 콜라이더를 모두 탐색한다.

        foreach (Collider hitCollider in hitCollidersInMaxSight)
        {
            if (hitCollider.CompareTag("Player"))
            {
                // 이미 타겟이 있을 때는 바보가 되지 않기 위해 최대 거리와 전방위로 찾아 준다.
                float distance = Vector3.Distance(hitCollider.transform.position, transform.position); // 몬스터와 플레이어의 거리
                if (target != null && distance < maxSightDistance)
                {
                    // 플레이어를 찾았다.
                    Debug.Log($"Player Tracking! distance{distance}");
                    target = hitCollider.gameObject;
                    return;
                }

                // 최소 탐지 범위 내에서 플레이어를 찾을 때
                distance = Vector3.Distance(hitCollider.transform.position, transform.position); // 몬스터와 플레이어의 거리
                if (distance < minSightDistance)
                {
                    // 플레이어를 찾았다.
                    Debug.Log($"Player in minimum sight! distance{distance}");
                    target = hitCollider.gameObject;
                    return;
                }

                // 최대 탐지 거리 안이고 시야각 내에서 플레이어를 찾을 때
                Vector3 directionToPlayer = hitCollider.transform.position - transform.position; // 몬스터와 플레이어의 방향 벡터
                float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer); // 몬스터와 플레이어의 각도

                if (angleToPlayer <= sightAngle / 2) // 좌, 우 때문에 1/2씩 나눔
                {
                    // 시야 내에 플레이어가 있음
                    Debug.Log($"Player detected in sight! Angle {angleToPlayer}, distance{directionToPlayer}");
                    target = hitCollider.gameObject;
                    return;
                }
            }
        }

        // 못 찾았으면 타겟을 밀어준다.
        target = null;
        destination = transform.position;
        animator.Play("Idle");
    }

    // Idle, Move, Attack 결정
    void SelectState()
    {
        if (target == null)
        {
            // 
        }
        else
        {
            //  패트롤
        }
    }

    void Move()
    {
        // TODO 무브가 완료됐을 때까지 플레이어가 없다면 타겟 제거
        if(target == null)
        {
            //TODO 패트롤
        }
        else if(target !=null)
        {
            destination = target.transform.position;
            Vector3 direction = (destination - transform.position).normalized;
            float step = tempSpeed * Time.deltaTime;
            Vector3 newPosition = Vector3.MoveTowards(transform.position, destination, step);
            rb.MovePosition(newPosition);

            // 목표 위치를 향해 부드럽게 회전
            direction.y = 0f;  // y축 회전만 하도록 제한
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1 * Time.deltaTime);

            animator.Play("Walk");
        }

    }

    void Attack()
    {

    }

    void Die()
    {

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
    }
    #endregion 기즈모
}