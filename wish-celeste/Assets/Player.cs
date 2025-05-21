using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    [SerializeField] float speed = 7;
    private float movingInput;

    private bool canWallSlide;
    public bool canDoubleJump;
    private bool facingRight = true;
    [SerializeField] private float jumpForce = 13;

    [Header("Collision info")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;

    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistance;

    [Header("Air Dash")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.5f;
    private bool isDashing;
    private bool canDash = true;

    private GameObject[] doubleJumpOrbs;
    private GameObject[] dashOrbs;


    // respawn
    private Vector3 respawnPoint;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        respawnPoint = transform.position;

        doubleJumpOrbs = GameObject.FindGameObjectsWithTag("DoubleJumpReset");
        dashOrbs = GameObject.FindGameObjectsWithTag("DashReset");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            JumpButton();
        }

        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.DownArrow)) && !isGrounded && canDash)
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
        if(movingInput < 0 && facingRight)
        {
            Flip();
        }
        else if(movingInput > 0 && !facingRight)
        {
            Flip();
        }
    }

    private void JumpButton()
    {
        if (isGrounded && isDashing == false)
        {
            Jump();
        }
        else if (canDoubleJump && isDashing == false)
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
        Debug.Log("IsGrounded: " + isGrounded);

}

    private IEnumerator AirDash()
    {
        if(canDash == true)
        {
            canDash = false;
            isDashing = true;
            float originalGravity = rb.gravityScale;
            rb.gravityScale = 0; // Disable gravity during dash

            float dashDirection = facingRight ? 1 : -1;
            rb.velocity = new Vector2(dashDirection * dashSpeed, 0);

            //yield return new WaitForSeconds(dashDuration);

            float dashTimer = 0f;
            while (dashTimer < dashDuration)
            {
                dashTimer += Time.deltaTime;

                if (!isDashing)
                    break;

                yield return null;
            }

            rb.gravityScale = originalGravity; // Restore gravity
            isDashing = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DeathZone"))
        {
            Respawn();
        }

        else if (collision.CompareTag("DoubleJumpReset"))
        {
            canDoubleJump = true;
            collision.gameObject.SetActive(false);
        }

        else if (collision.CompareTag("DashReset"))
        {
            canDash = true;
            collision.gameObject.SetActive(false);
        }

        else if (collision.CompareTag("Coin"))
        {
            collision.gameObject.SetActive(false);
        }
    
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDashing)
        {
            isDashing = false;
            rb.velocity = Vector2.zero;
            rb.gravityScale = 1;
            StopCoroutine(AirDash());
        }
    }

    private void Respawn()
    {
        transform.position = respawnPoint;

        foreach (GameObject orb in doubleJumpOrbs)
        {
            if (orb != null)
                orb.SetActive(true);
        }

        foreach (GameObject orb in dashOrbs)
        {
            if (orb != null)
                orb.SetActive(true);
        }
    }

}