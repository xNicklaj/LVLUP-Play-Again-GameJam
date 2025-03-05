using UnityEngine;
using UnityEngine.Timeline;

public class Bullet : MonoBehaviour
{
    #region References
    private VisionSystem vision; // TODO: should be disabled or non-existent when non homing, moreover, only one ray is fine for this. And a reduced radius. Add an override
    public BulletDefaults defaults; // to be set in the Inspector
    #endregion

    #region OverrideVariables
    public float maxFlyTimeMult = 1.0f;
    public float initialVelocityMult = 1.0f;
    public bool isHoming = false;
    public bool isFromEnemy = true;
    public Vector2 direction = new Vector2(1, 1); // modifiable for Homing, visible in Inspector for traps
    public Color color = Color.white; // we won't really modify the colors of all bullets together so no need to put this in defaults
    #endregion

    #region PrivateVariables
    private float currentTime = 0.0f;
    #endregion

    private void Awake()
    {
        vision = GetComponent<VisionSystem>();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        Debug.Assert(vision != null, "vision component missing");
        Debug.Assert(spriteRenderer != null, "spriteRenderer component missing");

        // Set initial color
        spriteRenderer.color = color;
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        AnimationCurve velocityOverTime = defaults.velocityOverTime;
        float maxFlyTime = defaults.maxFlyTime * maxFlyTimeMult;
        float initialVelocity = defaults.initialVelocity * initialVelocityMult;

        // Handling homing behaviour: should follow the player by changing its direction matching the player's position 
        if (isHoming)
            // TODO: homing seems broken... YES BECAUSE ALL OF THEM IMMEDIATELY GO TO ME. CHANGE SOMEHOW...
            handleHoming();

        // Handling: bullet disappears after "timeBeforeDestroy" seconds after it stopped moving.
        if (currentTime >= maxFlyTime + defaults.timeBeforeDestroy)
        {
            // Debug.Log("Destroying bullet after Timeout!");
            Destroy(gameObject);
            return;
        }
        else if (currentTime >= maxFlyTime)
            return; // just wait for it to disappear




        // if here: still flying! ----------------------

        // Calculate velocity based on fly time
        float velocityFactor = velocityOverTime.Evaluate(currentTime / maxFlyTime); // Normalize input
        // Debug.Log(velocityFactor);
        float velocity = velocityFactor * initialVelocity;

        // Move bullet
        Vector3 movement = (Vector3)direction.normalized * velocity * Time.deltaTime;
        transform.position += movement;

        // Debug.Log(currentTime / maxFlyTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Destroy(gameObject);
            return;
        }

        // Debug.Log("IMPACT!");
        ContactPoint2D contact = collision.GetContact(0);

        // Calculate reflection direction
        Vector2 incomingDirection = direction;
        Vector2 surfaceNormal = contact.normal;

        // Reflect direction using vector reflection formula
        direction = Vector2.Reflect(incomingDirection, surfaceNormal);

        // Immediately go to the end of the animation curve. The Update will do the rest
        currentTime = 0.80f * defaults.maxFlyTime * maxFlyTimeMult;
    }

    private void handleHoming()
    {
        // TODO: generalize for player. Also seems broken per se (SEE ABOVE)
        GameObject playerNearby = vision.GetClosestInSight(new LayerMask[] { vision.defaults.playerLayer });
        if (playerNearby != null)
        {
            Vector2 targetDirection = (playerNearby.transform.position - transform.position).normalized;
            direction = SmoothSteerTowards(direction, targetDirection);
            direction = playerNearby.transform.position - transform.position;
            color = Color.cyan;
        }
    }

    private Vector2 SmoothSteerTowards(Vector2 currentDir, Vector2 targetDir)
    {
        // Calculate the angle between current and target direction
        float angle = Vector2.SignedAngle(currentDir, targetDir);

        // Limit the turn speed
        float maxTurnThisFrame = 120f * Time.deltaTime;
        float turnAmount = Mathf.Clamp(angle, -maxTurnThisFrame, maxTurnThisFrame);

        // Rotate the current direction
        Vector2 newDirection = Quaternion.Euler(0, 0, turnAmount) * currentDir;

        return newDirection.normalized;
    }
}
