using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class M_Golem : Monster, IMonsterHearing
{
    // TODO JSON으로 뺀다.
    private float sightAngle = 60f;             // 시야각 (degree)
    private float maxSightDistance = 20f;       // 시야 최대 거리
    private float minSightDistance = 10f;       // 시야 최소 거리
    private float attackRange = 5f;

    bool isArrivedDestination = false;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(CoFindTarget()); // 플레이어 탐지는 항상 한다(while(true))
    }

    // FSM은 Idle로 부터 시작
    protected override void UpdateIdle()
    {
        if (target != null)
        {
            State = EState.Moving;
        }
        else
        {
            if (nextDecisionTime >= decisionInterval) // N 초에 한번 IDLE할지 Move할지 선택
            {
                if(Random.Range(0, 2) == 1 && isArrivedDestination == true)
                {
                    // Do NOT (IDLE)
                }
                else
                {
                    // TODO : 터레인 따위로 y좌표가 다를 수 있으니 코드가 바뀔 수 있음
                    // TODO : 나중에 NavMesh로 변경
                    destination = transform.position + new Vector3(Random.Range(-22f, 22), 0, Random.Range(-22, 22));
                    State = EState.Moving;
                }
                nextDecisionTime = 0f;
            }
        }
    }

    protected override void UpdateMoving()
    {
        if(target !=null)
        {
            // 공격 범위 안으로 왔음 => 공격
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, attackRange))
            {
                if (hit.collider.tag == "Player")
                {
                    State = EState.Skill;
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
            // TODO 터레인이 없다고 가정하고 일단 y는 0으로 밀어줌
            destination.y = 0;
        }

        // 목표 위치를 향해 이동
        Vector3 direction = (destination - transform.position).normalized;
        Vector3 newPosition = Vector3.MoveTowards(transform.position, destination, MoveSpeed * Time.deltaTime);
        rb.MovePosition(newPosition);

        // 목표 위치를 향해 부드럽게 회전
        direction.y = 0f;  // y축 회전만 하도록 제한
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);

        // 도착으면 Idle로 바꿔준다. 타겟이 있다면 알아서 공격상태로 전환될 것이다. (Idle -> Moving -> Skill)
        if (0.1f > Vector3.Distance(transform.position, destination))
        {
            isArrivedDestination = true;
            State = EState.Idle;
        }
        else
        {
            isArrivedDestination = false;
        }
    }

    protected override void UpdateSkill()
    {
        //TODO
    }

    protected override void UpdateDie()
    {
        //TODO
    }

    IEnumerator CoFindTarget()
    {
        while (true)
        {
            FindTarget();
            yield return new WaitForSeconds(0.1f);  // N초마다 반복
        }
    }

    /// <summary>
    /// 1. OverlapSphere로 범위내의 모든 콜라이더를 탐지한다.
    /// 2. 각 콜라이더방향으로 Ray를 쏴서 Player인지 확인한다.
    /// </summary>
    public bool FindTarget()
    {
        Collider[] hitCollidersInMaxSight = Physics.OverlapSphere(transform.position, maxSightDistance); // 최대 범위 내의 hit되는 콜라이더를 모두 탐색한다.

        foreach (Collider hitCollider in hitCollidersInMaxSight)
        {
            if (hitCollider.CompareTag("Player"))
            {
                // 벽 따위의 장애물을 고려해서 한번 더 체크해준다.
                Vector3 directionToTarget = hitCollider.transform.position - transform.position; // 몬스터와 플레이어의 방향 벡터
                float distanceToTarget = Vector3.Distance(transform.position, hitCollider.transform.position); // 거리 계산

                // 레이 쏴서 플레이어 아니면 Continue;
                if (Physics.Raycast(transform.position, directionToTarget.normalized, out RaycastHit hitInfo, distanceToTarget))
                {                    
                    if (!hitInfo.collider.CompareTag("Player"))
                    {
                        continue; 
                    }
                }

                // 이미 타겟팅이 됐을 때는 최대 탐지 범위 내에서 찾아 준다.
                if (target != null && distanceToTarget < maxSightDistance)
                {
                    target = hitCollider.gameObject;
                    return true;
                }

                // 최소 탐지 범위 내에서 플레이어를 찾을 때
                if (distanceToTarget < minSightDistance)
                {
                    target = hitCollider.gameObject;
                    return true;
                }

                // 최대 탐지 거리 안이고 시야각 내에서 플레이어를 찾을 때
                float angleToPlayer = Vector3.Angle(transform.forward, directionToTarget); // 몬스터와 플레이어의 각도
                if (angleToPlayer <= sightAngle / 2) // 좌, 우 때문에 1/2씩 나눔
                {
                    target = hitCollider.gameObject;
                    return true;
                }
            }
        }

        // 여끼까지 왔으면 플레이어를 못 찾았으므로 타겟을 밀어준다.
        target = null;
        return false;
    }

    // 공격 애니메이션이 Hit되는 시점
    // 이것도 부모 클래스로 올릴 수 있겠는데?
    void OnAnimAttackHitEvent()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, attackRange))
        {
            if (hit.collider.tag == "Player")
            {
                // 데미지 계산 ex
                TEMPPlayer player = hit.collider.gameObject.GetComponent<TEMPPlayer>();
                if (player != null)
                {
                    player.Damaged(masterAttackPower[0]);
                }
            }
        }
    }

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
