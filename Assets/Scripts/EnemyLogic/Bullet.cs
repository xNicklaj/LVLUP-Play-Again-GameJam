using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Timeline;

public class Bullet : NetworkBehaviour
{
    #region References
    private VisionSystem vision; // TODO: should be disabled or non-existent when non homing, moreover, only one ray is fine for this. And a reduced radius. Add an override
    public BulletDefaults defaults; // to be set in the Inspector
    public GameObject DustPrefab;
    #endregion

    #region OverrideVariables
    public float maxTraveledDistanceMult = 1.0f;
    public float initialVelocityMult = 1.0f;
    public bool isHoming = false;
    public NetworkVariable<float> damageMult = new NetworkVariable<float>(1.0f);
    public NetworkVariable<bool> isFromEnemy = new NetworkVariable<bool>();

    public Vector2 direction = new Vector2(1, 1); // modifiable for Homing, visible in Inspector for traps
    public Color color = Color.white; // we won't really modify the colors of all bullets together so no need to put this in defaults
    #endregion

    #region PrivateVariables
    private float currentTime = 0.0f;
    #endregion

    #region LagCompensation
    private bool hasHit = false;
    private Vector3 originalScale;
    #endregion



    private void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        Debug.Assert(spriteRenderer != null, "spriteRenderer component missing");

        originalScale = transform.localScale;

        // Add VisionComponent only if the bullet is homing
        if (isHoming)
        {
            vision = gameObject.AddComponent<VisionSystem>();
            vision.defaults = Resources.Load<VisionSystemDefaults>("Scriptables/VisionSystemDefaults");
            Debug.Assert(vision.defaults != null, "Failed to load VisionSystemDefaults from Resources");

            vision.debug = true; // TODO: remove this i guess
            vision.sightRadiusMult = defaults.homingRadiusMult; // override, so bullet has a different FOV than enemies
            vision.numberOfRaysOverride = 1; // one is fine for a bullet, it's stupid


            Debug.Assert(vision != null, "Failed to add VisionSystem component");
            Debug.Assert(defaults != null, "Failed to add VisionSystemDefaults");
        }

        // Set initial color
        spriteRenderer.color = color;
    }

    private void Update()
    {
        if (!IsOwner || !IsServer) return;
        // server only logic from now on

        if (hasHit) // master and slave both enter this, but only once
        {
            Debug.Log("SERVER: received hit confirmation by the client, proceeding to despawn"); // DO NOT REMOVE THIS LINE
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
            return;
        }

        currentTime += Time.deltaTime;

        float initialVelocity = defaults.initialVelocity * initialVelocityMult;
        float maxFlyTime = defaults.maxTraveledDistance * maxTraveledDistanceMult / initialVelocity;
        // Handling: bullet disappears after "timeBeforeDestroy" seconds after it stopped moving.
        //Debug.Log($"{currentTime / maxFlyTime}, maxFlyTime = {maxFlyTime}");
        if (currentTime >= maxFlyTime + defaults.timeBeforeDestroy)
        {
            // Debug.Log("Destroying bullet after Timeout!");
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
            return;
        }
        else if (currentTime >= maxFlyTime)
            return; // just wait for it to disappear




        // if here: still flying! ----------------------

        // Calculate velocity and bullet size based on fly time
        AnimationCurve velocityOverTime = defaults.velocityOverTime;
        AnimationCurve scaleOverTime = defaults.scaleOverTime;

        float velocityFactor = velocityOverTime.Evaluate(currentTime / maxFlyTime); // Normalize input
        float scaleFactor = scaleOverTime.Evaluate(currentTime / maxFlyTime); // Normalize input
        // Debug.Log(velocityFactor);
        float velocity = velocityFactor * initialVelocity;

        // Move bullet
        Vector3 movement = (Vector3)direction.normalized * velocity * Time.deltaTime;
        transform.position += movement;
        // Scale bullet
        transform.localScale = originalScale * scaleFactor;

        // Handling homing behaviour: should follow the player by changing its direction matching the player's position 
        if (isHoming)
        {
            Debug.Assert(vision != null, "VisionSystem seems broken in Update");
            Debug.Assert(defaults != null, "VisionSystemDefaults seems broken in Update");
            // TODO: homing seems broken... YES BECAUSE ALL OF THEM IMMEDIATELY GO TO ME. CHANGE SOMEHOW...
            handleHoming();
        }

        // Debug.Log(currentTime / maxFlyTime);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AskToDespawnServerRpc()
    {
        if (!IsServer)
            return;
        
        Debug.Log("SERVER: hey, got permission to despawn!"); // DO NOT REMOVE THIS LINE

        hasHit = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsServer)
            Debug.Log("SERVER: detected collision, playing effects"); // DO NOT REMOVE THIS LINE
        if (IsClient && !IsServer)
            Debug.Log("CLIENT: detected collision, playing effects"); // DO NOT REMOVE THIS LINE

        PlayHitEffects(); // both do this

        if (IsClient && !IsServer)
        {
            Debug.Log("CLIENT: Granting permission to despawn the bullet"); // DO NOT REMOVE THIS LINE
            AskToDespawnServerRpc(); // grant permission to despawn the bullet only after the client sees it hitting
        }

        //isHoming = false; // so the bullet won't follow the player after it collided with something

        //// Debug.Log("IMPACT!");
        //ContactPoint2D contact = collision.GetContact(0);

        //// Calculate reflection direction
        //Vector2 incomingDirection = direction;
        //Vector2 surfaceNormal = contact.normal;

        //// Reflect direction using vector reflection formula
        //direction = Vector2.Reflect(incomingDirection, surfaceNormal);

        //// Immediately go to the end of the animation curve. The Update will do the rest
        //currentTime = 0.80f * defaults.maxFlyTime * maxFlyTimeMult;
    }

    private void handleHoming()
    {
        LayerMask[] layersToSearchIn = isFromEnemy.Value ? new LayerMask[] { vision.defaults.playerLayer } : new LayerMask[] { vision.defaults.enemyLayer };
        GameObject nearbyTarget = vision.GetClosestInSight(layersToSearchIn);

        if (nearbyTarget != null)
        {
            Vector2 targetDirection = (nearbyTarget.transform.position - transform.position).normalized;
            direction = SmoothSteerTowards(direction, targetDirection);

            color = Color.cyan;
        }
    }

    private Vector2 SmoothSteerTowards(Vector2 currentDir, Vector2 targetDir)
    {
        // Calculate the angle between current and target direction
        float angle = Vector2.SignedAngle(currentDir, targetDir);

        // Limit the turn speed
        float maxTurnThisFrame = defaults.homingTurnSpeed * Time.deltaTime;
        float turnAmount = Mathf.Clamp(angle, -maxTurnThisFrame, maxTurnThisFrame);

        // Rotate the current direction
        Vector2 newDirection = Quaternion.Euler(0, 0, turnAmount) * currentDir;

        return newDirection.normalized;
    }

    private void PlayHitEffects()
    {
        GetComponent<SpriteRenderer>().enabled = false; // hide bullet
        GetComponent<Collider2D>().enabled = false; // disable collider so that it cannot impact on other players after hidden

        // show dust cloud animation (don't sync the spawns - each player sees them at the right time)
        GameObject dust = Instantiate(DustPrefab, transform.position, Quaternion.identity);

        // if (IsServer)
        //     dust.GetComponent<NetworkObject>().Spawn();
    }
}
