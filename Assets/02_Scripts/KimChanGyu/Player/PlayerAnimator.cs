using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animator playerAnimator; // 플레이어 애니메이터

    PlayerAnimationState currState; // 현재 상태

    bool isGround = true;
    private void Start()
    {
        // Animator 가져오기
        playerAnimator = GetComponentInChildren<Animator>();
    }
    public PlayerAnimationState GetState() // 현재 애니메이션 상태 반환
    {
        return currState;
    }
    void SetMoveSpeed(float value)
    {
        playerAnimator.SetFloat("Move", value);
    }
    public void SetIsCrouch(bool isCrouch)
    {
        playerAnimator.SetBool("IsCrouch", isCrouch);
    }
    public bool SetJumpTrigger()
    {
        // 현재 쭈구리고 있을 때
        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("CrouchMove"))
        {
            // false를 반환
            return false;
        }
        // 만약 서있다면 Jump Trigger 실행
        playerAnimator.SetTrigger("Jump");

        // true를 반환
        return true;
    }
    public void SetIsGround(bool isGround)
    {
        if (this.isGround == isGround) return;

        this.isGround = isGround;

        playerAnimator.SetBool("IsGround", isGround);
    }
    private void OnEnable()
    {
        GetComponent<PlayerController>().BindToPlayerAnimator(SetMoveSpeed);
    }
    private void OnDisable()
    {
        GetComponent<PlayerController>().UnbindFromPlayerAnimator(SetMoveSpeed);
    }
}

public enum PlayerAnimationState
{
    Stand,
    Crouch,
    Jump
}