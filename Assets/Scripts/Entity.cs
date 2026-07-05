using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Entity : MonoBehaviour
{
    protected Animator animator;
    protected Rigidbody2D rb;
    protected Collider2D col;
    protected SpriteRenderer sr;

    [Header("movement")]
    protected float xInput;
    [SerializeField] protected float moveSpeed;

    [Header("Health")]
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private int currentHealth;

    [Header("Attack Details")]
    [SerializeField] protected float attackRadius;
    [SerializeField] protected Transform attackPoint;
    [SerializeField] protected LayerMask whatIsTarget;

    [Header("Damage Feedback")]
    [SerializeField] private Material damageMaterial;
    [SerializeField] float damageFeedbackDuration = 0.2f;
    [SerializeField] protected float knockbackForceX = 8f;
    [SerializeField] protected float knockbackForceY = 6f;
    [SerializeField] protected float knockbackDuration = 0.2f;

    [Header("Collision details")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] protected bool isGrounded;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] protected Transform groundCheck;

    protected int facingDir = 1;
    protected bool facingRight = true;
    protected bool canMove = true;

    private Coroutine damageFeedbackCor;
    private Coroutine knockbackCor;

    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;
    }

    protected virtual void Update()
    {
        HandleCollision();
        HandleFlip();
        HandleAnimations();
    }

    protected virtual void FixedUpdate()
    {
        Move();
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

    public virtual void EnableMovementAndJump(bool enable)
    {
        canMove = enable;
    }

    protected virtual void HandleCollision()
    {
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
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

    public void Flip()
    {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
        facingDir = facingDir * -1;
    }

    protected virtual void HandleAttack()
    {
        if (isGrounded)
        {
            animator.SetTrigger("Attack");
        }
    }

    public void DamageTargets()
    {
        Collider2D[] enemyColliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, whatIsTarget);

        foreach (Collider2D enemy in enemyColliders)
        {
            Entity entityTarget = enemy.GetComponent<Entity>();

            if(entityTarget != null)
                entityTarget.TakeDamage(1, transform);
        }
    }

    public virtual void TakeDamage(int damage, Transform attacker)
    {
        currentHealth -= damage;

        PlayDamagedFeedback();

        ApplyKnockback(attacker);

        if (currentHealth <= 0)
            Die();
    }

    private void PlayDamagedFeedback()
    {
        if (damageFeedbackCor != null)
            StopCoroutine(damageFeedbackCor);

        damageFeedbackCor = StartCoroutine(DamageFeedbackCoroutine());
    }

    private IEnumerator DamageFeedbackCoroutine()
    {
        Material originalMaterial = sr.material;

        sr.material = damageMaterial;

        yield return new WaitForSeconds(damageFeedbackDuration);

        sr.material = originalMaterial;
    }


    protected virtual void ApplyKnockback(Transform attacker)
    {
        if (knockbackCor != null)
            StopCoroutine(knockbackCor);

        knockbackCor = StartCoroutine (KnockbackCoroutine(attacker));
    }

    private IEnumerator KnockbackCoroutine(Transform attacker)
    {
        canMove = false;

        float direction = transform.position.x > attacker.position.x ? 1 : -1;

        rb.linearVelocity = Vector2.zero;

        rb.AddForce(new Vector2(direction * knockbackForceX, knockbackForceY), ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        canMove = true;
    }

    protected virtual void Die()
    {
        animator.enabled = false;
        col.enabled = false;

        rb.gravityScale = 12;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 15);

        Destroy(gameObject, 3);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -groundCheckDistance));

        if(attackPoint != null)
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}