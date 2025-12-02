using UnityEngine;

public class SimpleNPC : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;

    private bool movingRight = true;

    [Header("Vision Settings")]
    [SerializeField] private float visionRange = 6f;
    [SerializeField] private float visionThickness = 0.2f;
    [SerializeField] private float visionOpacity = 0.4f;
    [SerializeField] private Color visionColor = Color.red;

    [Tooltip("Layers that block vision (walls, objects). Player layer is added automatically.")]
    [SerializeField] private LayerMask obstacleMask;

    private Transform player;
    private PlayerMovement playerScript;
    private int playerLayer;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerScript = player.GetComponent<PlayerMovement>();

        // Auto-detect player layer and include it
        playerLayer = 1 << player.gameObject.layer;
    }

    private void Update()
    {
        // If player is hidden → patrol only
        if (playerScript.IsHidden)
        {
            Patrol();
            return;
        }

        if (CanSeePlayer())
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

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

    private void ChasePlayer()
    {
        float speed = chaseSpeed;

        if (player.position.x > transform.position.x)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            movingRight = true;
        }
        else
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
            movingRight = false;
        }
    }

    private bool CanSeePlayer()
    {
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;
        Vector2 start = transform.position;
        float dist = Vector2.Distance(start, player.position);

        // If player is too far, skip
        if (dist > visionRange) return false;

        // FIRST: Check if player is in front direction (not behind NPC)
        if ((player.position.x - start.x > 0 && !movingRight) ||
            (player.position.x - start.x < 0 && movingRight))
        {
            return false;
        }

        // SECOND: Raycast for obstacles AND detect player layer
        int combinedMask = obstacleMask | playerLayer;

        RaycastHit2D hit = Physics2D.Raycast(start, direction, visionRange, combinedMask);

        if (!hit) return false;

        return hit.collider.CompareTag("Player");
    }

    private void OnDrawGizmosSelected()
    {
        if (leftPoint == null || rightPoint == null) return;

        Color gizmoColor = new Color(visionColor.r, visionColor.g, visionColor.b, visionOpacity);
        Gizmos.color = gizmoColor;

        Vector3 dir = (movingRight ? Vector3.right : Vector3.left);
        Vector3 start = transform.position;

        // Proper thickness-drawn box-like FOV
        float half = visionThickness * 0.5f;

        Vector3 topStart = start + new Vector3(0, half, 0);
        Vector3 bottomStart = start - new Vector3(0, half, 0);

        Gizmos.DrawLine(topStart, topStart + dir * visionRange);
        Gizmos.DrawLine(bottomStart, bottomStart + dir * visionRange);
        Gizmos.DrawLine(topStart, bottomStart);
        Gizmos.DrawLine(topStart + dir * visionRange, bottomStart + dir * visionRange);
    }
}
