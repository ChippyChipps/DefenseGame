using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class Player : Entity
{
    [Header("Player Only Setting")]
    [SerializeField] private InputActionReference moveInput;
    [SerializeField] private float jumpPower;

    private bool canJump = true;

    protected override void Update()
    {
        base.Update();
    }

    public void Move(InputAction.CallbackContext context)
    {
        xInput = context.ReadValue<Vector2>().x;
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (isGrounded && context.performed)
        {
            animator.SetTrigger("Attack");
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && canJump)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
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
