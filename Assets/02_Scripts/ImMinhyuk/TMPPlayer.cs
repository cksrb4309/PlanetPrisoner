using UnityEngine;

public class TEMPPlayer : MonoBehaviour
{
    private Rigidbody rb;  // 리지드 바디
    private float moveSpeed = 15f;  // 이동 속도
    private float rotationSpeed = 1f;  // 회전 속도

    private Camera playerCamera;  // 카메라

    private float pitch = 0f; // X축 회전 (상하)
    private float yaw = 0f;   // Y축 회전 (좌우)

    void Start()
    {
        // 리지드 바디와 카메라 가져오기
        rb = GetComponent<Rigidbody>();
        playerCamera = Camera.main;  // 메인 카메라를 가져옵니다.
    }

    void Update()
    {
        MovePlayerWithRigidbody();  // 리지드 바디로 이동
        RotatePlayerWithMouse();    // 마우스로 회전
    }

    private void MovePlayerWithRigidbody()
    {
        // Get input for movement (WASD 또는 화살표 키)
        float horizontal = Input.GetAxisRaw("Horizontal");  // A, D (즉시 반응)
        float vertical = Input.GetAxisRaw("Vertical");  // W, S (즉시 반응)

        // 카메라의 방향을 기준으로 이동
        Vector3 forward = playerCamera.transform.forward;  // 카메라의 앞 방향
        Vector3 right = playerCamera.transform.right;  // 카메라의 오른쪽 방향

        // Y축을 0으로 설정하여 평면 이동 (카메라는 위아래로 기울어질 수 있음)
        forward.y = 0f;
        right.y = 0f;

        // 이동 방향 계산
        Vector3 direction = forward * vertical + right * horizontal;
        direction.Normalize();  // 방향 벡터 정규화

        // 리지드 바디를 이용해 이동
        if (direction.magnitude >= 0.1f)
        {
            Vector3 move = direction * moveSpeed * Time.deltaTime;
            rb.MovePosition(rb.position + move);  // 리지드 바디로 물리 기반 이동
        }
    }

    private void RotatePlayerWithMouse()
    {
        // 마우스 이동에 따라 회전
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

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
}