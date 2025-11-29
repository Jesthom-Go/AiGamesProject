using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Jump Settings")]
    [SerializeField] private int maxJumps = 2; // 2 = double jump

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private Rigidbody2D body;
    private bool isGrounded;
    private int jumpsRemaining;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        jumpsRemaining = maxJumps;
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        body.linearVelocity = new Vector2(horizontal * speed, body.linearVelocity.y);
    }

    private void HandleJump()
    {
        CheckGrounded();

        // Reset jumps when grounded
        if (isGrounded)
        {
            jumpsRemaining = maxJumps;
        }

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space) && jumpsRemaining > 0)
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
            jumpsRemaining--;
        }
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheckPoint.position,
            groundCheckRadius,
            groundLayer
        );
    }
}
