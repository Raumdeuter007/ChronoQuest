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
    [SerializeField] private float sprintMultiplier = 1.6f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float maxJumpTime = 0.35f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;
    [SerializeField] private int totalDashFrames = 3;
    [SerializeField] private int dashReset = 15;

    [Header("Physics")]
    [SerializeField] private float fallGravityMultiplier = 2.5f;
    [SerializeField] private float lowJumpGravityMultiplier = 2f;
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
    private int jumpHash, moveHash, fallHash, sprintHash, hitTriggerHash;

    public float knockBackForce;
    public float knockBackCounter;
    public float knockBackTotalTime;
    public bool knockFromRight;

    // Internal flag to avoid multiple triggers
    private bool hasTriggeredHit = false;

    #region Unity Lifecycle
    private void Start()
    {
        // Cache animation parameter hashes
        jumpHash = Animator.StringToHash("InAir");
        moveHash = Animator.StringToHash("Running");
        fallHash = Animator.StringToHash("Falling");
        sprintHash = Animator.StringToHash("Sprint");
        hitTriggerHash = Animator.StringToHash("Hit"); // Changed to Trigger

        baseGravity = 10f;
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

        if (isGrounded && rb.linearVelocity.y <= 0 && knockBackCounter <= 0)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);

        // Handle landing
        if (!wasGrounded && isGrounded && knockBackCounter <= 0)
        {
            animator.SetBool(jumpHash, false);
            animator.SetBool(fallHash, false);
            isJumping = false;
            jumpTimeCounter = 0f;
        }

        HandleJump();
        ApplyManualGravity();
        Flip();
    }

    private void FixedUpdate()
    {
        if (isDash)
        {
            HandleDash();
            return;
        }

        float currentSpeed = speed;
        if (isSprint && isGrounded)
            currentSpeed *= sprintMultiplier;

        // Handle knockback
        if (knockBackCounter > 0)
        {
            knockBackCounter -= Time.deltaTime;

            if (!hasTriggeredHit && animator != null)
            {
                animator.SetTrigger(hitTriggerHash); // Trigger only once
                hasTriggeredHit = true;
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(horizontal * currentSpeed, rb.linearVelocity.y);
            hasTriggeredHit = false; // Reset for next knockback
        }

        currDashReset = Mathf.Max(currDashReset - 1, 0);
    }
    #endregion

    #region Player Controls
    public void Move(InputAction.CallbackContext context)
    {
        if (context.started)
            animator.SetBool(moveHash, true);
        else if (context.canceled)
            animator.SetBool(moveHash, false);

        horizontal = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isJumpPressed = true;
            if (isGrounded && !isJumping) StartJump();
        }
        else if (context.canceled)
        {
            isJumpPressed = false;
            if (isJumping && rb.linearVelocity.y > 0f)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
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
        if (knockBackCounter > 0) return;
        if (context.performed && !isDash && currDashReset == 0)
        {
            isDash = true;
            dashDirectionRight = isFacingRight;
            dashFrames = totalDashFrames;
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
                jumpTimeCounter -= Time.deltaTime;
            else
                isJumping = false;
        }

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
            if (knockBackCounter > 0) return;

            float gravityToApply;
            if (rb.linearVelocity.y < 0)
                gravityToApply = baseGravity * fallGravityMultiplier;
            else if (rb.linearVelocity.y > 0 && !isJumpPressed)
                gravityToApply = baseGravity * lowJumpGravityMultiplier;
            else
                gravityToApply = baseGravity;

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
        if (!isDash) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);

        if (dashDirectionRight)
            rb.linearVelocity = new Vector2(speed * 10, rb.linearVelocity.y);
        else
            rb.linearVelocity = new Vector2(-speed * 10, rb.linearVelocity.y);

        dashFrames -= 1;
        if (dashFrames <= 0)
        {
            isDash = false;
            currDashReset = dashReset;
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
        }
    }
    #endregion

}
