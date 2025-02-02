using UnityEngine;

public class M_EyePatrol : M_Eye
{
    protected override void UpdateMoving()
    {
        // 컴뱃 Eye가 살아있고 플레이러를 식별 했을 때
        if (IsAliveCombatEye() && target!=null)
        {
            // 최초 식별시에는 놀라는 애니메이션을 넣어줌
            if (preFrameTarget == null)
            {
                preFrameTarget = target;

                // 타겟 방향으로 회전
                // 네브매쉬랑 같이 사용하고 있어서 부자연스러울지도?? 지금은 괜찮음
                Vector3 direction = target.transform.position - transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 90);

                State = EState.Alert;
                return;
            }

            // 컴뱃 Eye와 가까이 있다면 Monster 클래스 UpdateMoving()와 같이 패트롤
            if (IsNearCombatEye())
            {
                base.UpdateMoving();
            }
            // 컴뱃 Eye와 멀리 있다면 CombatEye쪽으로 도망친다.
            else
            {
                GoToCombatEye();
            }
        }
        // 평소에는 Monster 클래스 UpdateMoving()와 같이 패트롤
        else
        {
            preFrameTarget = null;
            base.UpdateMoving();
        }
    }

    protected override void UpdateAlert()
    {
        // DO NOT
    }

    // 컴뱃 Eye가 살아있는지 여부 논리값 리턴
    bool IsAliveCombatEye()
    {
        if (parterEye != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 컴뱃 Eye가 가까이에 있는지 여부 논리값 리턴
    bool IsNearCombatEye()
    {
        float distance = Vector3.Distance(transform.position, parterEye.transform.position);
        if (distance<5)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 컴뱃 Eye 쪽으로 Destination을 찍어준다.
    void GoToCombatEye()
    {
        SetDestination(SetNearDestination_InNavMesh(parterEye.transform.position));
        agent.SetDestination(destination);
        State = EState.Moving;
    }

    // Json에서 스텟을 가져온다.
    protected override M_Stat SetStat()
    {
        M_Stat _stat;

        if (!MonsterStat.Instance.StatDict.TryGetValue("PatrolEye", out _stat))
        {
            Debug.LogWarning($"PatrolEye not found in stat Dictionary");
        }

        return _stat;
    }
}
