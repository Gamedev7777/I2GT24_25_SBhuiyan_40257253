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
    private float _distanceToTarget; // Variable to save the calculation of the distance between alien and the target

    // Start is called before the first frame update
    void Start()
    {
        AssignAnimationComponent();
       
        AddTheWaypoints();
        
        AssignNavMeshAgentComponent();
        
        InitialiseFireTimer();
    }

    private void InitialiseFireTimer()
    {
        _fireTimer = _fireInterval;
    }

    private void AssignNavMeshAgentComponent()
    {
        _aiNavMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void AssignAnimationComponent()
    {
        _animation = transform.GetChild(0).GetComponent<Animation>();
    }

    private void AddTheWaypoints()
    {
        for (int i = 0; i < 4; i++)
        {
            _waypoints.Add(GameObject.Find("Waypoint"+(i+1)).transform);
        }
    }

    // Update is called once per frame
    void Update()
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

   void HandleIdleState()
    {
        IncrementIdleTimer();
        
        PlayIdleAnimation();

        if (_idleTimer >= _maxIdleTime && PlayerPrefs.GetInt("Cutscene", 1) == 0)
        {
            _currentState = AlienAIstate.patrol; // Switches to patrol state
            _idleTimer = 0.0f; // Resets the idle timer
        }
    }

    private void PlayIdleAnimation()
    {
        if (gameObject.CompareTag("Alien2"))
        {
            _animation.Play(IsTalkingCutscene() ? "ZlorpSoldierTalking" : "ZlorpSoldierIdle");
        }
        else if (gameObject.CompareTag("Alien1"))
        {
            _animation.Play("ZlorpIdle");
        }
    }

    private static bool IsTalkingCutscene()
    {
        return PlayerPrefs.GetInt("Cutscene", 1) == 1 &&
               (PlayerPrefs.GetInt("Level", 1) == 4 || PlayerPrefs.GetInt("Level", 1) == 5 ||
                PlayerPrefs.GetInt("Level", 1) == 6 || PlayerPrefs.GetInt("Level", 1) == 7);
    }

    private void IncrementIdleTimer()
    {
        _idleTimer += Time.deltaTime;
    }

    void HandlePatrolState()
    {
        // (If the alien has almost reached the current waypoint and the path status is complete) or has not been assigned a waypoint 
        if ((_aiNavMeshAgent.pathStatus == NavMeshPathStatus.PathComplete && _aiNavMeshAgent.remainingDistance < 0.5f) ||
            _currentWaypointIndex == -1)
        { 
            PatrolAlienRandomly();
            PlayWalkingAnimation();
        }

        // Checks if there is a target available
        if (target != null)
        {
            CalculateDistanceToTarget();

            // If within chase distance
            if (_distanceToTarget > chaseDistance)
            {
                _currentState = AlienAIstate.chase; // Switches to chase state
            }
            else
            {
                SetAlienDestination();
                _currentState = AlienAIstate.attack; // Switches to attack state
            }
        }
    }

    private void CalculateDistanceToTarget()
    {
        _distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
    }

    private void PlayWalkingAnimation()
    {
        if (gameObject.CompareTag("Alien2"))
        {
            _animation.Play("ZlorpSoldierWalking");
        }
        else if (gameObject.CompareTag("Alien1"))
        {
            _animation.Play("ZlorpWalking");
        }
    }

    private void PatrolAlienRandomly()
    {
        int randomIndex = Random.Range(0, _waypoints.Count);
        _currentWaypointIndex = randomIndex; // Sets the current waypoint index
        _aiNavMeshAgent.SetDestination(_waypoints[_currentWaypointIndex]
            .position); // Sets the alien's destination to the selected waypoint
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
    private void HandleChaseState()
    {
        // Checks if there is a target available
        if (target != null)
        {
            SetAlienDestination();
            PlayWalkingAnimation();
        }
        else
        {
            // If the target is lost, transitions back to patrol state
            _currentState = AlienAIstate.patrol;
            return;
        }

        CalculateDistanceToTarget();

        // If within attack distance
        if (_distanceToTarget <= attackDistance)
        {
            _currentState = AlienAIstate.attack; // Switches to attack state
        }
    }

    private void SetAlienDestination()
    {
        _aiNavMeshAgent.SetDestination(target.transform.position);
        Quaternion targetRotation = Quaternion.LookRotation(_aiNavMeshAgent.velocity.normalized);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 1000f * Time.deltaTime);
    }

    private void HandleAttackState()
    {
        // Checks if there is a target available
        if (target == null)
        {
            // If the target is lost, transitions back to patrol state
            _currentState = AlienAIstate.patrol;
            return;
        }

        var directionToTarget = CalculateDirectionToTarget();
        CalculateDistanceToTarget();
        
        // Checks for direct line of sight to the player using a raycast
        if (Physics.Raycast(transform.position, directionToTarget, out var hit))
        {
            if (hit.collider.gameObject != target && _distanceToTarget > attackDistance)
            {
                // If there is no direct line of sight, transitions back to chase state
                _currentState = AlienAIstate.chase;
                return;
            }
        }

        RotateTowardsTheTarget(directionToTarget);

        DecrementFireTimer();

        if (_fireTimer <= 0 && IsMoving())
        {
            WalkAndFire();
        }
        else
        {
            if (_fireTimer <= 0)
            {
                PlayShootingAnimation(); 
                FireLaser();
                ResetFireTimer();
            }
            else
            {
                StopShootingAnimation();
            }
        }
        
        CalculateDistanceToTarget();

        // If the target moves out of attack distance
        if (_distanceToTarget > chaseDistance)
        {
            _currentState = AlienAIstate.chase; // Switches to chase state
        }
    }

    private void ResetFireTimer()
    {
        _fireTimer = _fireInterval;
    }
   
    
    private void WalkAndFire()
    {
        if (gameObject.tag == "Alien2")
        {
            _animation.Play("ZlorpSoldierWalkingFiring");
            if (_fireTimer <= 0)
            {
                FireLaser();
                ResetFireTimer();
            }
                
        }
        else if (gameObject.tag == "Alien1")
        {
            _animation.Play("ZlorpWalkingFiring");
            if (_fireTimer <= 0)
            {
                FireLaser();
                ResetFireTimer();
            }
        }
    }

    private void DecrementFireTimer()
    {
        _fireTimer -= Time.deltaTime;
    }

    private void RotateTowardsTheTarget(Vector3 directionToTarget)
    {
        Quaternion
            lookRotation = Quaternion.LookRotation(directionToTarget);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 1000f * Time.deltaTime);
    }

    private Vector3 CalculateDirectionToTarget()
    {
        Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
        return directionToTarget;
    }
    
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
    
    void FireLaser()
    {
        if (laserPrefab != null)
        {
            // Calculates the direction of the laser
            Vector3 laserDirection = (target.transform.position - transform.position).normalized;

            // Sets the rotation of the laser to face the target
            Quaternion laserRotation = Quaternion.LookRotation(laserDirection);

            // Plays the laser sound effect at the camera's position with 40% of the original volume
            AudioSource.PlayClipAtPoint(laserSound, SpawnManager.instance.transform.position, 0.2f);

            // Instantiates the laser at the alien's position with the calculated rotation
            GameObject laser = Instantiate(laserPrefab, laserSpawnPosition.position, laserRotation);

            // Gets the Rigidbody component of the laser to apply velocity
            Rigidbody laserRb = laser.GetComponent<Rigidbody>();

            // Sets the velocity of the laser to move towards the target
            laserRb.velocity = laserDirection * _laserSpeed;
        }
    }
}