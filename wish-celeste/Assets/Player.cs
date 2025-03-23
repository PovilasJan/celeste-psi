using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    [SerializeField] float speed = 5;
    private float movingInput;

    private bool canWallSlide;
    public bool canDoubleJump;
    private bool facingRight = true;
    [SerializeField] private float jumpForce = 6;

    [Header("Collision info")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;

    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistance;

    [Header("Air Dash")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    private bool isDashing;
    private bool canDash = true;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpButton();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isGrounded && canDash)
        {
            StartCoroutine(AirDash());
        }

        FlipControler();
        AnimatorController();
        CollisionCheck();

        if (isGrounded)
        {
            canDoubleJump = true;
            canDash = true; // Reset dash ability when grounded
        }
    }

    private void FlipControler()
    {
        movingInput = Input.GetAxis("Horizontal");
        if (rb.velocity.x < 0 && facingRight)
        {
            Flip();
        }
        else if (rb.velocity.x > 0 && !facingRight)
        {
            Flip();
        }
    }

    private void JumpButton()
    {
        if (isGrounded)
        {
            Jump();
        }
        else if (canDoubleJump)
        {
            canDoubleJump = false;
            Jump();
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void FixedUpdate()
    {
        if (!isDashing) // Only allow normal movement if not dashing
        {
            rb.velocity = new Vector2(movingInput * speed, rb.velocity.y);
        }
    }

    private void Flip()
    {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
    }

    private void AnimatorController()
    {
        bool IsMoving = rb.velocity.x != 0;
        anim.SetBool("IsMoving", IsMoving);
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
    }

    private void CollisionCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }

    private IEnumerator AirDash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0; // Disable gravity during dash

        float dashDirection = facingRight ? 1 : -1;
        rb.velocity = new Vector2(dashDirection * dashSpeed, 0);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity; // Restore gravity
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true; // Allow dash again after cooldown
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}