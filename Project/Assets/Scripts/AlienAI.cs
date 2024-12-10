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
    public GameObject target; // Reference to the target transform which is the player
    public float chaseDistance = 20.0f; // Distance within which the alien starts chasing the target
    public float attackDistance = 10.0f; // Distance within which the alien starts attacking the target
    public GameObject laserPrefab; // Prefab for the laser projectile
    public Transform laserSpawnPosition; // Position from which the laser will be fired
    public AudioClip laserSound; // Sound clip to be played when the laser is fired

    // Private variables
    private List<Transform> _waypoints = new(); // List of waypoints for patrolling
    private NavMeshAgent _aiNavMeshAgent; // Reference to the NavMeshAgent component for movement
    private readonly float _maxIdleTime = 1.0f; // Time to stay idle before transitioning to patrol
    private AlienAIstate _currentState = AlienAIstate.idle; // Current state of the alien
    private int _currentWaypointIndex = -1; // Index of the current waypoint for patrolling
    private float _idleTimer = 1.0f; // Timer to keep track of idle time
    private readonly float _laserSpeed = 30.0f; // Speed of the laser projectile
    private float _fireTimer; // Timer to manage firing intervals in attack state
    private readonly float _fireInterval = 0.5f; // Time interval between laser shots
    private Animation _animation; // Reference to the Animation component of the Alien
    private float _distanceToTarget;

    // Start is called before the first frame update
    void Start()
    {
        // Gets the animation component from the first child of the alien game object
        _animation = transform.GetChild(0).GetComponent<Animation>();

        // Finds the waypoints by name and assigns them to the list
        _waypoints[0] = GameObject.Find("Waypoint1").transform;
        _waypoints[1] = GameObject.Find("Waypoint2").transform;
        _waypoints[2] = GameObject.Find("Waypoint3").transform;
        _waypoints[3] = GameObject.Find("Waypoint4").transform;

        // Gets the NavMeshAgent component attached to the alien for navigation
        _aiNavMeshAgent = GetComponent<NavMeshAgent>();

        // Initialises the fire timer with the interval value
        _fireTimer = _fireInterval;
    }

    // Update is called once per frame
    void Update()
    {
        // Executes behaviour based on the current state
        switch (_currentState)
        {
            case AlienAIstate.idle:
//                Debug.Log("idle");
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
            if (PlayerPrefs.GetInt("Cutscene", 1) == 1 &&
                (PlayerPrefs.GetInt("Level", 1) == 4 || PlayerPrefs.GetInt("Level", 1) == 5 ||
                 PlayerPrefs.GetInt("Level", 1) == 6 || PlayerPrefs.GetInt("Level", 1) == 7))
            {
                Debug.Log("Talking");
                _animation.Play("ZlorpSoldierTalking");
            }
            else
            {
                Debug.Log("idle");
                _animation.Play("ZlorpSoldierIdle");
            }
        }


        else if (gameObject.tag == "Alien1")
        {
            _animation.Play("ZlorpIdle");
        }

        if (_idleTimer >= _maxIdleTime && PlayerPrefs.GetInt("Cutscene", 1) == 0)
        {
            Debug.Log("patrol");
            _currentState = AlienAIstate.patrol; // Switches to patrol state
            _idleTimer = 0.0f; // Resets the idle timer
        }
    }

    // Handles the patrol state behaviour
    void HandlePatrolState()
    {
        // If the alien has reached the current waypoint or has not been assigned a waypoint
        if ((_aiNavMeshAgent.pathStatus == NavMeshPathStatus.PathComplete && _aiNavMeshAgent.remainingDistance < 0.5f) ||
            _currentWaypointIndex == -1)
        {
            // Moves to a random waypoint in the list
            int randomIndex = Random.Range(0, _waypoints.Count);
            _currentWaypointIndex = randomIndex; // Sets the current waypoint index
            _aiNavMeshAgent.SetDestination(_waypoints[_currentWaypointIndex]
                .position); // Sets the alien's destination to the selected waypoint
            Debug.Log(_waypoints[_currentWaypointIndex]);
            // Plays walking animation based on the alien type
            if (gameObject.tag == "Alien2")
            {
                _animation.Play("ZlorpSoldierWalking");
            }
            else if (gameObject.tag == "Alien1")
            {
                _animation.Play("ZlorpWalking");
            }
        }

        Debug.Log(target);
        // Checks if there is a target available
        if (target != null)
        {
            // Calculates the distance to the target
            float _distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            
            // If within chase distance, transitions to chase state
            if (_distanceToTarget > chaseDistance)
            {
                Debug.Log("chase 1");
                _currentState = AlienAIstate.chase; // Switches to chase state
            }
            else
            {
                _aiNavMeshAgent.SetDestination(target.transform.position);

                Quaternion targetRotation = Quaternion.LookRotation(_aiNavMeshAgent.velocity.normalized);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 1000f * Time.deltaTime);
                Debug.Log("attack 1");
                _currentState = AlienAIstate.attack; // Switches to attack state
            }
        }
    }

    // Checks if the alien is currently moving
    private bool IsMoving()
    {
        float speedThreshold = 0.1f; // Minimum speed to consider as movement
        float distanceThreshold = 0.1f; // Minimum remaining distance to consider as moving

        return _aiNavMeshAgent.velocity.magnitude > speedThreshold &&
               _aiNavMeshAgent.remainingDistance > distanceThreshold;
    }

    // Handles the chase state behaviour
    void HandleChaseState()
    {
        // Sets the alien's destination to the target's position
        if (target != null)
        {
            _aiNavMeshAgent.SetDestination(target.transform.position);

            Quaternion targetRotation = Quaternion.LookRotation(_aiNavMeshAgent.velocity.normalized);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 1000f * Time.deltaTime);


            // Plays walking animation based on the alien type
            if (gameObject.tag == "Alien2")
            {
                _animation.Play("ZlorpSoldierWalking");
            }
            else if (gameObject.tag == "Alien1")
            {
                _animation.Play("ZlorpWalking");
            }
        }
        else
        {
            // If the target is lost, transitions back to patrol state
            _currentState = AlienAIstate.patrol;
            return;
        }

        // Calculates the distance to the target
        _distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        // If within attack distance, transitions to attack state
        if (_distanceToTarget <= attackDistance)
        {
            Debug.Log("Attack 1");
            _currentState = AlienAIstate.attack; // Switches to attack state
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

        // Calculates the direction to the target for rotation and attack
        Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
        _distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        // Checks for direct line of sight to the player using a raycast
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToTarget, out hit))
        {
            if (hit.collider.gameObject != target && _distanceToTarget > attackDistance)
            {
                Debug.Log("Chase 2");
                // If there is no direct line of sight, transitions back to chase state
                _currentState = AlienAIstate.chase;
                return;
            }
        }

        // Rotates towards the target to face it
        Quaternion
            lookRotation = Quaternion.LookRotation(directionToTarget); // Creates a rotation to look at the target

        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 1000f * Time.deltaTime);


        // Fires laser at intervals based on the fire timer
        _fireTimer -= Time.deltaTime;

        // Plays the appropriate shooting or walking animation
        if (_fireTimer <= 0 && IsMoving())
        {
            if (gameObject.tag == "Alien2")
            {
                _animation.Play("ZlorpSoldierWalkingFiring");
                if (_fireTimer <= 0)
                {
                    FireLaser(); // Fires a laser towards the target
                    _fireTimer = _fireInterval; // Resets the fire timer
                }
                
            }
            else if (gameObject.tag == "Alien1")
            {
                _animation.Play("ZlorpWalkingFiring");
                if (_fireTimer <= 0)
                {
                    FireLaser(); // Fires a laser towards the target
                    _fireTimer = _fireInterval; // Resets the fire timer
                }
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
        _distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        // If the target moves out of attack distance, transitions back to chase state
        if (_distanceToTarget > chaseDistance)
        {
            Debug.Log("Chase 3");
            _currentState = AlienAIstate.chase; // Switches to chase state
        }
    }

    // Plays the shooting animation if it is not already playing
    void PlayShootingAnimation()
    {
        if (gameObject.tag == "Alien2")
        {
            if (!_animation.IsPlaying("ZlorpSoldierIdleFiring"))
            {
                _animation.Play("ZlorpSoldierIdleFiring");
            }
        }
        else if (gameObject.tag == "Alien1")
        {
            if (!_animation.IsPlaying("ZlorpIdleFiring"))
            {
                _animation.Play("ZlorpIdleFiring");
            }
        }
    }

    // Stops the shooting animation if it is playing
    void StopShootingAnimation()
    {
        if (gameObject.tag == "Alien2")
        {
            if (!_animation.IsPlaying("ZlorpSoldierIdleFiring"))
            {
                _animation.Stop("ZlorpSoldierIdleFiring");
            }
        }
        else if (gameObject.tag == "Alien1")
        {
            if (!_animation.IsPlaying("ZlorpIdleFiring"))
            {
                _animation.Stop("ZlorpIdleFiring");
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

            // Plays the laser sound effect at the camera's position with 40% of the original volume
            AudioSource.PlayClipAtPoint(laserSound, SpawnManager.instance.transform.position, 0.4f);

            // Instantiates the laser at the alien's position with the calculated rotation
            GameObject laser = Instantiate(laserPrefab, laserSpawnPosition.position, laserRotation);

            // Gets the Rigidbody component of the laser to apply velocity
            Rigidbody laserRb = laser.GetComponent<Rigidbody>();

            // Sets the velocity of the laser to move towards the target
            laserRb.velocity = laserDirection * _laserSpeed;
        }
    }
}