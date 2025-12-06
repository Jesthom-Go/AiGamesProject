using UnityEngine;

public class SimpleNPC : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;
    private bool movingRight = true;

    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed = 4f;

    [Header("Vision Settings")]
    [SerializeField] private float visionRange = 6f;
    [SerializeField] private float visionFOV = 60f;
    [SerializeField] private LayerMask visionMask;
    [SerializeField] private string wallTag = "Wall";

    [Header("Color Feedback")]
    [SerializeField] private Color patrolColor = Color.green;
    [SerializeField] private Color alertColor = Color.red;

    private SpriteRenderer sr;

    private Transform player;
    private PlayerMovement playerScript;
    private bool seeingPlayer = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerScript = player.GetComponent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();

        // Set default color
        if (sr) sr.color = patrolColor;
    }

    private void Update()
    {
        if (!player || !playerScript) return;

        bool canSee = !playerScript.IsHidden && CanSeePlayer();

        if (canSee)
        {
            sr.color = alertColor;
            ChasePlayer();
        }
        else
        {
            sr.color = patrolColor;
            Patrol();
        }
    }

    // --------------------------
    // PATROL LOGIC
    // --------------------------
    private void Patrol()
    {
        float speed = patrolSpeed;

        if (movingRight)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);

            if (transform.position.x >= rightPoint.position.x)
                movingRight = false;
        }
        else
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);

            if (transform.position.x <= leftPoint.position.x)
                movingRight = true;
        }
    }

    // --------------------------
    // CHASE LOGIC
    // --------------------------
    private void ChasePlayer()
    {
        float speed = chaseSpeed;

        // Move toward player
        if (player.position.x > transform.position.x)
        {
            movingRight = true;
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
        else
        {
            movingRight = false;
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }
    }

    // --------------------------
    // VISION SYSTEM
    // --------------------------
    private bool CanSeePlayer()
    {
        Vector2 origin = transform.position;
        Vector2 dirToPlayer = (player.position - transform.position).normalized;

        // STEP 1 — RANGE CHECK
        if (Vector2.Distance(origin, player.position) > visionRange)
            return false;

        // STEP 2 — FOV CHECK
        Vector2 facing = movingRight ? Vector2.right : Vector2.left;
        float angle = Vector2.Angle(facing, dirToPlayer);

        if (angle > visionFOV / 2f)
            return false;

        // STEP 3 — RAYCAST FOR LINE OF SIGHT
        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            dirToPlayer,
            visionRange,
            visionMask
        );

        Debug.DrawLine(origin, origin + dirToPlayer * visionRange, Color.yellow);

        if (!hit)
            return false;

        // If wall hit → cannot see
        if (hit.collider.CompareTag(wallTag))
            return false;

        // Must hit player first
        return hit.collider.CompareTag("Player");
    }
}
