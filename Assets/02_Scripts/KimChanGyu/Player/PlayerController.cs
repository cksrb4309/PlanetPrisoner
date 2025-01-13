using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Need Reference")] // Header 정리

    [SerializeField] Transform cameraTransform; // 카메라의 Transform

    [Header("Move Attribute")] // Header 정리

    [SerializeField] float moveSpeed = 5f; // 이동속도
    [SerializeField] float acceleration = 3f; // 이동속도 증가속도
    [SerializeField] float deceleration = 3f; // 이동속도 감소속도
    [SerializeField] float jumpSpeed = 10f;
    [SerializeField] float gravity = -9.81f;

    [Header("Rotate Attribute")] // Header 정리

    [SerializeField] float pitchClampMax = 70f; // 카메라 각도 최대값
    [SerializeField] float pitchClampMin = -70f; // 카메라 각도 최대값
    [SerializeField] float rotateSpeed = 10f; // 회전 속도

    [Header("InputAction Settings")] // Header 정리

    [SerializeField] InputActionReference keyboardMoveInputAction; // w,a,s,d 키보드 이동 입력 값
    [SerializeField] InputActionReference mouseMoveInputAction; // 마우스 이동 입력 값
    [SerializeField] InputActionReference jumpInputAction; // 점프 입력

    CharacterController characterController; // 플레이어 캐릭터 컨트롤러
    Transform playerTransform; // 플레이어 트랜스폼

    Vector2 moveDelta = Vector2.zero; // 키보드 이동 입력 값 저장 변수
    Vector2 rotateDelta = Vector2.zero; // 마우스 이동 입력 값 저장 변수

    float velocityX = 0f; // X축 Velocity
    float velocityY = 0f; // Y축 Velocity 
    float velocityZ = 0f; // Z축 Velocity

    Vector3 velocity = Vector3.zero; // 이동에 적용하는 velocity

    float currCameraEulerX = 0f; // 카메라 x축 회전 값
    Vector3 cameraEuler = Vector3.zero;
    private void OnEnable()
    {
        keyboardMoveInputAction.action.Enable();
        mouseMoveInputAction.action.Enable();
        jumpInputAction.action.Enable();
    }
    private void OnDisable()
    {
        keyboardMoveInputAction.action.Disable();
        mouseMoveInputAction.action.Disable();
        jumpInputAction.action.Disable();
    }
    private void Start()
    {
        // GetComponent로 필요 컴포넌트 가져오기
        characterController = GetComponent<CharacterController>();

        // 플레이어 Transform 초기화
        playerTransform = transform;
    }
    private void Update()
    {
        #region Velocity X,Z 값 설정

        // 키보드 이동 입력 값 moveDelta에 저장
        moveDelta = keyboardMoveInputAction.action.ReadValue<Vector2>();

        // X축의 입력이 있을 때
        if (Mathf.Abs(moveDelta.x) > 0.1f)
        {
            // 해당 방향으로 주는 힘을 증가시킨다
            velocityX += moveDelta.x * Time.deltaTime * acceleration;

            // 이동속도만큼 제한을 둔다
            if (velocityX > moveSpeed) velocityX = moveSpeed;
            if (velocityX < -moveSpeed) velocityX = -moveSpeed;
        }

        // X축의 입력이 없다면 X축의 velocity가 0f가 아니라면
        else if (velocityX != 0f)
        {
            // velocity.x가 음수라면 1, 양수라면 -1을 sign 값에 넣는다
            float sign = -Mathf.Sign(velocityX);

            // sign 값을 이용해서 양수라면 음수 값으로, 음수라면 양수 값으로 변화시킨다
            velocityX += Time.deltaTime * deceleration * sign;

            // 만약 velocity.x가 0을 지나 부호가 달라졌다면 velocity.x의 값을 0f로 초기화 한다
            if (Mathf.Sign(velocityX) == sign) velocityX = 0f;
        }

        // Y축의 입력이 있을 때
        if (Mathf.Abs(moveDelta.y) > 0.1f)
        {
            // 해당 방향으로 주는 힘을 증가시킨다
            velocityZ += moveDelta.y * Time.deltaTime * acceleration;

            // 이동속도만큼 제한을 둔다
            if (velocityZ > moveSpeed) velocityZ = moveSpeed;
            if (velocityZ < -moveSpeed) velocityZ = -moveSpeed;
        }

        // Y축의 입력이 없다면
        else if (velocityZ != 0f)
        {
            // velocity.y가 음수라면 1, 양수라면 -1을 sign 값에 넣는다
            float sign = -Mathf.Sign(velocityZ);

            // sign 값을 이용해서 양수라면 음수 값으로, 음수라면 양수 값으로 변화시킨다
            velocityZ += Time.deltaTime * deceleration * sign;

            // 만약 velocity.z가 0을 지나 부호가 달라졌다면 velocity.z의 값을 0f로 초기화 한다
            if (Mathf.Sign(velocityZ) == sign) velocityZ = 0f;
        }

        // velocity의 x와 z의 벡터 크기 값을 구한다
        // moveSpeed로 제한을 두기 전의 값인 prevMagnitude와
        // 제한을 둔 후인 currMagnitude 값을 만든다
        float prevMagnitude = new Vector2(velocityX, velocityZ).magnitude;
        float currMagnitude = prevMagnitude > moveSpeed ? moveSpeed : prevMagnitude;

        // 만약 벡터 크기의 값이 이동속도보다 크다면 벡터 크기 값을 이동속도 값으로 초기화 한다
        if (prevMagnitude > moveSpeed) prevMagnitude = moveSpeed;

        #endregion
        #region Velocity Y 값 설정

        // 만약 캐릭터가 지상 위에 있다면
        if (characterController.isGrounded)
        {
            // velocityY를 0으로 초기화
            velocityY = 0; 

            // 점프 키를 눌렀다면
            if (jumpInputAction.action.IsPressed())
            {
                velocityY = jumpSpeed;
            }
        }

        // 만약 캐릭터가 땅 위에 없다면
        else
        {
            velocityY += gravity * Time.deltaTime;
        }

        #endregion
        #region velocity 값 갱신

        // X와 Z는 prevMagnitude로 나눈 다음에 제한을 둔 currMagnitude를 곱해준다
        velocity.Set(
            prevMagnitude > float.Epsilon ? (velocityX / prevMagnitude) * currMagnitude : 0f,
            velocityY,
            prevMagnitude > float.Epsilon ? (velocityZ / prevMagnitude) * currMagnitude : 0f);

        #endregion

        #region 카메라 회전

        // 마우스 이동 값 가져오기
        Vector2 mouseMoveValue = mouseMoveInputAction.action.ReadValue<Vector2>(); 

        // 마우스 이동 값이 있을 때
        if (mouseMoveValue != Vector2.zero)
        { 
            // 마우스 상하 이동 값은 up:1, down:-1이라 생각하면 되는데
            // 실제로 카메라의 x축 회전을 적용했을 때는 up:-1, down:1 값을 해야하기 때문에 
            // mouseMoveValue.y에 -1f을 곱해줌으로써 값을 맞춰준다
            mouseMoveValue.y *= -1f;

            // 카메라의 회전은 Clamp를 이용해서 제한을 한다
            currCameraEulerX = Mathf.Clamp(
                currCameraEulerX + (mouseMoveValue.y * Time.deltaTime * rotateSpeed),
                pitchClampMin,
                pitchClampMax);

            // 카메라의 상하 회전은 Transform eulerAngle X에 적용
            // 좌우 회전은 playerTransform.eulerAngles.y를 가져옴
            cameraTransform.rotation = Quaternion.Euler(currCameraEulerX, playerTransform.eulerAngles.y, 0f);

            // 플레이어 트랜스폼에는 x축의 마우스 회전 값만 적용한다
            playerTransform.Rotate(0, mouseMoveValue.x * rotateSpeed * Time.deltaTime, 0);

            
        }

        #endregion

        // 플레이어 이동 적용
        characterController.Move(transform.rotation * velocity * Time.deltaTime);
    }
    private void LateUpdate()
    {
        // Rigidbody의 LinearVelocity 초기화
        //Vector3 tempVelocity = playerRigidbody.linearVelocity;
        //tempVelocity.Set(0, tempVelocity.y, 0);
        //playerRigidbody.linearVelocity = tempVelocity;
        // playerRigidbody.angularVelocity는 에디터에서 Constraints Freeze Rotation x,y,z 선택했음!
    }
}
