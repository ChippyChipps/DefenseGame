using UnityEngine;
using UnityEngine.InputSystem;

public class Enemy : Entity
{
    private bool playerDetected;

    protected override void Update()
    {
        base.Update();
        HandleAttack();
    }

    protected override void HandleAttack()
    {
        
        if (playerDetected)
            animator.SetTrigger("attack");
    }

    public void SetDirection(float direction)
    {
        xInput = direction;
    }

    protected override void Move()
    {
        if (!canMove)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }
        else
            rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocity.y);
    }

    protected override void HandleCollision()
    {
        base.HandleCollision();
        playerDetected = Physics2D.OverlapCircle(attackPoint.position, attackRadius, whatIsTarget);
    }

    protected override void Die()
    {
        base.Die();
        UI.instance.AddKillCount();
    }
}
