using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Enum representing different AI states for the Alien
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
    public GameObject target; // Reference to the target transform which is the player
    public float chaseDistance = 25.0f; // Distance within which the alien starts chasing the target
    public float attackDistance = 15.0f; // Distance within which the alien starts attacking the target
    public GameObject laserPrefab; // Prefab for the laser

    // Private variables
    private AlienAIstate _currentState = AlienAIstate.idle; // Current state of the alien
    private int _currentWaypointIndex = -1; // Index of the current waypoint for patrolling
    private float _idleTimer = 0.0f; // Timer to keep track of idle time
    private float _laserSpeed = 30.0f; // Speed of the laser projectile
    private float _fireTimer; // Timer to manage firing intervals in attack state
    private float _fireInterval = 1.0f; // Time interval between laser shots

    private Animation animation; // Reference to the Animation component of the Alien

    // Start is called before the first frame update
    void Start()
    {
        // Gets the animation component from the first child of the alien game object
        animation = transform.GetChild(0).GetComponent<Animation>();

        // Finds the player by tag and assigns it as the target
        target = GameObject.FindGameObjectWithTag("Player");

        // Finds the waypoints by name and assigns them to the list
        waypoints[0] = GameObject.Find("Waypoint1").transform;
        waypoints[1] = GameObject.Find("Waypoint2").transform;
        waypoints[2] = GameObject.Find("Waypoint3").transform;
        waypoints[3] = GameObject.Find("Waypoint4").transform;

        // Gets the NavMeshAgent component attached to the alien for navigation
        aiNavMeshAgent = GetComponent<NavMeshAgent>();

        // Initializes the fire timer with the interval value
        _fireTimer = _fireInterval;
    }

    // Update is called once per frame
    void Update()
    {
        // Executes behaviour based on the current state
        switch (_currentState)
        {
            case AlienAIstate.idle:
                Debug.Log("idle");
                HandleIdleState(); // Handles behaviour when the alien is idle
                break;
            case AlienAIstate.patrol:
                Debug.Log("patrol");
                HandlePatrolState(); // Handles behaviour when the alien is patrolling
                break;
            case AlienAIstate.chase:
                Debug.Log("chase");
                HandleChaseState(); // Handles behaviour when the alien is chasing the target
                break;
            case AlienAIstate.attack:
                Debug.Log("attack");
                HandleAttackState(); // Handles behaviour when the alien is attacking the target
                break;
        }
    }

    // Handles the idle state behaviour
    void HandleIdleState()
    {
        // Increments the idle timer
        _idleTimer += Time.deltaTime;

        // Plays idle animation based on the alien type
        if (gameObject.tag == "Alien2")
        {
            animation.Play("ZlorpSoldierIdle");
        }
        else if (gameObject.tag == "Alien1")
        {
            animation.Play("ZlorpIdle");
        }

        // Transitions to patrol state after idle time has elapsed
        if (_idleTimer >= idleTime)
        {
            _currentState = AlienAIstate.patrol; // Switches to patrol state
            _idleTimer = 0.0f; // Resets the idle timer
        }
    }

    // Handles the patrol state behaviour
    void HandlePatrolState()
    {
        // If the alien has reached the current waypoint or has not been assigned a waypoint
        if ((aiNavMeshAgent.pathStatus == NavMeshPathStatus.PathComplete && aiNavMeshAgent.remainingDistance < 0.5f) ||
            _currentWaypointIndex == -1)
        {
            // Moves to a random waypoint in the list
            int randomIndex = UnityEngine.Random.Range(0, waypoints.Count);
            _currentWaypointIndex = randomIndex; // Sets the current waypoint index
            aiNavMeshAgent.SetDestination(waypoints[_currentWaypointIndex].position); // Sets the alien's destination to the selected waypoint

            // Plays walking animation based on the alien type
            if (gameObject.tag == "Alien2")
            {
                animation.Play("ZlorpSoldierWalking");
            }
            else if (gameObject.tag == "Alien1")
            {
                animation.Play("ZlorpWalking");
            }
        }

        // Checks if there is a target available
        if (target != null)
        {
            // Calculates the distance to the target
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

            // If within chase distance, transitions to chase state
            if (distanceToTarget <= chaseDistance)
            {
                _currentState = AlienAIstate.chase; // Switches to chase state
            }
        }
    }

    // Checks if the alien is currently moving
    private bool IsMoving()
    {
        float speedThreshold = 0.1f; // Minimum speed to consider as movement
        float distanceThreshold = 0.1f; // Minimum remaining distance to consider as moving

        return aiNavMeshAgent.velocity.magnitude > speedThreshold &&
               aiNavMeshAgent.remainingDistance > distanceThreshold;
    }

    // Handles the chase state behaviour
    void HandleChaseState()
    {
        // Sets the alien's destination to the target's position
        if (target != null)
        {
            aiNavMeshAgent.SetDestination(target.transform.position);

            // Plays walking animation based on the alien type
            if (gameObject.tag == "Alien2")
            {
                animation.Play("ZlorpSoldierWalking");
            }
            else if (gameObject.tag == "Alien1")
            {
                animation.Play("ZlorpWalking");
            }
        }
        else
        {
            // If the target is lost, transitions back to patrol state
            _currentState = AlienAIstate.patrol;
            return;
        }

        // Calculates the distance to the target
        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        // If within attack distance, transitions to attack state
        if (distanceToTarget <= attackDistance)
        {
            _currentState = AlienAIstate.attack; // Switches to attack state
            return;
        }
        // If the target moves out of the chase distance, transitions back to patrol state
        else if (distanceToTarget > chaseDistance)
        {
            _currentState = AlienAIstate.patrol; // Switches to patrol state
        }
    }

    // Handles the attack state behaviour
    void HandleAttackState()
    {
        // Checks if the target is not null before proceeding
        if (target == null)
        {
            // If the target is lost, transitions back to patrol state
            _currentState = AlienAIstate.patrol;
            return;
        }

        // Rotates towards the target to face it
        Vector3 direction = (target.transform.position - transform.position).normalized; // Calculate the direction to the target
        Quaternion lookRotation = Quaternion.LookRotation(direction); // Create a rotation to look at the target
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3.0f); // Smoothly rotate towards the target

        // Fires laser at intervals based on the fire timer
        _fireTimer -= Time.deltaTime;

        // Plays the appropriate shooting or walking animation
        if (_fireTimer <= 0 && IsMoving())
        {
            if (gameObject.tag == "Alien2")
            {
                animation.Play("ZlorpSoldierWalkingFiring");
            }
            else if (gameObject.tag == "Alien1")
            {
                animation.Play("ZlorpWalkingFiring");
            }
        }
        else
        {
            if (_fireTimer <= 0)
            {
                PlayShootingAnimation(); // Plays the shooting animation
                FireLaser(); // Fires a laser towards the target
                _fireTimer = _fireInterval; // Resets the fire timer
            }
            else
            {
                StopShootingAnimation(); // Stops the shooting animation
            }
        }

        // Calculates the distance to the target
        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        // If the target moves out of attack distance, transitions back to chase state
        if (distanceToTarget > attackDistance)
        {
            _currentState = AlienAIstate.chase; // Switches to chase state
        }
    }

    // Plays the shooting animation if it is not already playing
    void PlayShootingAnimation()
    {
        if (gameObject.tag == "Alien2")
        {
            if (!animation.IsPlaying("ZlorpSoldierIdleFiring"))
            {
                animation.Play("ZlorpSoldierIdleFiring");
            }
        }
        else if (gameObject.tag == "Alien1")
        {
            if (!animation.IsPlaying("ZlorpIdleFiring"))
            {
                animation.Play("ZlorpIdleFiring");
            }
        }
    }

    // Stops the shooting animation if it is playing
    void StopShootingAnimation()
    {
        if (gameObject.tag == "Alien2")
        {
            if (!animation.IsPlaying("ZlorpSoldierIdleFiring"))
            {
                animation.Stop("ZlorpSoldierIdleFiring");
            }
        }
        else if (gameObject.tag == "Alien1")
        {
            if (!animation.IsPlaying("ZlorpIdleFiring"))
            {
                animation.Stop("ZlorpIdleFiring");
            }
        }
    }

    // Fires a laser towards the target
    void FireLaser()
    {
        if (laserPrefab != null)
        {
            // Calculates the direction of the laser
            Vector3 laserDirection = (target.transform.position - transform.position).normalized;

            // Sets the rotation of the laser to face the target
            Quaternion laserRotation = Quaternion.LookRotation(laserDirection);

            // Instantiates the laser at the alien's position with the calculated rotation
            GameObject laser = Instantiate(laserPrefab, transform.position, laserRotation);

            // Gets the Rigidbody component of the laser to apply velocity
            Rigidbody laserRb = laser.GetComponent<Rigidbody>();

            // Sets the velocity of the laser to move towards the target
            laserRb.velocity = laserDirection * _laserSpeed;

            Debug.Log("Laser Fired"); // Logs to the console for debugging
        }
    }

    // Handles trigger enter events such as when the alien detects a player
    void OnTriggerEnter(Collider other)
    {
        // If the detected object is tagged as "Player"
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Detected Human"); // Logs to the console for debugging
            target = other.transform.gameObject; // Sets the target to the detected player
        }
    }

    // Handles trigger exit events such as when the player leaves the detection range
    void OnTriggerExit(Collider other)
    {
        // If the detected object is tagged as "Player"
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("No longer detecting Human"); // Logs to the console for debugging
            target = null; // Clears the target reference
        }
    }
}
