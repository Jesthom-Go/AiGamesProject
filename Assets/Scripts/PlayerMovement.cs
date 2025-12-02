using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Jump Settings")]
    [SerializeField] private int maxJumps = 2;

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Hiding Settings")]
    [SerializeField] private float hiddenOpacity = 0.3f;
    [SerializeField] private KeyCode hideKey = KeyCode.E;

    [Header("Hiding Detection")]
    [SerializeField] private Collider2D hideDetector; // dedicated child collider for container detection

    private Rigidbody2D body;
    private SpriteRenderer sr;

    private bool isGrounded;
    private int jumpsRemaining;

    public bool IsHidden { get; private set; } = false;

    // Track nearby container
    private bool nearContainer = false;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();

        if (sr == null)
            Debug.LogError("No SpriteRenderer found on player or children!");

        if (hideDetector == null)
            Debug.LogError("HideDetector is not assigned! Assign a child collider for detection.");
    }

    private void Update()
    {
        HandleHidingInput();

        if (IsHidden)
            return;

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

        if (isGrounded)
            jumpsRemaining = maxJumps;

        if (Input.GetKeyDown(KeyCode.Space) && jumpsRemaining > 0)
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
            jumpsRemaining--;
        }
    }

    private void CheckGrounded()
    {
        if (groundCheckPoint == null)
        {
            isGrounded = false;
            return;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
    }

    // ----------------------------
    // HIDING SYSTEM
    // ----------------------------
    private void HandleHidingInput()
    {
        if (!nearContainer)
            return;

        if (Input.GetKeyDown(hideKey))
        {
            if (!IsHidden)
                Hide();
            else
                Unhide();
        }
    }

    private void Hide()
    {
        IsHidden = true;
        body.linearVelocity = Vector2.zero;

        // Fade sprite
        if (sr != null)
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, hiddenOpacity);

        // Disable all player colliders except the hide detector
        Collider2D[] allCols = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in allCols)
        {
            if (col != hideDetector && !col.isTrigger)
                col.enabled = false;
        }
    }

    private void Unhide()
    {
        IsHidden = false;

        if (sr != null)
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);

        Collider2D[] allCols = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in allCols)
        {
            if (col != hideDetector && !col.isTrigger)
                col.enabled = true;
        }
    }

    // ----------------------------
    // Detect containers using dedicated hideDetector
    // ----------------------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HideContainer") && collision == hideDetector)
        {
            Debug.Log("Near");
            nearContainer = true;
        }
        else if (collision.CompareTag("HideContainer"))
        {
            nearContainer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("HideContainer"))
        {
            nearContainer = false;

            if (IsHidden)
                Unhide();
        }
    }
}
