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
    [SerializeField]  private float jumpForce = 6;
    // Start is called before the first frame update
    [Header("Collision info")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask whatIsGround;
                     private bool isGrounded;

    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistance;
    // Update is called once per frame
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Space)) {
            JumpButton();
        } 

        FlipControler();
        AnimatorController();
        CollisionCheck();
        if (isGrounded) {
            canDoubleJump = true;
        }
    }

    private void FlipControler()
    {
        movingInput = Input.GetAxis("Horizontal");
        if (rb.velocity.x < 0 && facingRight)
        { Flip(); }
        else if (rb.velocity.x > 0 && !facingRight) { Flip(); }

    }
    private void JumpButton()
    {
        if(isGrounded)
        { Jump(); }
        else if (canDoubleJump)
        {
            canDoubleJump = false;
            Jump();
        }
    }
    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x,jumpForce);
    }


    private void FixedUpdate()
    {

        rb.velocity = new Vector2(movingInput * speed, rb.velocity.y);
    }
    private void Flip()
    {
        transform.Rotate(0,180,0);
        facingRight = !facingRight;
    }
    private void AnimatorController()
    {
        bool IsMoving = rb.velocity.x != 0;
        anim.SetBool("IsMoving", IsMoving);
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
    }
    private void CollisionCheck() {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
