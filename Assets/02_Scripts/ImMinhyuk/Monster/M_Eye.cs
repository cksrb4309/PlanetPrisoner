using System.Collections;
using UnityEngine;

public class M_Eye : Monster
{
    MonsterSight monsterSight; // 시력 클래스        
    [SerializeField] protected M_Eye parterEye; // 파트너 Eye

    // 죽었을 때 M_EyeGroupHelper에게 알려줘서 해당 Eye를 Null로 밀어주는 용도의 delegate
    public delegate void DeathEventHandler();
    public event DeathEventHandler OnDeath; // 이벤트 작성 문법 [event] [delegate 타입] [이벤트 이름]
    // public event Action OnDeath; // 위 두줄 대신 이렇게도 사용 가능

    protected override void Start()
    {
        base.Start();

        // 시력 세팅
        monsterSight = gameObject.AddComponent<MonsterSight>();
        monsterSight.Initialize(this, headSight, target);

        // 타게팅(플레이어) 무한루프 코루틴
        StartCoroutine(CoFindTarget());
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle();
        SetAnimDefault();
    }

    protected override void UpdateAttack()
    {
        SetAnimFaster();
    }

    protected override void UpdateDeath()
    {
        OnDeath?.Invoke(); // 리스너 패턴으로 M_EyeGroupHelper에서 이 객체를 null로 밀어줘서 사망처리 해준다.
    }

    IEnumerator CoFindTarget()
    {
        while (true)
        {
            target = monsterSight.FindTargetInSight();
            yield return new WaitForSeconds(0.1f);  // N초마다 타겟을 탐색
        }
    }

    // 파트너 Eye가 누구인지 설정해줌
    public void SetPartnerEye(M_Eye _partnerEye)
    {
        parterEye = _partnerEye;
    }

    // 파트너의 사망을 설정하는 함수
    public void SetPartnerDie()
    {
        parterEye = null;
    }

    void Synergy()
    {
        // TODO 같이 싸우면 시너지 효과를 주고 싶은데..
    }

    // Json에서 스텟을 가져온다. 하위 클래스에서 재정의
    protected override M_Stat SetStat()
    {
        return null;
    }
}
