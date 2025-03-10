using System.Collections;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NetworkAnimator))]
public class EnemyNavigation : MonoBehaviour
{
    #region References
    private Transform target;
    private NavMeshAgent agent;
    private VisionSystem vision;
    #endregion

    [HideInInspector] public bool isMoving = false; // inspector
    public bool patrols = true;
    [Min(0f)]
    public float resetTargetAfterLosingSightDelay = 3.0f; // inspector
    [Min(0f)]
    public float randomDirectionWhilePatrollingDelay = 5.0f; // inspector
    [Min(0f)]
    public float waitBeforeStartPatrolling = 5.0f; // inspector
    private Coroutine resetTargetCoroutine;
    private Coroutine patrolCoroutine;

    private Animator _animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        vision = GetComponent<VisionSystem>();
        _animator = GetComponent<Animator>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.autoRepath = true;
        agent.stoppingDistance = vision.sightRadiusMult * vision.defaults.sightRadius - 1;
    }

    private void OnValidate()
    {
        /* this is so the while(true) in the patrolling coroutine breaks out and modifications in the inspector while in play mode actually have effect */
        if (Application.isPlaying && patrolCoroutine != null)
        {
            StopCoroutine(patrolCoroutine);
            patrolCoroutine = StartCoroutine(RandomDestinationAfterDelay(waitBeforeStartPatrolling, randomDirectionWhilePatrollingDelay));
        }
    }

    void Update()
    {

        if (target == null) // Enemy does not know where to go
        {
            // Try to find a player to chase
            Transform newTarget = vision.GetClosestInSight(new LayerMask[] { vision.defaults.playerLayer })?.transform;
            if (newTarget != null)
            {
                // We found a player: let's chaes them
                agent.stoppingDistance = agent.stoppingDistance = vision.sightRadiusMult * vision.defaults.sightRadius - 1; // to reset what patrol routine does

                target = newTarget;

                UpdateAnimator((target.transform.position - transform.position).normalized.x, (target.transform.position - transform.position).normalized.y, agent.velocity.magnitude);

                // Debounce the target reset since we found someone to chase
                if (resetTargetCoroutine != null)
                    StopCoroutine(resetTargetCoroutine);
                resetTargetCoroutine = StartCoroutine(ResetTargetAfterDelay(resetTargetAfterLosingSightDelay));

                // Cancel patrol if a target is found
                if (patrols && patrolCoroutine != null)
                {
                    StopCoroutine(patrolCoroutine);
                    patrolCoroutine = null;
                }
            }
            else if (patrols && patrolCoroutine == null) // Only start patrolling if not already patrolling
            {
                Debug.Log("Starting patrol");
                agent.stoppingDistance = 0f; // otherwise taking random points near the enemy sometimes is not resulting in a movement
                patrolCoroutine = StartCoroutine(RandomDestinationAfterDelay(waitBeforeStartPatrolling, randomDirectionWhilePatrollingDelay));
            }
            UpdateAnimator(agent.velocity.normalized.x, agent.velocity.normalized.z, agent.velocity.magnitude);
        }

        if (target != null)
            agent.SetDestination(target.position);

        isMoving = agent.remainingDistance > agent.stoppingDistance;
    }

    IEnumerator RandomDestinationAfterDelay(float delayBefore, float delayWhile)
    {
        // if enemy must patrol, wait some seconds befort starting to do it
        yield return new WaitForSeconds(delayBefore);

        while (true) // Continuous patrol from now on
        {
            // get a random point, not on obstacles
            Vector2 v = vision.GetValidPointAroundMe(mustCollideWith: vision.defaults.groundLayer, mustNotCollideWith: vision.defaults.obstacleLayer);
            // go there
            agent.SetDestination(v);
            Debug.Log("Setting patrol destination: " + v);

            // Wait until agent reaches destination or gets close to it
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }

            // Now wait for the delay between movements
            yield return new WaitForSeconds(delayWhile);
        }
    }

    IEnumerator ResetTargetAfterDelay(float delay)
    {
        // So that even if we lose sight (target becomes null), enemy still chases the last known position for some seconds

        yield return new WaitForSeconds(delay);
        target = null;
    }

    // Reset the coroutine when the object is disabled (never happens technically but ok)
    void OnDisable()
    {
        if (resetTargetCoroutine != null)
        {
            StopCoroutine(resetTargetCoroutine);
            resetTargetCoroutine = null;
        }
        if (patrolCoroutine != null)
        {
            StopCoroutine(patrolCoroutine);
            patrolCoroutine = null;
        }
    }

    void UpdateAnimator(float horizontal, float vertical, float speed)
    {
        _animator.SetFloat("Horizontal", horizontal);
        _animator.SetFloat("Vertical", vertical);
        _animator.SetFloat("Speed", speed);
    }
}