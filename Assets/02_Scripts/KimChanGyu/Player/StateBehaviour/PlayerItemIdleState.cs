using UnityEngine;

public class PlayerItemIdleState : StateMachineBehaviour
{
    [SerializeField] PlayerItemHandler playerItemHandler;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerItemHandler == null) playerItemHandler = animator.GetComponentInParent<PlayerItemHandler>();

        //playerAnimator.
    }
}