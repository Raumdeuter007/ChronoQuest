using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class KingMovement : MonoBehaviour
{
    [Header("Player Component References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Player Settings")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpPower = 16f;
    [SerializeField] private float gravity = 1f;

    [Header("Grounding")]
    [SerializeField] private Transform layerCheck;
    [SerializeField] private LayerMask groundLayer;
    private float horizontal;
    private bool isFacingRight = true;


    #region Player Controls
    public void Move(InputAction.CallbackContext context)
    {
        Debug.Log("Move Called");
        horizontal = context.ReadValue<Vector2>().x;

    }
    public void Jump(InputAction.CallbackContext _)
    {
        Debug.Log("Jump");
        rb.linearVelocityY = jumpPower * 10;
    }
    #endregion
    private void FixedUpdate()
    {
        Debug.Log("hello");
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocityY);
    }

    private void Update()
    {
        rb.linearVelocityY -= gravity * Time.deltaTime;
    }
    private void Flip()
    {
        if ((isFacingRight && horizontal < 0f) || (!isFacingRight && horizontal > 0f))
        {
            isFacingRight = !isFacingRight;
            //Vector2 localScale =
        }
    }
}
