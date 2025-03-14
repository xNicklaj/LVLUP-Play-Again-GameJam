using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VisionSystem : MonoBehaviour
{
    #region References
    public VisionSystemDefaults defaults; // to be set in the Inspector
    #endregion

    #region OverrideVariables
    public float sightRadiusMult = 1.0f;
    public float sightAngleMult = 1.0f;
    public bool debug = false;  // Visualize rays
    public bool isBlind = false;
    private GameObject trackedObject = null;
    public int numberOfRaysOverride = 3;
    public bool seesThroughWalls = false;
    #endregion

    #region DetectionVariables
    private RaycastHit2D hit;
    private bool _seesObject = false;
    public bool SeesObject { get => _seesObject; private set => _seesObject = value; } // not modifiable from outside
    public GameObject TrackedObject { get => trackedObject; private set => trackedObject = value; }
    #endregion

    private void Start()
    {
    }

    private void Update()
    {
        if (debug || defaults.debugEverything)
            DrawFovBoundaries();
    }

    private void drawDebugRays()
    {
        if (SeesObject)
            Debug.DrawLine(transform.position, hit.point, Color.red);
        else if (hit.collider != null)
            Debug.DrawRay(transform.position, (hit.transform.position - transform.position).normalized * hit.distance, Color.green);
    }

    /// <summary>
    /// Detects the closest object within sight, using multiple raycasts to verify visibility.
    /// </summary>
    /// <param name="layers">
    /// An array of LayerMasks representing the layers to check for detectable objects.
    /// </param>
    /// <returns>
    /// The closest GameObject detected within the specified layers and in sight, 
    /// or null if no object is found or visibility is obstructed.
    /// </returns>
    /// <remarks>
    /// This method performs a multi-step detection process:
    /// 1. Checks if the entity is blind
    /// 2. Finds the closest object within a detection circle
    /// 3. Verifies the object is within the field of view
    /// 4. Performs multiple raycasts to check for obstacles
    /// 
    /// Detection can be modified by:
    /// - Blind state
    /// - Field of view
    /// - Ability to see through walls
    /// - Number of rays
    /// - Ray spread angle
    /// </remarks>
    /// <example>
    /// <code>
    /// // Detect closest enemy or item
    /// GameObject target = GetClosestInSight(new LayerMask[] { enemyLayer, itemLayer });
    /// </code>
    /// </example>
    /// <exception cref="System.ArgumentNullException">
    /// Thrown if the input layers array is null.
    /// </exception>
    public GameObject GetClosestInSight(LayerMask[] layers)
    {
        if (isBlind)
            return null;

        GameObject closestObject = GetClosestInCircle(layers);
        if (closestObject == null || !IsInFOV(closestObject))
            return null; // nothing in sight
        else if (seesThroughWalls)
            return closestObject; // is in sight and no need to raycast, as it will see through obstacles

        Vector2 vectorToObject = closestObject.transform.position - transform.position;

        // Combine all the layers into a single bitmask
        LayerMask layerMasks = 0;
        foreach (LayerMask layer in layers) layerMasks |= layer;

        int numberOfRays = numberOfRaysOverride != defaults.numberOfRays ? numberOfRaysOverride : defaults.numberOfRays;
        for (int i = 0; i < numberOfRays; i++)
        {
            // Calculate ray direction with slight spread
            Vector2 rayDirection;
            if (numberOfRays > 1)
            {
                float angleOffset = Mathf.Lerp(-defaults.angleBetweenRays, defaults.angleBetweenRays, (float)i / (numberOfRays - 1));
                rayDirection = Quaternion.Euler(0, 0, angleOffset) * vectorToObject.normalized;
            }
            else
            {
                rayDirection = vectorToObject.normalized;
            }

            // cast the ray
            hit = Physics2D.Raycast(
                transform.position,
                rayDirection,
                vectorToObject.magnitude,
                defaults.obstacleLayer | layerMasks
            );


            // we hit something && it's in the right layer
            SeesObject = (hit.collider != null) && (layerMasks & (1 << hit.collider.gameObject.layer)) != 0;

            if (debug || defaults.debugEverything)
            {
                drawDebugRays();
            }
            if (SeesObject)
                return hit.collider.gameObject; // no need to cast other rays, we already saw the target
        }

        // None of the rays hit the target layer
        return null;
    }

    /// <summary>
    /// Finds the closest GameObject within a circular detection area, filtered by specified layers.
    /// </summary>
    /// <param name="layers">
    /// An array of LayerMasks representing the layers to check for detectable objects.
    /// </param>
    /// <returns>
    /// The closest GameObject within the detection circle that matches the specified layers, 
    /// or null if no matching object is found.
    /// </returns>
    /// <remarks>
    /// This method performs a circular area detection with the following steps:
    /// 1. Combines input layers into a single layer mask
    /// 2. Uses Physics2D.OverlapCircleAll to find colliders in the detection area
    /// 3. Filters colliders by the specified layers
    /// 4. Selects the closest object to the current transform
    /// 
    /// Detection is influenced by:
    /// - Sight radius
    /// - Sight radius multiplier
    /// - Specified layer masks
    /// </remarks>
    /// <example>
    /// <code>
    /// // Find the closest enemy or item within detection radius
    /// GameObject nearestTarget = GetClosestInCircle(new LayerMask[] { enemyLayer, itemLayer });
    /// </code>
    /// </example>
    /// <exception cref="System.ArgumentNullException">
    /// Thrown if the input layers array is null.
    /// </exception>
    /// <seealso cref="Physics2D.OverlapCircleAll"/>
    public GameObject GetClosestInCircle(LayerMask[] layers)
    {
        GameObject closestObject = null;

        // Combine all the layers into a single bitmask
        LayerMask layerMasks = 0;
        foreach (LayerMask layer in layers) layerMasks |= layer;

        // Perform the overlap circle check, considering whether to pass obstacles or not
        Collider2D[] collidersInArea = Physics2D.OverlapCircleAll(
            transform.position,
            defaults.sightRadius * sightRadiusMult,
            layerMasks
        );

        // Loop through all colliders found and check which one is the closest
        float closestDistance = Mathf.Infinity;
        foreach (Collider2D collider in collidersInArea)
        {
            bool isInRightLayer = (layerMasks & (1 << collider.gameObject.layer)) != 0;
            // Debug.Log(isInRightLayer);
            if (!isInRightLayer)
            {
                continue;
            }

            // Calculate the distance to the collider
            float distance = Vector2.Distance(transform.position, collider.transform.position);

            // Update the closest object if this one is closer
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObject = collider.gameObject;
            }
        }

        return closestObject;
    }

    public Vector2 GetValidPointAroundMe(LayerMask mustCollideWith, LayerMask mustNotCollideWith, float radiusMult = 1.0f)
    {
        return GetValidPointAroundPosition(gameObject.transform.position, defaults.sightRadius * sightRadiusMult * radiusMult, mustCollideWith, mustNotCollideWith);
    }

    public static Vector2 GetValidPointAroundPosition(Vector2 origin, float radius, LayerMask mustCollideWith, LayerMask mustNotCollideWith)
    {
        for (int i = 0; i < 3; i++) // 3 max attempts to find a valid point
        {
            // Generate a random point inside the circle
            Vector2 randomOffset = Random.insideUnitCircle * radius;
            Vector2 randomPoint = origin + randomOffset;

            // Check if it collides with the right layer
            Collider2D groundHit = Physics2D.OverlapPoint(randomPoint, mustCollideWith);
            // Check if it does NOT collide with any wrong layer
            Collider2D obstacleHit = Physics2D.OverlapPoint(randomPoint, mustNotCollideWith);

            if (groundHit != null && obstacleHit == null && randomPoint != origin)
            {
                return randomPoint;
            }
        }

        // If no valid point is found, return the origin (or handle it differently)
        Debug.LogWarning("Valid points not found near this origin and within the given radius. Returning the origin.");

        return origin;
    }

    public int CountNearMe(LayerMask[] layers, float radiusMult = 1.0f)
    {
        return CountNearPosition(gameObject.transform.position, sightRadiusMult * defaults.sightRadius * radiusMult, layers);
    }


    public static int CountNearPosition(Vector2 origin, float radius, LayerMask[] layers)
    {
        // Combine all the layers into a single bitmask
        LayerMask layerMasks = 0;
        foreach (LayerMask layer in layers) layerMasks |= layer;

        // Perform the overlap circle check, considering whether to pass obstacles or not
        Collider2D[] collidersInArea = Physics2D.OverlapCircleAll(
            origin,
            radius,
            layerMasks
        );

        return collidersInArea.Count(c => (layerMasks & (1 << c.gameObject.layer)) != 0);
    }

    public static List<Collider2D> FindNearPosition(Vector2 origin, float radius, LayerMask[] layers)
    {
        // Combine all the layers into a single bitmask
        LayerMask layerMasks = 0;
        foreach (LayerMask layer in layers) layerMasks |= layer;

        // Perform the overlap circle check, considering whether to pass obstacles or not
        List<Collider2D> collidersInArea = new List<Collider2D>(Physics2D.OverlapCircleAll(
            origin,
            radius,
            layerMasks
        ));

        return collidersInArea;
    }

    /// <summary>
    /// Determines whether a given GameObject is within the field of view (FOV).
    /// </summary>
    /// <param name="obj">
    /// The GameObject to check for visibility.
    /// </param>
    /// <returns>
    /// True if the object is within the detection radius and sight angle, false otherwise.
    /// </returns>
    /// <remarks>
    /// Visibility is determined by two primary factors:
    /// 1. Distance from the current object
    /// 2. Angle relative to the object's forward direction
    /// 
    /// Calculation considers:
    /// - Sight radius (maximum detection distance)
    /// - Sight angle (maximum detection angle)
    /// - Radius and angle multipliers for dynamic adjustment
    /// </remarks>
    /// <example>
    /// <code>
    /// // Check if an enemy is within field of view
    /// bool isVisible = IsInFOV(enemyGameObject);
    /// </code>
    /// </example>
    /// <exception cref="System.NullReferenceException">
    /// Thrown if the input GameObject is null.
    /// </exception>
    /// <seealso cref="Vector2.Angle"/>
    public bool IsInFOV(GameObject obj)
    {
        float sightAngle = defaults.sightAngle * sightAngleMult;
        float sightRadius = defaults.sightRadius * sightRadiusMult;

        Vector2 vectorToObject = obj.transform.position - transform.position;
        float angleToPlayer = Vector2.Angle(transform.right, vectorToObject.normalized); // Corrected angle calculation

        return !(vectorToObject.magnitude > sightRadius || angleToPlayer > sightAngle * 0.5f);
    }

    private void DrawFovBoundaries()
    {
        float sightAngle = defaults.sightAngle * sightAngleMult;
        float sightRadius = defaults.sightRadius * sightRadiusMult;

        // Draw detection radius
        Debug.DrawLine(transform.position, transform.position + transform.right * sightRadius, Color.yellow);

        // Draw FOV boundaries
        Vector3 leftBoundary = Quaternion.Euler(0, 0, sightAngle * 0.5f) * transform.right;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, -sightAngle * 0.5f) * transform.right;

        Debug.DrawLine(transform.position, transform.position + leftBoundary * sightRadius, Color.yellow);
        Debug.DrawLine(transform.position, transform.position + rightBoundary * sightRadius, Color.yellow);
    }
}
