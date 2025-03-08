using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavigation : MonoBehaviour
{
    #region References
    private Transform target;
    private NavMeshAgent agent;
    private VisionSystem vision;
    #endregion

    [HideInInspector] public bool isMoving; // inspector
    private Coroutine resetTargetCoroutine;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        vision = GetComponent<VisionSystem>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.autoRepath = true;
        agent.stoppingDistance = vision.sightRadiusMult * vision.defaults.sightRadius - 1;
    }

    void Update()
    {
        if (target == null)
        {
            Transform newTarget = vision.GetClosestInSight(new LayerMask[] { vision.defaults.playerLayer })?.transform;

            if (newTarget != null)
            {
                target = newTarget;
                // Start the coroutine to reset target after 5 seconds
                if (resetTargetCoroutine != null)
                    StopCoroutine(resetTargetCoroutine);
                resetTargetCoroutine = StartCoroutine(ResetTargetAfterDelay(2f));
            }
        }

        if (target != null)
        {
            agent.SetDestination(target.position);
        }

        isMoving = agent.desiredVelocity.magnitude > 0;
    }

    IEnumerator ResetTargetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        target = null;
    }

    // Optional: Reset the coroutine when the object is disabled
    void OnDisable()
    {
        if (resetTargetCoroutine != null)
        {
            StopCoroutine(resetTargetCoroutine);
            resetTargetCoroutine = null;
        }
    }
}