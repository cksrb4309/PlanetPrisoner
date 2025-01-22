using UnityEngine;

public class M_EyeGroupHelper : MonoBehaviour
{
    [SerializeField] M_PatrolEye patrolEye;
    [SerializeField] M_CombatEye combatEye;
    void Start()
    {
        patrolEye = GetComponentInChildren<M_PatrolEye>();
        combatEye = GetComponentInChildren<M_CombatEye>();
    }
}
