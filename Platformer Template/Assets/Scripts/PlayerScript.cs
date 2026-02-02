using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
     public static PlayerScript instance;

    private Rigidbody2D rb;

    private Vector2 moveInput;
    private Vector3 velocity;

    private InputAction moveAction;
    private InputAction jumpAction;

    [Header("Speed")]
    public float maxSpeed = 20f;
    public float accelleration = 40f;
    public float decelleration = 20f;

    [Header("Jump")]
    private bool jumpPressed;
    public float jumpForce = 6f;
    private float coyoteTimer;
    public float coyoteTime = 0.5f;
    public float wallJumpHorizontalForce = 20;
    public float wallJumpVerticalForce = 10;
    public float horizontalWallJumpMult = 5f;
    public float verticalWallJumpMult = 5f;

    public LayerMask groundLayer;
    
    [Header("Raycasts")]
    public float groundCheckOffset = 0.4f;
    public float groundCheckDistance = 2f;
    public float sideRayDistance = 2f;

    void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>();
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    void Update()
    {
        HandleInput();
        rayCastDebug();

        if (isGrounded())
        {
            coyoteTimer = coyoteTime;
        } else
        {
            coyoteTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleJump();
    }

    void HandleInput()
    {
        moveInput = moveAction.ReadValue<Vector2>();


        if(jumpAction.WasPressedThisFrame())
        {
            jumpPressed = true;
            Debug.Log("Jump Input");
        }
    }

    void HandleMovement()
    {
        //Checks if input is negative or positive to go left or right
        float targetSpeed = moveInput.x * maxSpeed;

        float accelRate;

        if (MathF.Abs(targetSpeed) > 0.01)
        {
            accelRate = accelleration;
        }
        else
        {
            accelRate = decelleration;
        }

        float finalSpeed = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, accelRate * Time.fixedDeltaTime);

        rb.linearVelocity = new Vector2(finalSpeed, rb.linearVelocity.y);
    }

    void HandleJump()
    {
        if (jumpPressed) 
        { 
            if (coyoteTimer > 0)
            {
                rb.linearVelocityY = jumpForce;
                coyoteTimer = 0;
            } else if (CanWallJumpLeft())
            {
                rb.linearVelocity = new Vector2(
                    -Mathf.Abs(rb.linearVelocity.x * horizontalWallJumpMult) - wallJumpHorizontalForce,
                    (rb.linearVelocity.y * verticalWallJumpMult) + wallJumpVerticalForce);
            } else if (CanWallJumpRight())
            {
                rb.linearVelocity = new Vector2(
                    Mathf.Abs(rb.linearVelocity.x * horizontalWallJumpMult) + wallJumpHorizontalForce,
                    Mathf.Abs(rb.linearVelocity.y * verticalWallJumpMult) + wallJumpVerticalForce);
            }
        }


        jumpPressed = false;
    }

    bool isGrounded()
    {

        float[] horizontalOffsets = {-groundCheckOffset, 0f, groundCheckOffset};

        //Does a raycast on each of the 3 offsets to see if any are hitting the ground
        foreach (float offset in horizontalOffsets) 
        {
            if (Physics2D.Raycast(
                new Vector2(transform.position.x + offset, transform.position.y),
                Vector2.down,
                groundCheckDistance,
                groundLayer))
            {
                return true;
            } 
        }
        return false;
    }

    bool CanWallJumpLeft()
    {
        //Checks a raycast to the right of the player
        if (Physics2D.Raycast(transform.position, Vector2.right, sideRayDistance, LayerMask.GetMask("Wall")))
        {
            return true;
        }
        return false;
    }
 
    bool CanWallJumpRight()
    {
        //Checks a raycast to the left of the player
        if (Physics2D.Raycast(transform.position, Vector2.left, sideRayDistance, LayerMask.GetMask("Wall")))
        {
            return true;
        }
        return false;
    }

    void rayCastDebug()
    {
        //Ground Check Rays
        Debug.DrawRay(new Vector2(transform.position.x - groundCheckOffset, transform.position.y), Vector2.down * groundCheckDistance, Color.red);
        Debug.DrawRay(transform.position, Vector2.down * groundCheckDistance, Color.orange);
        Debug.DrawRay(new Vector2(transform.position.x + groundCheckOffset, transform.position.y), Vector2.down * groundCheckDistance, Color.yellow);
        //Wall jump rays
        Debug.DrawRay(transform.position, Vector2.right * sideRayDistance, Color.blue);
        Debug.DrawRay(transform.position, Vector2.left * sideRayDistance, Color.green);

    }

}

