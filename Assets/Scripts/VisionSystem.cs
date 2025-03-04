using UnityEditor;
using UnityEngine;

public class VisionSystem : MonoBehaviour
{
    public VisionSystemDefaults defaults; // to be set in the Inspector

    #region References
    private Transform playerTransform;
    #endregion

    #region OverrideVariables
    // TODO: should also override defaults.sightAngle somehow 
    public float sightRadiusMult = 1.0f;
    public bool debug = false;  // Visualize rays
    public bool isBlind = false;
    public bool seesThroughWalls = false;
    #endregion

    #region DetectionVariables
    private bool playerDetected = false;
    #endregion

    private void Start()
    {
        // Find the player by tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        playerDetected = detectPlayer();
        // Debug.Log(playerDetected);
    }

    private bool detectPlayer()
    {
        if (isBlind)
        {
            return false; // What did you expect?
        }

        // First check if player is within detection radius and in field of view (no raycast if not)
        Vector2 vectorToPlayer = playerTransform.position - transform.position;
        float angleToPlayer = Vector2.Angle(transform.right, vectorToPlayer.normalized); // Corrected angle calculation

        if (vectorToPlayer.magnitude > defaults.sightRadius * sightRadiusMult || angleToPlayer > defaults.sightAngle * 0.5f)
        {
            return false;
        }
        else if (seesThroughWalls)
        {
            return true; // no need for raycasts
        }

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            vectorToPlayer.normalized, // direction towards player
            vectorToPlayer.magnitude,  // distance from player
            defaults.obstacleLayer | defaults.playerLayer
        );

        if (defaults.debugEverything || (!defaults.debugEverything && debug))
        {
            Debug.DrawRay(transform.position, vectorToPlayer, Color.green);
            DrawDebugVisuals();
        }

        return (hit.collider != null) && (hit.collider.gameObject.tag == "Player"); // we're seeing something and that's the player!
    }

    private void DrawDebugVisuals()
    {
        // Draw detection radius
        Debug.DrawLine(transform.position, transform.position + transform.right * defaults.sightRadius * sightRadiusMult, Color.yellow);

        // Draw line to player if visible
        if (playerDetected)
            Debug.DrawLine(transform.position, playerTransform.position, Color.red);

        // Draw FOV boundaries
        Vector3 leftBoundary = Quaternion.Euler(0, 0, defaults.sightAngle * 0.5f) * transform.right;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, -defaults.sightAngle * 0.5f) * transform.right;

        Debug.DrawLine(transform.position, transform.position + leftBoundary * defaults.sightRadius * sightRadiusMult, Color.yellow);
        Debug.DrawLine(transform.position, transform.position + rightBoundary * defaults.sightRadius * sightRadiusMult, Color.yellow);
    }
}
