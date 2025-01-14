using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float lookSpeed = 1f;
    public float jumpForce = 5f;

    private CharacterController controller;
    private Vector3 moveDirection;
    private float verticalVelocity;

    public bool canMove = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (canMove)
        {
            MovePlayer();
            LookAround();
        }
    }

    void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        moveDirection = transform.right * moveX + transform.forward * moveZ;
        moveDirection *= moveSpeed;

        if (controller.isGrounded)
        {
            verticalVelocity = -0.5f; // 중력
            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = jumpForce;
            }
        }
        else
        {
            verticalVelocity -= 9.81f * Time.deltaTime; // 중력 적용
        }

        moveDirection.y = verticalVelocity;
        controller.Move(moveDirection * Time.deltaTime);
    }

    void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        transform.Rotate(Vector3.up * mouseX);
        Camera cam = Camera.main;
        cam.transform.Rotate(Vector3.left * mouseY);
    }
}
