using UnityEngine;
using System;
using UnityEngine.InputSystem;
using Unity.Mathematics;

public class PlayerMovement : MonoBehaviour
{
#region Variables
    public static PlayerMovement instance;

    private Rigidbody2D rb;

    private Vector2 moveInput;
    private Vector3 velocity;

    private InputAction moveAction;
    private InputAction jumpAction;


    [Header("Speed")]
    public float maxSpeed = 20f;
    public float groundAcceleration = 40f;
    public float groundTurningAcceleration = 60f;
    public float airAcceleration = 30f;
    public float airTurningAcceleration = 40f;
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
    private float lastOnWallTime;
    private float lastOnRightWallTime;
    private float lastOnLeftWallTime;

    private bool isJumping;
    private bool isWallJumping;

    [Header("Gravity")]
    public float normalGravityScale = 1f;
    public float fallingGravityScale = 3f;
    public float jumpCutMultiplier = 0.5f;

    [Header("Raycasts")]
    public float groundCheckOffset = 0.4f;
    public float wallCheckOffset = 0.3f;
    public float groundCheckDistance = 2f;
    public float sideRayDistance = 2f;

    [Header("Masks")]
    public LayerMask groundMask;
    public LayerMask wallJumpMask;
#endregion

#region Start & Awake
    void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>();

        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");

    }

    void Start()
    {
        SetGravity(normalGravityScale);
    }
#endregion

    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();

        if (jumpAction.WasPressedThisFrame())
        {
            lastPressedJumpTime = jumpBufferTime;
        }

        HandleTimers();
        HandleJumpingStuff();
        Debugging();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleTimers()
    {
        lastOnGroundTime -= Time.deltaTime;
        lastPressedJumpTime -= Time.deltaTime;
        lastOnLeftWallTime -= Time.deltaTime;
        lastOnRightWallTime -= Time.deltaTime;
        lastOnWallTime -= Time.deltaTime;

        if (!isJumping)
        {
            if (IsGrounded())
            {
                lastOnGroundTime = coyoteTimeAmount;
            }
        }

        if (IsTouchingRightWall())
        {
            lastOnRightWallTime = coyoteTimeAmount;
        }

        if (IsTouchingLeftWall())
        {
            lastOnLeftWallTime = coyoteTimeAmount;
        }

        lastOnWallTime = Mathf.Max(lastOnLeftWallTime, lastOnRightWallTime);
    }

    void HandleJumpingStuff()
    {
        #region Checking If Can Jump
        if (lastPressedJumpTime > 0)
        {
            if (CanJump())
            {
                Jump();
            }
            else if (CanWallJumpLeft())
            {
                WallJump(false);
            }
            else if (CanWallJumpRight())
            {
                WallJump(true);
            }
        }

        if (isJumping && rb.linearVelocity.y < 0)
        {
            isJumping = false;
        }
        #endregion

        #region Jump Gravity Stuff

        if (jumpAction.WasReleasedThisFrame())
        {
            if (CanJumpCut())
            {
                rb.linearVelocityY *= jumpCutMultiplier;
                isJumping = false;
            }
        }
        else if (rb.linearVelocity.y < 0)
        {
            SetGravity(fallingGravityScale);
        }
        else
        {
            SetGravity(normalGravityScale);
        }
        #endregion
    }

    void HandleMovement()
    {
        //Checks if input is negative or positive to go left or right
        float currentSpeed = rb.linearVelocity.x;
        float targetSpeed = moveInput.x * maxSpeed;

        bool isTurning = MathF.Sign(currentSpeed) != MathF.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01;

        float accelRate;

        if (lastOnGroundTime > 0)
        {
            if (Mathf.Abs(targetSpeed) > 0.01)
            {
                accelRate = isTurning ? groundTurningAcceleration : groundAcceleration;
            } else
            {
                accelRate = groundDeceleration;
            }
        } else
        {
            if (Mathf.Abs(targetSpeed) > 0.01)
            {
                accelRate = isTurning ? airTurningAcceleration : airAcceleration;
            }
            else
            {
                accelRate = airDeceleration;
            }
        }

        float finalSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accelRate * Time.fixedDeltaTime);

        if (Mathf.Abs(finalSpeed) < 0.001)
        {
            finalSpeed = 0f;
        }

        rb.linearVelocityX = finalSpeed;
    }

    void SetGravity(float amount)
    {
        rb.gravityScale = amount;
    }

    #region Jumps
    void Jump()
    {
        isJumping = true;

        lastOnGroundTime = 0;
        lastPressedJumpTime = 0;

        rb.linearVelocityY = jumpForce;
    }

    void WallJump(bool isJumpingRight) //True: Right Jump, False: Left Jump
    {
        isJumping = true;

        lastOnGroundTime = 0;
        lastPressedJumpTime = 0;

        lastOnLeftWallTime = 0;
        lastOnRightWallTime = 0;
        lastOnWallTime = 0;

        //Wall Jump To The Left
        if (isJumpingRight == false)
        {
            rb.linearVelocity = new Vector2(-wallJumpHorizontalForce, wallJumpVerticalForce);
        }
        //Wall Jump To The Right
        if (isJumpingRight == true)
        {
            rb.linearVelocity = new Vector2(wallJumpHorizontalForce, wallJumpVerticalForce);
        }
    }
    #endregion

    #region Grounded & Wall Check Bools

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

    bool IsTouchingRightWall()
    {
        float[] verticalOffsets = { -wallCheckOffset, 0f, wallCheckOffset };

        foreach (float offset in verticalOffsets)
        {
            //Checks a raycast to the right of the player
            if (Physics2D.Raycast(
                new Vector2(transform.position.x, transform.position.y + offset),
                Vector2.right,
                sideRayDistance,
                wallJumpMask))
            {
                return true;
            }
        }
        return false;
    }

    bool IsTouchingLeftWall()
    {
        float[] verticalOffsets = { -wallCheckOffset, 0f, wallCheckOffset };

        foreach (float offset in verticalOffsets)
        {
            //Checks a raycast to the right of the player
            if (Physics2D.Raycast(
                new Vector2(transform.position.x, transform.position.y + offset),
                Vector2.left,
                sideRayDistance,
                wallJumpMask))
            {
               return true;
            }
        }
        return false;
    }

    #endregion

    #region Can Jump Bools

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
        return lastOnRightWallTime > 0 && lastOnGroundTime <= 0;
    }

    bool CanWallJumpRight()
    {
        return lastOnLeftWallTime > 0 && lastOnGroundTime <= 0;
    }

    #endregion

    void Debugging()
    {
        //Ground Check Rays
        Debug.DrawRay(new Vector2(transform.position.x - groundCheckOffset, transform.position.y), Vector2.down * groundCheckDistance, Color.red);
        Debug.DrawRay(transform.position, Vector2.down * groundCheckDistance, Color.orange);
        Debug.DrawRay(new Vector2(transform.position.x + groundCheckOffset, transform.position.y), Vector2.down * groundCheckDistance, Color.yellow);
        //Left wall jump rays
        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - wallCheckOffset), Vector2.right * sideRayDistance, Color.blue);
        Debug.DrawRay(transform.position, Vector2.right * sideRayDistance, Color.blue);
        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + wallCheckOffset), Vector2.right * sideRayDistance, Color.blue);
        //Right wall jump rays
        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - wallCheckOffset), Vector2.left * sideRayDistance, Color.green);
        Debug.DrawRay(transform.position, Vector2.left * sideRayDistance, Color.green);
        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + wallCheckOffset), Vector2.left * sideRayDistance, Color.green);

        //Debug.Log(IsTouchingRightWall());
        //Debug.Log("Gravity Scale: " + rb.gravityScale);
        //Debug.Log("Horizontal Velocity: " + rb.linearVelocity.x);
        //Debug.Log("Vertical Velocity: " + rb.linearVelocity.y);

    }

}
