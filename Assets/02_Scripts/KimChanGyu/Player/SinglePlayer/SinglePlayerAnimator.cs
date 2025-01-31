using UnityEngine;

public class SinglePlayerAnimator : PlayerAnimator
{
    public GameObject multiPlayerMeshRenderer;
    public GameObject singlePlayerMeshRenderer;

    public Animator targetAnimator;
    protected override void Start()
    {
        playerAnimator = targetAnimator;

        multiPlayerMeshRenderer.SetActive(false);
        singlePlayerMeshRenderer.SetActive(true);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
    }
    public override void SetIsCrouch(bool isCrouch)
    {
        base.SetIsCrouch(isCrouch);
    }
    public override void SetIsGround(bool isGround)
    {
        base.SetIsGround(isGround);
    }
    public override void SetItemChangeTrigger(AnimationParameter triggerState)
    {
        base.SetItemChangeTrigger(triggerState);
    }
    public override void SetItemUseTrigger(AnimationParameter triggerState)
    {
        base.SetItemUseTrigger(triggerState);
    }
    public override bool SetJumpTrigger()
    {
        return base.SetJumpTrigger();
    }
    protected override void SetMoveSpeed(float value)
    {
        base.SetMoveSpeed(value);
    }
    public override void SetWaistValue(float value)
    {
        return;
    }
}
