using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    [Header("Player Settings")]
    [SerializeField] private float jumpPower;

    [Header("Combo")]
    [SerializeField] private float attackCooldown = .2f;
    [SerializeField] private float comboResetTime = .8f;
    [SerializeField] private int maxCombo = 3;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.5f;

    [Header("Hit Recoil")]
    [SerializeField] private float recoilForce = 3f;
    [SerializeField] private float recoilDuration = 0.08f;

    private bool isRecoiling;

    private bool canJump = true;

    private int comboIndex;

    private float attackCooldownTimer;
    private float comboResetTimer;

    private bool isDashing;
    private bool canDash = true;


    protected override void Update()
    {
        base.Update();

        if (attackCooldownTimer > 0)
            attackCooldownTimer -= Time.deltaTime;

        if (comboResetTimer > 0)
            comboResetTimer -= Time.deltaTime;

        if (comboResetTimer <= 0)
            comboIndex = 0;
    }

    public void InputMovement(InputAction.CallbackContext context)
    {
        xInput = context.ReadValue<Vector2>().x;
    }

    protected override void Move()
    {
        if (isDashing)
        {
            return;
        }

        base.Move();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (!isGrounded)
            return;

        if (!canJump)
            return;

        if (isDashing)
            return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (!isGrounded)
            return;

        if (!canMove)
            return;

        if (isDashing)
            return;

        if (attackCooldownTimer > 0)
            return;

        comboIndex++;

        if (comboIndex > maxCombo)
            comboIndex = 1;

        switch (comboIndex)
        {
            case 1:
                animator.Play("playerAttack");
                break;

            case 2:
                animator.Play("playerAttack2");
                break;

            case 3:
                animator.Play("playerAttack3");
                break;
        }

        attackCooldownTimer = attackCooldown;
        comboResetTimer = comboResetTime;
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (!isGrounded)
            return;

        if (!canDash)
            return;

        if (isDashing)
            return;

        if (!canMove) 
            return;

        StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;

        canMove = false;
        canJump = false;

        animator.SetTrigger("Dash");

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;

        float timer = 0f;
        while (timer < dashDuration)
        {
            rb.linearVelocity = new Vector2(facingDir * dashSpeed, 0);
            timer += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        rb.gravityScale = originalGravity;

        canMove = true;
        canJump = true;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void HitRecoil()
    {
        StartCoroutine(HitRecoilCoroutine());
    }

    private IEnumerator HitRecoilCoroutine()
    {
        isRecoiling = true;
        canMove = false;

        float elapsed = 0f;

        while (elapsed < recoilDuration)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(new Vector2(-facingDir * recoilForce, 0), ForceMode2D.Impulse);

            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        canMove = true;
        isRecoiling = false;
    }

    public override void EnableMovementAndJump(bool enable)
    {
        base.EnableMovementAndJump(enable);
        canJump = enable;
    }

    protected override void Die()
    {
        base.Die();
        UI.instance.EnabledGameOverUI();
    }
}