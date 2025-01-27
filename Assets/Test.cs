using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEditorInternal;

public class PlayerController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer; // ĳ������ ��������Ʈ ������
    public Player player;

    public GameObject[] enableObjects; // �׾����� ��Ȱ��ȭ�� ������Ʈ ����Ʈ

    private float dashCooldownTimer; // �뽬 ��Ÿ���� ����ϱ� ���� Ÿ�̸�

    private int select1 = 1;
    private int select2 = 1;

    private bool isHit = false;
    private bool isDie = false;

    [SerializeField] private float invincibilityTime = 1f; // ĳ���� ���� �ð�
    [SerializeField] private float moveSpeed = 10f; // ĳ���� �̵� �ӵ�
    [SerializeField] private float jumpSpeed = 10f; // ĳ���� ���� �ӵ�

    private Rigidbody2D rb; // ĳ������ Rigidbody2D

    private Vector2 moveDirection; // �Էµ� �̵� ����

    private bool isJump; // ���� ������ ���θ� ��Ÿ���� �÷���
    private bool isDashing; // �뽬 ������ ���θ� ��Ÿ���� �÷���

    public UnityEditor.Animations.AnimatorStateMachine stateMachine; // ĳ������ ���¸� ������ ���� �ӽ�
    RopeActive grappling;

    private void Awake()
    {

        // ������Ʈ �ʱ�ȭ
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        player = new Player(100, 30f, 1f, 0.5f);
        Debug.Log("player : " + player);

        // ���� �ӽ� �ʱ�ȭ
        stateMachine = new UnityEditor.Animations.AnimatorStateMachine(this);
    }

    private void Start()
    {
        // ���� �ӽ��� �ʱ� ���¸� Idle�� ����
        stateMachine.Initalize(stateMachine.idleState);
        grappling = GetComponent<RopeActive>();
    }

    private void FixedUpdate()
    {
        // �뽬 ���� �ƴ� ��쿡�� �̵� ó��
        if (!isDashing)
        {
            ApplyMovement();
        }
    }

    private void Update()
    {
        // ���� �ӽ��� ���� ���¸� ����
        stateMachine.Execute();

        // ���콺 ��ġ�� ������� ĳ������ ��������Ʈ ���� ����
        Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = UnityEngine.Camera.main.ScreenToWorldPoint(
            new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, Mathf.Abs(UnityEngine.Camera.main.transform.position.z))
        );

        // ���콺 ��ġ�� ĳ������ ���� ��ġ�� ���� ĳ������ ���� ����
        if ((mouseWorldPosition.x > transform.position.x && spriteRenderer.flipX) ||
            (mouseWorldPosition.x < transform.position.x && !spriteRenderer.flipX))
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        // �뽬 ��Ÿ�� Ÿ�̸� ������Ʈ
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    // �÷��̾ �������� ���� ��
    public void TakeDamage(int damage)
    {
        Debug.Log(player != null ? "Not Null" : "Null");
        Debug.Log(player.ToString());

        if (player.IsAlive() && !isHit && !isDie)
        {
            isHit = true;
            player.Damage(damage);

            //player �� �ٲ��(��ġ�� ��� or ���� ���)
            stateMachine.TransitionTo(stateMachine.hurtState);

            //Player ���� �ð�
            Invoke("Invincibility", invincibilityTime);

            Debug.Log($"Player Hp : {player.PlayerHp}");
        }
        if (!player.IsAlive() && !isDie)
        {
            Die();
        }
    }

    //�����ð�
    private void Invincibility()
    {
        isHit = false;
    }


    public void Die()
    {
        foreach (var enableObject in enableObjects)
        {
            enableObject.SetActive(false);
            Debug.Log("��Ȱ��ȭ");
        }
        stateMachine.TransitionTo(stateMachine.dieState);
        Debug.Log("Player Die");
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        isDie = true;

    }


    //select1���� �������� �޼ҵ� 
    public int GetSelect1()
    {
        return select1;
    }

    //select2���� �������� �޼ҵ�
    public int GetSelect2()
    {
        return select2;
    }

    //Q(��(1), Ǯ(2), ����(3))
    public void OnSelect1(InputValue value)
    {
        if (select1 >= 3)
        {
            select1 = 1;
        }
        else
        {
            select1 += 1;
        }
        // SlowTime �۵� �� �ð� ���� �ڷ�ƾ ����
        //SlowTime.Instance.Slow();
        StopAllCoroutines(); // ���� �ڷ�ƾ ���� (�ߺ� ����)
        //StartCoroutine(ResetTimeScale());
    }

    //E
    public void OnSelect2(InputValue value)
    {
        if (select2 >= 3)
        {
            select2 = 1;
        }
        else
        {
            select2 += 1;
        }
        // SlowTime �۵� �� �ð� ���� �ڷ�ƾ ����
        //SlowTime.Instance.Slow();
        StopAllCoroutines(); // ���� �ڷ�ƾ ���� (�ߺ� ����)
        //StartCoroutine(ResetTimeScale());
    }

    private IEnumerator ResetTimeScale()
    {
        yield return new WaitForSecondsRealtime(SlowTime.Instance.slowLength);
        SlowTime.Instance.Back();
    }


    //��Ŭ�� ����
    public void OnAbsorb()
    {
        WeaponController.Instance.AbsorbClick();
    }

    //��Ŭ�� ���� ���
    public void OnAbsorbCancle()
    {
        WeaponController.Instance.AbsorbClickUp();
    }

    //��Ŭ�� ����
    public void OnEmit()
    {
        Combination();
        WeaponController.Instance.WeaponSelect();
    }


    //��Ŭ�� ���� ���
    public void OnEmitCancle()
    {
        WeaponController.Instance.WeaponLeft();
    }


    //���� ���
    private void Combination()
    {
        Dictionary<(int, int), string> combinations = new Dictionary<(int, int), string>
        {
            { (1, 1), "water"}, //��+��
            { (2, 2), "treeVine" }, // Ǯ+Ǯ
            { (3, 3), "rockBomb" }, // ����+����
            { (1, 2), "potion" }, // ��+Ǯ
            { (2, 1), "potion" }, // Ǯ+��
            { (2, 3), "platform" }, // Ǯ+����
            { (3, 2), "platform" }, // ����+Ǯ
            { (1, 3), "bullet" }, // ��+����
            { (3, 1), "bullet" }  // ����+��
        };

        //���õ� ����
        var selectedCombination = (select1, select2);

        // ���տ� �ش��ϴ� ��� ���
        if (combinations.TryGetValue(selectedCombination, out string result))
        {
            WeaponController.Instance.WeaponMode = result;
        }
        else
        {
            Debug.Log("��ȿ���� ���� �����Դϴ�.");
        }
    }


    // �̵� �Է� ó��
    public void OnMove(InputValue value)
    {
        if (isDie) return; // isDie�� true��� ���� ��ȯ�� ����

        moveDirection = value.Get<Vector2>(); // �̵� ���� ����

        if (moveDirection.x != 0)
        {
            // �̵� �Է��� �ִ� ��� Walk ���·� ��ȯ
            stateMachine.TransitionTo(stateMachine.walkState);
        }
        else
        {
            // �̵� �Է��� ���� ��� Idle ���·� ��ȯ
            stateMachine.TransitionTo(stateMachine.idleState);
        }


    }


    // ���� �Է� ó��
    public void OnJump(InputValue value)
    {
        if (isDie) return; // isDie�� true��� ���� ��ȯ�� ����

        if (value.isPressed && !isJump)
        {
            isJump = true; // ���� �÷��� ����
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, moveSpeed); // ���� �ӵ� ����
            stateMachine.TransitionTo(stateMachine.jumpState); // ���� ���·� ��ȯ
        }
    }

    // �뽬 �Է� ó��
    public void OnSprint(InputValue value)
    {
        if (value.isPressed && !isDashing && dashCooldownTimer <= 0)
        {
            // ������ ���� ���� �뽬 ������ �����ϱ� ���� flipX �Ǵ� ���콺 ��ġ�� �������� ���� ����
            float dashDirection = moveDirection.x != 0 ? moveDirection.x : (spriteRenderer.flipX ? -1 : 1);

            StartCoroutine(Dash(dashDirection)); // �뽬 ������ �Ű������� ����
            stateMachine.TransitionTo(stateMachine.dashState); // �뽬 ���·� ��ȯ
        }
    }

    // �̵� ���� ó��
    private void ApplyMovement()
    {
        if (grappling.isAttach)
        {
            rb.AddForce(new Vector2(moveDirection.x * moveSpeed, 0));
        }
        else
        {
            Vector2 targetVelocity = new Vector2(moveDirection.x * moveSpeed, rb.linearVelocity.y);
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, 0.1f); // �ε巯�� ��ȭ        }
        }
    }

    // �뽬 ���� ó��
    private System.Collections.IEnumerator Dash(float dashDirection)
    {
        isDashing = true; // �뽬 �÷��� ����
        Vector2 dashVelocity = new Vector2(dashDirection * player.DashSpeed, rb.linearVelocity.y); // �뽬 ���� ����
        // rb.AddForce(dashForce, ForceMode2D.Impulse); // �뽬 �� ����

        Debug.Log($"Dash Velocity: {dashVelocity}");
        rb.linearVelocity = dashVelocity;

        yield return new WaitForSeconds(player.DashDuration); // �뽬 ���� �ð� ���
        isDashing = false; // �뽬 �÷��� ����
        dashCooldownTimer = player.DashCooldown; // �뽬 ��Ÿ�� ����
        stateMachine.TransitionTo(stateMachine.idleState); // Idle ���·� ��ȯ
    }

    // �ٴ� �浹 ����
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDie) return; // isDie�� true��� ���� ��ȯ�� ����
        if (collision.gameObject.CompareTag("ground"))
        {
            isJump = false; // ���� �÷��� ����
            if (moveDirection.x != 0)
            {
                // �̵� �Է��� �ִ� ��� Walk ���·� ��ȯ
                stateMachine.TransitionTo(stateMachine.walkState);
            }
            else
            {
                // �̵� �Է��� ���� ��� Idle ���·� ��ȯ
                stateMachine.TransitionTo(stateMachine.idleState);
            }
        }
        if (collision.gameObject.CompareTag("Item"))
        {
            Debug.Log("Items");
            player.GetEnergyCore(1);
            Destroy(collision.gameObject);
        }
    }

}
