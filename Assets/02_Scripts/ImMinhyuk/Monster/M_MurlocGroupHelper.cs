using System.Collections;
using UnityEngine;

public class M_MurlocGroupHelper : MonoBehaviour
{
    [SerializeField] M_Murloc[] murlocs; // 집단에 속한 모든 멀록
    private int id = 0;

    private void Start()
    {
        murlocs = GetComponentsInChildren<M_Murloc>();

        foreach (M_Murloc murloc in murlocs)
        {
            murloc.SetGroupMurloc(murlocs, id++); // 해당 멀록이 몇번 멀록인지 ID를 부여한다.
            murloc.OnDeath += OnDieMurloc; // 멀록의 사망이벤트를 등록한다.
        }
    }

    void OnDieMurloc(int no)
    {
        // 멀록 그룹원에게 사망한 사실을 알린다.
        foreach (M_Murloc murloc in murlocs)
        {
            if (murloc != null)
            {
                murloc.SetMemberDie(no);
            }
        }
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
