using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpPower;
    private bool facingRight = true;
    private bool canMove = true;
    private bool canJump = true;
    
    [SerializeField] Rigidbody2D rb;
    [SerializeField] InputActionReference move;
    private Animator animator;

    [Header("Collision details")]
    [SerializeField] float groundCheckDistance;
    [SerializeField] bool isGrounded;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] Transform groundCheck;  


    private float horizontal;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
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

    private void FixedUpdate()
    {
        if (!canMove)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);
    }

    private void HandleAnimations()
    {
        animator.SetFloat("xVelocity", rb.linearVelocity.x);
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        animator.SetBool("isGrounded", isGrounded);
    }

    private void HandleFlip()
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
    }

    public void Move(InputAction.CallbackContext context)
    {
            horizontal = context.ReadValue<Vector2>().x;
    }
    
    public void Jump(InputAction.CallbackContext context)
    {
        if(context.performed && isGrounded && canJump)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if(isGrounded && context.performed)
        {
            animator.SetTrigger("Attack");
        }
    }

    private void HandleCollision()
    {
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    }
}