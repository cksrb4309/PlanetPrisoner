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

    [SerializeField] CapsuleCollider leftArmCollider;
    [SerializeField] CapsuleCollider rightArmCollider;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(CoFindTarget());
    }

    protected override void UpdateIdle()
    {
        if (target != null)
        {
            State = EState.Moving;
        }
        else
        {
            if (nextDecisionTime >= decisionInterval) // N 초당 한번 IDLE할지 Move할지 선택
            {
                if(Random.Range(0, 2) == 1)
                {
                    // Do NOT (IDLE)
                }
                else
                {
                    // TODO : 터레인 따위로 y좌표가 다를 수 있으니 코드가 바뀔 수 있음
                    destination = transform.position + new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
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
            // 공격 범위 안으로 왔음
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, attackRange))
            {
                if (hit.collider.tag == "Player")
                {
                    State = EState.Skill;
                    return;
                }
            }

            // 탐지 범위 밖으로 나감
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

        // 이하 목표지점을 향해 이동
        Vector3 direction = (destination - transform.position).normalized;
        float step = Speed * Time.deltaTime;
        Vector3 newPosition = Vector3.MoveTowards(transform.position, destination, step);
        rb.MovePosition(newPosition);

        // 목표 위치를 향해 부드럽게 회전
        direction.y = 0f;  // y축 회전만 하도록 제한
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5 * Time.deltaTime);

        // 다왔으면 Idle로 바꿔준다. 타겟이 있다면 알아서 공격상태로 전환될 것이다.
        if (0.1f > Vector3.Distance(transform.position, destination))
        {
            State = EState.Idle;
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

    public bool FindTarget()
    {
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
                    //Debug.Log($"Player Tracking! distance{distance}");
                    target = hitCollider.gameObject;
                    return true;
                }

                // 최소 탐지 범위 내에서 플레이어를 찾을 때
                distance = Vector3.Distance(hitCollider.transform.position, transform.position); // 몬스터와 플레이어의 거리
                if (distance < minSightDistance)
                {
                    // 플레이어를 찾았다.
                    //Debug.Log($"Player in minimum sight! distance{distance}");
                    target = hitCollider.gameObject;
                    return true;
                }

                // 최대 탐지 거리 안이고 시야각 내에서 플레이어를 찾을 때
                Vector3 directionToPlayer = hitCollider.transform.position - transform.position; // 몬스터와 플레이어의 방향 벡터
                float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer); // 몬스터와 플레이어의 각도
                if (angleToPlayer <= sightAngle / 2) // 좌, 우 때문에 1/2씩 나눔
                {
                    // 시야 내에 플레이어가 있음
                    //Debug.Log($"Player detected in sight! Angle {angleToPlayer}, distance{directionToPlayer}");
                    target = hitCollider.gameObject;
                    return true;
                }
            }
        }

        // 못 찾았으면 타겟을 밀어준다.
        target = null;
        return true;
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
