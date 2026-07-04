using UnityEngine;
using UnityEngine.InputSystem;

public class Enemy : Entity
{
    private bool playerDetected;

    protected override void Update()
    {
        xInput = 1;

        base.Update();
        HandleAttack();
    }

    protected override void HandleAttack()
    {
        
        if (playerDetected)
            animator.SetTrigger("attack");
    }

    protected override void Move()
    {
        if (!canMove)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }
        rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocity.y);
    }

    protected override void HandleCollision()
    {
        base.HandleCollision();
        playerDetected = Physics2D.OverlapCircle(attackPoint.position, attackRadius, whatIsTarget);
    }
}
