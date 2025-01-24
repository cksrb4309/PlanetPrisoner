using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animator playerAnimator; // 플레이어 애니메이터

    bool isGround = true; // 현재 isGround 상태

    const int HAND_LAYER = 1;

    private void Start()
    {
        // Animator 가져오기
        playerAnimator = GetComponentInChildren<Animator>();
    }
    #region PlayerController
    void SetMoveSpeed(float value) // 이동속도 값 적용 
    {
        // 이동 값 애니메이터에 적용
        playerAnimator.SetFloat(AnimationParameter.Move.ToString(), value);
    }
    public void SetIsCrouch(bool isCrouch) // 앉은 상태 여부 적용 
    {
        playerAnimator.SetBool(AnimationParameter.IsCrouch.ToString(), isCrouch);
    }
    public bool SetJumpTrigger() // 점프 애님이 불가능하면 False, 가능하면 True 반환 후 애니메이션 적용 
    {
        // 현재 쭈구리고 있을 때
        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("CrouchMove"))
        {
            // false를 반환
            return false;
        }
        // 만약 서있다면 Jump Trigger 실행
        playerAnimator.SetTrigger(AnimationParameter.Jump.ToString());

        // true를 반환
        return true;
    }
    public void SetIsGround(bool isGround) // IsGround 여부 적용
    {
        // 현재 상태와 동일하다면 반환
        if (this.isGround == isGround) return;

        // 현재 상태에 적용
        this.isGround = isGround;

        // IsGround 적용
        playerAnimator.SetBool("IsGround", isGround);
    }
    #endregion
    #region PlayerItemHandler
    public void SetItemChangeTrigger(AnimationParameter triggerState) // 아이템 변경 시의 트리거 셋팅
    {
        playerAnimator.SetLayerWeight(HAND_LAYER, triggerState == AnimationParameter.NoItem ? 0f : 1f);

        playerAnimator.SetTrigger(triggerState.ToString());

        Debug.Log("Equip 트리거명 : " + triggerState.ToString());
    }
    public void SetItemUseTrigger(AnimationParameter triggerState)
    {
        playerAnimator.SetTrigger(triggerState.ToString());

        Debug.Log("Use 트리거명 : " + triggerState.ToString());
    }
    #endregion
    private void OnEnable()
    {
        // 함수 구독 시켜놓기
        GetComponent<PlayerController>().BindToPlayerAnimator(SetMoveSpeed);
    }
    private void OnDisable()
    {
        // 함수 구독 해제하기
        GetComponent<PlayerController>().UnbindFromPlayerAnimator(SetMoveSpeed);
    }
}