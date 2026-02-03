using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    private Rigidbody2D rb;

    private Vector2 moveInput;
    private Vector3 velocity;

    [Header("Speed")]
    public float maxSpeed = 20f;
    public float groundAcceleration = 40f;
    public float airAcceleration = 30f;
    public float groundDeceleration = 20f;
    public float airDeceleration = 15f;

    [Header("Jump")]
    public float jumpForce = 6f;
    public float wallJumpHorizontalForce = 20;
    public float wallJumpVerticalForce = 10;
    public float coyoteTimeAmount = 0.2f;
    public float jumpBufferTime = 0.1f;

    private float lastPressedJumpTime;
    private float lastOnGroundTime;

    private bool isJumping;

    [Header("Gravity")]
    public float normalGravityScale = 1f;
    public float fallingGravityScale = 3f;
    public float jumpCutMultiplier = 0.5f;

    [Header("Raycasts")]
    public float groundCheckOffset = 0.4f;
    public float groundCheckDistance = 2f;
    public float sideRayDistance = 2f;

    LayerMask groundMask;
    LayerMask wallJumpMask;
    
    void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>();

        groundMask = LayerMask.GetMask("Ground");
        wallJumpMask = LayerMask.GetMask("Wall", "Ground");
    }

    void Start()
    {
        SetGravity(normalGravityScale);
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        lastOnGroundTime -= Time.deltaTime;
        lastPressedJumpTime -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            lastPressedJumpTime = jumpBufferTime;
        }

        if (!isJumping)
        {
            if (IsGrounded())
            {
                lastOnGroundTime = coyoteTimeAmount;
            }
        }

        if (CanJump() && lastPressedJumpTime > 0)
        {
            Jump();
        }

        if (isJumping && rb.linearVelocity.y < 0)
        {
            isJumping = false;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (CanJumpCut())
            {
                rb.linearVelocityY *= jumpCutMultiplier;
                isJumping = false;
            }
        } else if (rb.linearVelocity.y < 0)
        {
            SetGravity(fallingGravityScale);
        } else
        {
            SetGravity(normalGravityScale);
        }

        RayCastDebug();

    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        //Checks if input is negative or positive to go left or right
        float targetSpeed = moveInput.x * maxSpeed;

        float accelRate;

        if (lastOnGroundTime > 0)
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01) ? groundAcceleration : groundDeceleration;
        } else
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01) ? airAcceleration : airDeceleration;
        }

        float finalSpeed = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, accelRate * Time.fixedDeltaTime);

        rb.linearVelocityX = finalSpeed;
    }


    void SetGravity(float amount)
    {
        rb.gravityScale = amount;
    }

    void Jump()
    {
        isJumping = true;

        lastOnGroundTime = 0;
        lastPressedJumpTime = 0;

        rb.linearVelocityY = jumpForce;
        isJumping = true;

        /*
        else if (CanWallJumpLeft())
        {
            rb.linearVelocity = new Vector2(-wallJumpHorizontalForce, wallJumpVerticalForce);
            isJumping = true;
        }
        else if (CanWallJumpRight())
        {
            rb.linearVelocity = new Vector2(wallJumpHorizontalForce, wallJumpVerticalForce);
            isJumping = true;
        }
        */
    }


    #region Bool functions
    private bool IsGrounded()
    {
        float[] horizontalOffsets = { -groundCheckOffset, 0f, groundCheckOffset };

        //Does a raycast on each of the 3 offsets to see if any are hitting the ground
        foreach (float offset in horizontalOffsets)
        {
            if (Physics2D.Raycast(
                new Vector2(transform.position.x + offset, transform.position.y),
                Vector2.down,
                groundCheckDistance,
                groundMask))
            {
                if (!isJumping)
                {
                    return true;
                } 
            }
        }
        return false;
    }

    private bool CanJump()
    {
        return lastOnGroundTime > 0 && !isJumping;
    }

    private bool CanJumpCut()
    {
        return isJumping && rb.linearVelocity.y > 0;
    }

    bool CanWallJumpLeft()
    {
        //Checks a raycast to the right of the player
        if (Physics2D.Raycast(transform.position, Vector2.right, sideRayDistance, wallJumpMask))
        {
            return true;
        }
        return false;
    }

    bool CanWallJumpRight()
    {
        //Checks a raycast to the left of the player
        if (Physics2D.Raycast(transform.position, Vector2.left, sideRayDistance, wallJumpMask))
        {
            return true;
        }
        return false;
    }
    #endregion

    void RayCastDebug()
    {
        //Ground Check Rays
        Debug.DrawRay(new Vector2(transform.position.x - groundCheckOffset, transform.position.y), Vector2.down * groundCheckDistance, Color.red);
        Debug.DrawRay(transform.position, Vector2.down * groundCheckDistance, Color.orange);
        Debug.DrawRay(new Vector2(transform.position.x + groundCheckOffset, transform.position.y), Vector2.down * groundCheckDistance, Color.yellow);
        //Wall jump rays
        Debug.DrawRay(transform.position, Vector2.right * sideRayDistance, Color.blue);
        Debug.DrawRay(transform.position, Vector2.left * sideRayDistance, Color.green);

        Debug.Log("Gravity Scale: " + rb.gravityScale);
        //Debug.Log("Horizontal Velocity: " + rb.linearVelocity.x);
        //Debug.Log("Vertical Velocity: " + rb.linearVelocity.y);

    }

}
