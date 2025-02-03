using UnityEngine;

public class TMPlayer : MonoBehaviour, IDamagable
{
    private Rigidbody rb;  // 리지드 바디
    private float moveSpeed = 55f;  // 이동 속도
    private float rotationSpeed = 1f;  // 회전 속도

    private Camera playerCamera;  // 카메라

    private float pitch = 0f; // X축 회전 (상하)
    private float yaw = 0f;   // Y축 회전 (좌우)

    private Vector3 moveDirection = Vector3.zero;  // 이동 방향 저장용
    private float mouseX = 0f;  // 마우스 X 입력 저장
    private float mouseY = 0f;  // 마우스 Y 입력 저장

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCamera = Camera.main; 
    }

    void Update()
    {
        // 이동 방향 계산
        float horizontal = Input.GetAxisRaw("Horizontal");  // A, D (즉시 반응)
        float vertical = Input.GetAxisRaw("Vertical");  // W, S (즉시 반응)

        // 카메라의 방향을 기준으로 이동
        Vector3 forward = playerCamera.transform.forward;  // 카메라의 앞 방향
        Vector3 right = playerCamera.transform.right;  // 카메라의 오른쪽 방향

        forward.y = 0f;  // 평면 이동 (Y축 제거)
        right.y = 0f;

        moveDirection = (forward * vertical + right * horizontal).normalized;

        // 마우스 회전 입력 저장
        mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;
    }

    void FixedUpdate()
    {
        MovePlayerWithRigidbody();  // 리지드 바디로 이동
        RotatePlayerWithMouse();    // 마우스로 회전
    }

    private void MovePlayerWithRigidbody()
    {
        if (moveDirection.magnitude >= 0.1f)
        {
            // 목표 속도 계산
            Vector3 targetVelocity = moveDirection * moveSpeed;

            // X, Z 속도만 설정하고, Y축은 중력의 영향을 받도록 남겨둠
            rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
        }
        else
        {
            // 움직이지 않으면 속도를 0으로 설정
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }

    private void RotatePlayerWithMouse()
    {
        // Y축 회전 (좌우)
        yaw += mouseX;

        // X축 회전 (상하)
        pitch -= mouseY;

        // 회전 제한 (위아래 각도 제한)
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        // 회전 적용
        playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);  // 카메라의 상하 회전
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);  // 플레이어의 좌우 회전
    }

    public void Damaged(int damage)
    {
        Debug.Log($"플레이어 피격 {damage}");
    }
}