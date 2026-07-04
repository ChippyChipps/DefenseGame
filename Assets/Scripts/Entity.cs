using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Entity : MonoBehaviour
{
    protected Animator animator;
    protected Rigidbody2D rb;

    [Header("Player Settings")]
    [SerializeField] protected float moveSpeed;
    [SerializeField] private float jumpPower;
    protected float xInput;
    protected int facingDir = 1;
    private bool facingRight = true;
    protected bool canMove = true;
    private bool canJump = true;
    
    [SerializeField] private InputActionReference move;

    [Header("Collision details")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] protected bool isGrounded;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] protected Transform groundCheck;

    [Header("Attack Details")]
    [SerializeField] protected float attackRadius;
    [SerializeField] protected Transform attackPoint;
    [SerializeField] protected LayerMask whatIsTarget;

    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        HandleCollision();
        HandleFlip();
        HandleAnimations();
    }

    public void EnableMovementAndJump(bool enable)
    {
        canJump = enable;
        canMove = enable;
    }

    protected virtual void FixedUpdate()
    {
        Move();
    }

    protected virtual void HandleAnimations()
    {
        animator.SetFloat("xVelocity", rb.linearVelocity.x);
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        animator.SetBool("isGrounded", isGrounded);
    }

    protected virtual void HandleFlip()
    {
        if (rb.linearVelocity.x > 0 && facingRight == false)
            Flip();
        else if (rb.linearVelocity.x < 0 && facingRight == true)
            Flip();
    }

    private void Flip()
    {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
        facingDir = facingDir * -1;
    }

    protected virtual void Move()
    {
        if (!canMove)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }        
        else
            rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocity.y);

    }
    
    public void Jump(InputAction.CallbackContext context)
    {
        if(context.performed && isGrounded && canJump)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
    }

    protected virtual void HandleAttack()
    {
        if(isGrounded)
        {
            animator.SetTrigger("Attack");
        }
    }

    protected virtual void HandleCollision()
    {
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    public void DamageEnemies()
    {
        Collider2D[] enemyColliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, whatIsTarget);

        foreach (Collider2D enemy in enemyColliders)
        {
            //Enemy enemyScript = enemy.GetComponent<Enemy>();
            //enemyScript.TakeDamage();

            //string enemyName = enemyScript.GetEnemyName();
            //Debug.Log("I damange enemy "+enemyName);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -groundCheckDistance));
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}