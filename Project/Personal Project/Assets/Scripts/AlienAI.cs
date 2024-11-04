using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AlienAIstate
{
    idle, // Alien is idle, waiting for a certain amount of time
    patrol, // Alien is patrolling between waypoints
    chase, // Alien is chasing the target
    attack // Alien is attacking the target
}

public class AlienAI : MonoBehaviour
{
    // Public variables
    public float idleTime = 1.0f; // Time to stay idle before transitioning to patrol
    public NavMeshAgent aiNavMeshAgent; // Reference to the NavMeshAgent component for movement
    public List<Transform> waypoints = new List<Transform>(); // List of waypoints for patrolling
    public Transform target; // Reference to the target transform which is the player
    public float chaseDistance = 25.0f; // Distance within which the alien starts chasing the target
    public float attackDistance = 15.0f; // Distance within which the alien starts attacking the target
    public GameObject laserPrefab; // Prefab for the laser

    // Private variables
    private AlienAIstate _currentState = AlienAIstate.idle; // Current state of the alien
    private int _currentWaypointIndex = -1; // Index of the current waypoint for patrolling
    private float _idleTimer = 0.0f; // Timer to keep track of idle time
    private float _laserSpeed = 50.0f; // Speed of the laser projectile
    private float _fireTimer; // Timer to manage firing intervals in attack state
    private float _fireInterval = 1.0f; // Time interval between laser shots

    // Start is called before the first frame update
    private void Start()
    {
        aiNavMeshAgent = GetComponent<NavMeshAgent>(); // Gets the NavMeshAgent component attached to the alien
        _fireTimer = _fireInterval; // Initialises the fire timer
    }

    // Update is called once per frame
    private void Update()
    {
        // Executes behaviour based on the current state
        switch (_currentState)
        {
            case AlienAIstate.idle:
                HandleIdleState(); // Handles behaviour when the alien is idle
                break;
            case AlienAIstate.patrol:
                HandlePatrolState(); // Handles behaviour when the alien is patrolling
                break;
            case AlienAIstate.chase:
                HandleChaseState(); // Handles behaviour when the alien is chasing the target
                break;
            case AlienAIstate.attack:
                HandleAttackState(); // Handles behaviour when the alien is attacking the target
                break;
        }
    }

    // Handles the idle state behaviour
    private void HandleIdleState()
    {
        // Increments the idle timer
        _idleTimer += Time.deltaTime;
        // Transitions to patrol state after idle time has elapsed
        if (_idleTimer >= idleTime)
        {
            _currentState = AlienAIstate.patrol; // Switches to patrol state
            _idleTimer = 0.0f; // Resets the idle timer
        }
    }

    // Handles the patrol state behaviour
    private void HandlePatrolState()
    {
        // If the alien is not currently moving or if it has not been assigned a waypoint
        if ((aiNavMeshAgent.pathStatus == NavMeshPathStatus.PathComplete && aiNavMeshAgent.remainingDistance < 0.5f) ||
            _currentWaypointIndex == -1)
        {
            // Moves to a random waypoint in the list (loops back to the beginning if at the end)
            int randomIndex = UnityEngine.Random.Range(0, waypoints.Count);
            _currentWaypointIndex = randomIndex; // Sets the current waypoint index
            aiNavMeshAgent.SetDestination(waypoints[_currentWaypointIndex].position); // Sets the alien's destination to the selected waypoint
        }

        // Checks if there is a target available
        if (target != null)
        {
            // Calculates the distance to the target
            float distanceTotarget = Vector3.Distance(transform.position, target.position);
            // If within chase distance, transitions to chase state
            if (distanceTotarget <= chaseDistance)
            {
                _currentState = AlienAIstate.chase; // Switches to chase state
            }
        }
    }

    // Handles the chase state behaviour
    private void HandleChaseState()
    {
        // Sets the alien's destination to the target's position
        if (target != null)
        {
            aiNavMeshAgent.SetDestination(target.position);
        }
        else
        {
            // If the target is lost, transitions back to patrol state
            _currentState = AlienAIstate.patrol;
            return;
        }

        // Calculates the distance to the target
        float distanceTotarget = Vector3.Distance(transform.position, target.position);
        // If within attack distance, transitions to attack state
        if (distanceTotarget <= attackDistance)
        {
            _currentState = AlienAIstate.attack; // Switches to attack state
            return;
        }
        // If the target moves out of the chase distance, transitions back to patrol state
        else if (distanceTotarget > chaseDistance)
        {
            _currentState = AlienAIstate.patrol; // Switches to patrol state
        }
    }

    // Handles the attack state behaviour
    private void HandleAttackState()
    {
        // Checks if the target is not null before proceeding
        if (target == null)
        {
            // If the target is lost, transitions back to patrol state
            _currentState = AlienAIstate.patrol;
            return;
        }

        // Rotates towards the target to face it
        Vector3 direction = (target.position - transform.position).normalized; // Calculate the direction to the target
        Quaternion lookRotation = Quaternion.LookRotation(direction); // Create a rotation to look at the target
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3.0f); // Smoothly rotate towards the target

        // Fires laser at intervals based on the fire timer
        _fireTimer -= Time.deltaTime;
        if (_fireTimer <= 0)
        {
            FireLaser(); // Fires a laser towards the target
            _fireTimer = _fireInterval; // Resets the fire timer
        }

        // Calculates the distance to the target
        float distanceTotarget = Vector3.Distance(transform.position, target.position);
        // If the target moves out of attack distance, transitions back to chase state
        if (distanceTotarget > attackDistance)
        {
            _currentState = AlienAIstate.chase; // Switches to chase state
        }
    }

    // Fires a laser towards the target
    private void FireLaser()
    {
        if (laserPrefab != null)
        {
            // Calculates the direction of the laser
            Vector3 laserDirection = (target.position - transform.position).normalized;
            // Sets the rotation of the laser to face the target
            Quaternion laserRotation = Quaternion.LookRotation(laserDirection);
            // Instantiates the laser at the alien's position with the calculated rotation
            GameObject laser = Instantiate(laserPrefab, transform.position, laserRotation);
            // Gets the Rigidbody component of the laser to apply velocity
            Rigidbody laserRb = laser.GetComponent<Rigidbody>();
            // Sets the velocity of the laser to move towards the target
            laserRb.velocity = laserDirection * _laserSpeed;
            Debug.Log ("Laser Fired"); // Logs to the console for debugging
        }
    }

    // Handles trigger enter events such as when the alien detects a player
    private void OnTriggerEnter(Collider other)
    {
        // If the detected object is tagged as "Player"
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log ("Detected Human"); // Logs to the console for debugging
            target = other.transform; // Sets the target to the detected player
        }
    }

    // Handles trigger exit events such as when the player leaves the detection range
    private void OnTriggerExit(Collider other)
    {
        // If the detected object is tagged as "Player"
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log ("No longer detecting Human"); // Logs to the console for debugging
            target = null; // Clears the target reference
        }
    }
}