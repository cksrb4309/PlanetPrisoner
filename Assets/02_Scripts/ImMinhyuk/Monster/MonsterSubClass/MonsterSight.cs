using log4net.Util;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MonsterSight : MonoBehaviour
{
    Monster monster;
    GameObject headSight; 
    GameObject target; // 몬스터 입장에선 플레이어

    public void Initialize(Monster _monster, GameObject headSight, GameObject target)
    {
        monster = _monster;
        this.headSight = headSight;
        this.target = target;
    }

    /// <summary>
    /// 1. OverlapSphere로 범위내의 모든 콜라이더를 탐지한다.
    /// 2. 각 콜라이더방향으로 Ray를 쏴서 Player인지 확인한다.
    /// </summary>
    public GameObject FindTargetInSight()
    {
        Collider[] hitCollidersInMaxSight = Physics.OverlapSphere(transform.position, monster.Stat.maxSightRange); // 최대 범위 내의 hit되는 콜라이더를 모두 탐색한다.

        foreach (Collider hitCollider in hitCollidersInMaxSight)
        {
            // 장애물을 고려하지 않고 오버랩 스피어 안에 플레이어가 있음
            if (hitCollider.CompareTag("Player"))
            {
                Vector3 directionToTarget = hitCollider.transform.position - headSight.transform.position; // 몬스터와 플레이어의 방향 벡터
                float distanceToTarget = Vector3.Distance(headSight.transform.position, hitCollider.transform.position); // 거리 계산

                // 이미 타겟팅이 됐을 때는 최대 탐지 범위 내에서 찾아 준다.
                if (target != null && distanceToTarget < monster.Stat.maxSightRange)
                {
                    return hitCollider.gameObject;
                }

                // 최소 탐지 범위 내에서는 플레이어를 항상 찾는다.
                if (distanceToTarget < monster.Stat.minSightRange)
                {
                    return hitCollider.gameObject;
                }

                // 최대 탐지 거리 안이고 시야각 내에서 플레이어를 찾을 때
                // 벽 따위의 장애물을 고려해서 레이로 다시 체크해준다.
                // 레이 쏴서 플레이어 아니면 Continue
                if (Physics.Raycast(headSight.transform.position, directionToTarget.normalized, out RaycastHit hitInfo, distanceToTarget))
                {
                    if (hitInfo.collider.CompareTag("Player"))
                    {
                        float angleToPlayer = Vector3.Angle(transform.forward, directionToTarget); // 몬스터와 플레이어의 각도
                        if (angleToPlayer <= monster.Stat.sightAngle / 2) // 좌, 우 때문에 1/2씩 나눔
                        {
                            return hitCollider.gameObject;
                        }
                    }
                }
            }
        }

        // 여끼까지 왔으면 플레이어를 못 찾았으므로 타겟을 밀어준다.
        return null;
    }
}
