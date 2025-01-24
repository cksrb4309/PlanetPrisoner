using System.Collections;
using UnityEngine;

public class PlayerItemAttackState : StateMachineBehaviour
{
    [SerializeField] PlayerItemHandler playerItemHandler = null;
    [SerializeField] float attackDelay = 1f;
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerItemHandler == null) playerItemHandler = animator.GetComponentInParent<PlayerItemHandler>();

        playerItemHandler.OnAttackComplete();
    }
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerItemHandler == null) playerItemHandler = animator.GetComponentInParent<PlayerItemHandler>();

        playerItemHandler.StartCoroutine(AttackCoroutine());
    }
    IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(attackDelay);

        playerItemHandler.OnItemAttack();
    }
}
