using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun
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
    [SerializeField] private float hiddenOpacity = 0.25f;
    [SerializeField] private KeyCode hideKey = KeyCode.E;

    [Header("Detector")]
    [SerializeField] private Collider2D hideDetector; // trigger child collider

    private Rigidbody2D body;
    private SpriteRenderer sr;

    private bool nearContainer = false;
    public bool IsHidden { get; private set; } = false;

    private int jumpsRemaining;
    private bool isGrounded;

    private RigidbodyConstraints2D defaultConstraints;
    private float defaultGravity;

    private PhotonView pv;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        pv = GetComponent<PhotonView>();

        defaultConstraints = body.constraints;
        defaultGravity = body.gravityScale;

        if (hideDetector == null)
            Debug.LogError("❌ Assign the HideDetector (child trigger collider)!");

        //------------------------------------------------
        // Disable physics for remote players
        //------------------------------------------------
        if (!pv.IsMine)
        {
            body.isKinematic = true;          // remote physics synced automatically by Photon
            hideDetector.enabled = false;     // remote players cannot hide locally
        }
    }

    private void Update()
    {
        if (!pv.IsMine) return; // 🔥 prevent remote input

        HandleHidingInput();

        if (IsHidden)
            return;

        HandleMovement();
        HandleJump();
    }

    // ----------------------------------
    // MOVEMENT
    // ----------------------------------
    private void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");

        FixSideSticking();

        body.linearVelocity = new Vector2(x * speed, body.linearVelocity.y);

        // Flip sprite
        if (x != 0)
            sr.flipX = (x < 0);
    }

    private void FixSideSticking()
    {
        RaycastHit2D leftHit = Physics2D.Raycast(transform.position, Vector2.left, 0.15f, groundLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(transform.position, Vector2.right, 0.15f, groundLayer);

        if (leftHit.collider != null && body.linearVelocity.x < 0)
        {
            body.linearVelocity = new Vector2(-speed, body.linearVelocity.y);
        }
        else if (rightHit.collider != null && body.linearVelocity.x > 0)
        {
            body.linearVelocity = new Vector2(speed, body.linearVelocity.y);
        }
    }

    // ----------------------------------
    // JUMPING
    // ----------------------------------
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
        isGrounded = Physics2D.OverlapCircle(
            groundCheckPoint.position,
            groundCheckRadius,
            groundLayer
        );
    }

    // ----------------------------------
    // HIDING SYSTEM
    // ----------------------------------
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

        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, hiddenOpacity);

        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
        {
            if (col == hideDetector) continue;
            col.enabled = false;
        }

        body.gravityScale = 0f;
        body.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private void Unhide()
    {
        IsHidden = false;

        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);

        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
        {
            if (col == hideDetector) continue;
            col.enabled = true;
        }

        body.gravityScale = defaultGravity;
        body.constraints = defaultConstraints;
    }

    // -----------------------------------------
    // HIDING DETECTION
    // -----------------------------------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!pv.IsMine) return; // remote players shouldn't trigger hiding logic

        if (collision.CompareTag("HideContainer"))
        {
            if (collision.IsTouching(hideDetector))
                nearContainer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!pv.IsMine) return;

        if (collision.CompareTag("HideContainer"))
        {
            if (!collision.IsTouching(hideDetector))
            {
                nearContainer = false;

                if (IsHidden)
                    Unhide();
            }
        }
    }
}
