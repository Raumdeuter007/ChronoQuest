using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class KingMovement : MonoBehaviour
{
    [Header("Player Component References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Player Settings")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private int sprintMultiplier = 2;
    [SerializeField] private float jumpForce = 15f; // Initial jump force
    [SerializeField] private float maxJumpTime = 0.35f; // Maximum time player can hold jump
    [SerializeField] private float jumpCutMultiplier = 0.5f; // How much to cut jump when releasing button
    [SerializeField] private int totalDashFrames = 3;
    [SerializeField] private int dashReset = 15;
    [Header("Physics")]
    [SerializeField] private float fallGravityMultiplier = 2.5f; // Faster falling
    [SerializeField] private float lowJumpGravityMultiplier = 2f; // When releasing jump early
    private float baseGravity;

    [Header("Grounding")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    // Movement variables
    private float horizontal;
    private bool isFacingRight = true, isSprint = false, isDash = false, dashDirectionRight = false;
    private int currDashReset = 0;
    // Jump variables
    private bool isJumpPressed = false;
    private bool isJumping = false;
    private float jumpTimeCounter, dashFrames = 0;

    // Ground state
    private bool wasGrounded;
    private bool isGrounded;

    // Animation hashes
    private int jumpHash = 0, moveHash = 0, fallHash = 0, sprintHash = 0;

    #region Unity Lifecycle
    private void Start()
    {
        // Cache animation parameter hashes
        jumpHash = Animator.StringToHash("InAir");
        moveHash = Animator.StringToHash("Running");
        fallHash = Animator.StringToHash("Falling");
        sprintHash = Animator.StringToHash("Sprint");

        // Store the base gravity scale
        baseGravity = 10f;

        // Delay initial ground check to allow physics to settle
        Invoke(nameof(InitializeGroundState), 0.05f);
    }
    private void InitializeGroundState()
    {
        isGrounded = IsGrounded();
        wasGrounded = isGrounded;
    }

    private void Update()
    {
        // Update ground state
        wasGrounded = isGrounded;
        isGrounded = IsGrounded();

        // Zero Y velocity when grounded
        if (isGrounded && rb.linearVelocity.y <= 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        }

        // Handle landing
        if (!wasGrounded && isGrounded)
        {
            animator.SetBool(jumpHash, false);
            animator.SetBool(fallHash, false);
            isJumping = false;
            jumpTimeCounter = 0f;
        }


        // Handle jump logic
        HandleJump();

        // Apply gravity multipliers for better jump feel
        ApplyManualGravity();

        // Handle sprite flipping
        Flip();
    }

    private void FixedUpdate()
    {
        if (isDash)
            HandleDash();
        // Apply horizontal movement
        else if (isSprint)
            rb.linearVelocity = new Vector2(horizontal * speed * sprintMultiplier, rb.linearVelocity.y);
        else
            rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
        currDashReset = currDashReset == 0 ? 0 : currDashReset - 1;
    }
    #endregion

    #region Player Controls
    public void Move(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetBool(moveHash, true);
        }
        else if (context.canceled)
        {
            animator.SetBool(moveHash, false);
        }
        horizontal = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Jump button pressed
            isJumpPressed = true;

            if (isGrounded && !isJumping)
            {
                StartJump();
            }
        }
        else if (context.canceled)
        {
            // Jump button released
            isJumpPressed = false;

            // Cut jump short if player releases button early
            if (isJumping && rb.linearVelocity.y > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
            }
        }
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            animator.SetBool(sprintHash, true);
            isSprint = true;
        }
        else if (context.canceled)
        {
            animator.SetBool(sprintHash, false);
            isSprint = false;
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!isDash && currDashReset == 0)
            {
                isDash = true;
                dashDirectionRight = isFacingRight;
                dashFrames = totalDashFrames;
            }
        }
    }
    #endregion

    #region Jump Logic
    private void StartJump()
    {
        animator.SetBool(jumpHash, true);
        isJumping = true;
        jumpTimeCounter = maxJumpTime;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void HandleJump()
    {
        if (isJumping && isJumpPressed)
        {
            if (jumpTimeCounter > 0)
            {
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        // Stop jumping if button released or moving downward
        if ((!isJumpPressed || rb.linearVelocity.y <= 0) && isJumping)
        {
            animator.SetBool(fallHash, true);
            isJumping = false;
        }
    }

    private void ApplyManualGravity()
    {
        if (!isGrounded)
        {
            // Apply gravity based on jump state
            float gravityToApply;

            if (rb.linearVelocity.y < 0)
            {
                // Falling
                gravityToApply = baseGravity * fallGravityMultiplier;
            }
            else if (rb.linearVelocity.y > 0 && !isJumpPressed)
            {
                // Rising but not holding jump
                gravityToApply = baseGravity * lowJumpGravityMultiplier;
            }
            else
            {
                // Normal gravity
                gravityToApply = baseGravity;
            }

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y - gravityToApply * Time.deltaTime);
        }
    }
    #endregion

    #region Ground Check
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
    #endregion

    #region Dash Logic
    private void HandleDash()
    {
        if (isDash)
        {
            if (dashDirectionRight)
                rb.linearVelocityX = speed * 10;
            else
                rb.linearVelocityX = speed * -10;
            dashFrames -= 1;
            if (dashFrames == 0)
            {
                isDash = false;
                currDashReset = dashReset;
            }
        }
    }
    #endregion

    #region Sprite Flipping
    private void Flip()
    {
        if ((isFacingRight && horizontal < 0f) || (!isFacingRight && horizontal > 0f))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
            animator.SetBool(moveHash, true);
        }
    }
    #endregion
}