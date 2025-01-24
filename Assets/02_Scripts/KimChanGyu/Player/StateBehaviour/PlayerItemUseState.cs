using UnityEngine;

public class PlayerItemUseState : StateMachineBehaviour
{
    [SerializeField] PlayerItemHandler playerItemHandler = null;
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerItemHandler == null) playerItemHandler = animator.GetComponentInParent<PlayerItemHandler>();

        playerItemHandler.OnItemUseComplete();
    }
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
