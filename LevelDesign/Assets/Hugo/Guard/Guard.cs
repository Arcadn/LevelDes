using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.GlobalIllumination;

public class Guard : MonoBehaviour
{
    public enum State
    {
        Roaming,
        ChaseTarget,
        StopChasing,
    }

    private State state;
    public static event System.Action OnGuardHasSpottedPlayer;
    // NavMesh object to move against
    [SerializeField] private Transform movePositionTransform;
    private NavMeshAgent navMeshAgent;
    // Guard Movement Variables
    public float guardSpeed;
    public float guardWaitTime;
    public float guardTurnSpeed;
    private Vector3 guardStartingPosition;
    private float guardReachedStartPosition = 0.1f;

    // Guard Sight Variables
    public Light spotlight;
    public float viewDistance;
    public float viewDistanceChase;
    public float stopChaseDistance;
    private float viewAngle;

    // Guard Spot Variables
    public float playerVisibleTimer;
    public float timeToSpotPlayer;

    public float ChangeDelay = 0.2f;
    public LayerMask viewMask;

    public Transform pathHolder;
    Transform player;
    Color originalSpotlightColor = Color.yellow;
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        guardStartingPosition = transform.position;
        // Guard sight: using the spotlight angle as field of vision.
        viewAngle = spotlight.spotAngle;
        spotlight.color = originalSpotlightColor;
        //Array of how many guard waypoints for movement
        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        // Loop through the waypoints array for movement

        if (playerVisibleTimer <= timeToSpotPlayer)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            for (int i = 0; i < waypoints.Length; i++)
            {
                waypoints[i] = pathHolder.GetChild(i).position;
                // Set the waypoints to the same height as the guard.
                waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
            }
            StartCoroutine(FollowPath(waypoints));
            state = State.Roaming;
        }
    }

    private void Update()
    {
        switch (state)
        {
            default:
            case State.Roaming:

                if (CanSeePlayer())
                {
                    Debug.Log("Guard says: I see you!");
                    playerVisibleTimer += Time.deltaTime;
                }
                else
                {
                    playerVisibleTimer -= Time.deltaTime;
                }
                playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);
                // Fade between idle color to spotted color of spotlight depending on playerVisibleTimer/timeToSpotPlayer;
                spotlight.color = Color.Lerp(originalSpotlightColor, Color.red, playerVisibleTimer / timeToSpotPlayer);

                if (playerVisibleTimer >= timeToSpotPlayer)
                {
                    if (OnGuardHasSpottedPlayer != null)
                    {
                        state = State.ChaseTarget;
                        OnGuardHasSpottedPlayer();
                    }
                }

                break;
            case State.ChaseTarget:
                spotlight.color = Color.red;
                
                StopAllCoroutines();
                

                navMeshAgent.updatePosition = true;
                navMeshAgent.nextPosition = transform.position;
                navMeshAgent.destination = movePositionTransform.position;

                if (Vector3.Distance(transform.position, player.position) > stopChaseDistance)
                {
                    state = State.StopChasing;
                }
                break;
            case State.StopChasing:
                // Guard sight: using the spotlight angle as field of vision.
                viewAngle = spotlight.spotAngle;
                
                if (CanSeePlayer())
                {
                    playerVisibleTimer += Time.deltaTime;
                }
                else
                {
                    playerVisibleTimer -= Time.deltaTime;
                }
                spotlight.color = Color.Lerp(originalSpotlightColor, Color.red, playerVisibleTimer / timeToSpotPlayer);

                navMeshAgent.destination = guardStartingPosition;
                if (Vector3.Distance(transform.position, guardStartingPosition) < guardReachedStartPosition)
                {
                    navMeshAgent.updatePosition = false;

                    Debug.Log("Guard says: I have returned to my start position.");
                    state = State.Roaming;
                    Start();
                }
                break;

        }

    }

    bool CanSeePlayer()
    {
        if (Vector3.Distance(transform.position, player.position) < viewDistance)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleBetweenGuardAndPlayer < viewAngle / 2f)
            {
                if (!Physics.Linecast(transform.position, player.position, viewMask))
                {
                    return true;
                }
            }
        }
        return false;
    }


    //Guard movement with wait function when it enters a new waypoint
    IEnumerator FollowPath(Vector3[] waypoints)
    {
        Debug.Log("Guard says: I am patrolling, like an idiot!");
        transform.position = waypoints[0];

        int guardWaypointIndex = 1;
        Vector3 guardWaypoint = waypoints[guardWaypointIndex];
        transform.LookAt(guardWaypoint);
        // Loop guard movement
        while (true)
        {
            //Guard movement
            transform.position = Vector3.MoveTowards(transform.position, guardWaypoint, guardSpeed * Time.deltaTime);

            // Guard wait then continue to move to next waypoint
            if (transform.position == guardWaypoint)
            {
                guardWaypointIndex = (guardWaypointIndex + 1) % waypoints.Length;
                guardWaypoint = waypoints[guardWaypointIndex];
                // wait for as long as the guardWaitTime
                yield return new WaitForSeconds(guardWaitTime);
                // wait while the guard is turning
                yield return StartCoroutine(TurnToFace(guardWaypoint));
            }
            // wait for 1 frame
            yield return null;
        }
    }

    // Guard turning to waypoint direction
    IEnumerator TurnToFace(Vector3 lookTarget)
    {
        Debug.Log("Guard says: I am turning, super smart!");
        Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.06f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, guardTurnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }


    // Creating (gizmos) invisible sphere?s and lines for guard waypoints
    private void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;

        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, .3f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }
        Gizmos.DrawLine(previousPosition, startPosition);

        // Red line that shows guards view distance
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }

    public void Enable()
    {
        state = State.ChaseTarget;
    }
}
