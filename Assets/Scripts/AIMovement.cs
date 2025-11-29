using UnityEngine;

public class NPC_AI : MonoBehaviour
{
    private enum State { Patrol, Chase }
    private State currentState = State.Patrol;

    [Header("Movement Settings")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float patrolDistance = 3f;

    [Header("Vision Settings")]
    [SerializeField] private float visionRange = 5f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private bool useLineOfSight = true;

    [Header("Vision Gizmo (Scene View Only)")]
    [SerializeField] private bool showVisionGizmo = true;
    [SerializeField] private Color visionGizmoColor = new Color(1f, 0.92f, 0.016f, 0.6f); // default yellow
    [SerializeField] private float gizmoLineThickness = 0.5f; // works best for wire discs

    private Rigidbody2D body;
    private Vector2 startPos;
    private bool movingRight = true;
    private Transform player;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        startPos = transform.position;
    }

    private void Update()
    {
        DetectPlayer();

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;

            case State.Chase:
                ChasePlayer();
                break;
        }
    }

    private void Patrol()
    {
        float speed = movingRight ? patrolSpeed : -patrolSpeed;
        body.linearVelocity = new Vector2(speed, body.linearVelocity.y);

        if (movingRight && transform.position.x >= startPos.x + patrolDistance)
            movingRight = false;
        else if (!movingRight && transform.position.x <= startPos.x - patrolDistance)
            movingRight = true;
    }

    private void ChasePlayer()
    {
        if (player == null)
        {
            currentState = State.Patrol;
            return;
        }

        float direction = Mathf.Sign(player.position.x - transform.position.x);
        body.linearVelocity = new Vector2(direction * chaseSpeed, body.linearVelocity.y);
    }

    private void DetectPlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, visionRange, playerLayer);

        if (hit != null)
        {
            if (useLineOfSight)
            {
                Vector2 direction = (hit.transform.position - transform.position).normalized;
                float distance = Vector2.Distance(transform.position, hit.transform.position);

                RaycastHit2D obstacle = Physics2D.Raycast(transform.position, direction, distance, obstacleLayer);

                if (obstacle.collider != null)
                {
                    currentState = State.Patrol;
                    return;
                }
            }

            player = hit.transform;
            currentState = State.Chase;
        }
        else
        {
            player = null;
            currentState = State.Patrol;
        }
    }

    private void OnDrawGizmos()
    {
        if (!showVisionGizmo) return;

        Gizmos.color = visionGizmoColor;

        // Draw disc manually with thickness
        int segments = 32;
        Vector3 prevPoint = transform.position + new Vector3(visionRange, 0, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = (i / (float)segments) * Mathf.PI * 2;
            Vector3 nextPoint = transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * visionRange;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
}
