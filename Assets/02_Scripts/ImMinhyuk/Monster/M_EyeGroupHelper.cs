using UnityEngine;

// Eye 몬스터들을 하위 오브젝트로 갖고 있는 상위 오브젝트.
public class M_EyeGroupHelper : MonoBehaviour
{
    [SerializeField] M_EyePatrol patrolEye; // 인스펙터로 연결
    [SerializeField] M_EyeCombat combatEye; // 인스펙터로 연결

    void Start()
    {
        patrolEye = GetComponentInChildren<M_EyePatrol>();
        combatEye = GetComponentInChildren<M_EyeCombat>();

        // 서로의 파트너를 지정
        patrolEye.SetPartnerEye(combatEye);
        combatEye.SetPartnerEye(patrolEye);

        combatEye.OnDeath += OnDieCombatEye; // 컴뱃 Eye의 사망이벤트를 등록한다.
        patrolEye.OnDeath += OnDiePatrolEye; // 패트롤 Eye의 사망이벤트를 등록한다.
    }

    void OnDiePatrolEye()
    {
        if (patrolEye != null) combatEye.SetPartnerDie(); // 패트롤 Eye 사망처리
    }

    void OnDieCombatEye()
    {
        if (combatEye != null) patrolEye.SetPartnerDie(); // 전투 Eye 사망처리
    }
    private void OnEnable()
    {
        NextDayController.Subscribe(DestroyMonster, ActionType.NextDayTransition);
        NextDayController.Subscribe(DestroyMonster, ActionType.GameOverTransition);
    }
    private void OnDisable()
    {
        NextDayController.Unsubscribe(DestroyMonster, ActionType.NextDayTransition);
        NextDayController.Unsubscribe(DestroyMonster, ActionType.GameOverTransition);
    }
    void DestroyMonster() => Destroy(gameObject);
}
