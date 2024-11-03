using System;
using System.Collections;
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
    public NavMeshAgent aiNavMeshAgent; // Reference to the NavMeshAgent component
    public List<Transform> waypoints = new List<Transform>(); // List of waypoints for patrolling
    public Transform target; // Reference to the target transform
    public float chaseDistance = 25.0f; // Distance within which the alien starts chasing the target
    public float attackDistance = 15.0f; // Distance within which the alien starts attacking the target
    public GameObject laserPrefab; // Prefab for the laser attack

    // Private variables
    private AlienAIstate _currentState = AlienAIstate.idle; // Current state of the alien
    private int _currentWaypointIndex = -1; // Index of the current waypoint
    private float _idleTimer = 0.0f; // Timer to keep track of idle time
    private float _laserSpeed = 50.0f; // Speed of the laser
    private float _fireTimer; // Timer to manage firing intervals
    private float _fireInterval = 1.0f; // Time interval between laser shots

    // Start is called before the first frame update
    void Start()
    {
        aiNavMeshAgent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component attached to the alien
        _fireTimer = _fireInterval; // Initialize the fire timer
    }

    // Update is called once per frame
    void Update()
    {
        // Execute behavior based on the current state
        switch (_currentState)
        {
            case AlienAIstate.idle:
                HandleIdleState();
                break;
            case AlienAIstate.patrol:
                HandlePatrolState();
                break;
            case AlienAIstate.chase:
                HandleChaseState();
                break;
            case AlienAIstate.attack:
                HandleAttackState();
                break;
        }
    }

    // Handles the idle state behavior
    void HandleIdleState()
    {
        // Increment the idle timer
        _idleTimer += Time.deltaTime;
        // Transition to patrol state after idle time has elapsed
        if (_idleTimer >= idleTime)
        {
            _currentState = AlienAIstate.patrol;
            _idleTimer = 0.0f; // Reset the idle timer
        }
    }

    // Handles the patrol state behavior
    void HandlePatrolState()
    {
        // If the agent is not currently moving and is close to the current waypoint
        if ((aiNavMeshAgent.pathStatus == NavMeshPathStatus.PathComplete && aiNavMeshAgent.remainingDistance < 0.5f) ||
            _currentWaypointIndex == -1)
        {
            // Move to the next waypoint in the list (loop back to the beginning if at the end)
            int randomIndex = UnityEngine.Random.Range(0, waypoints.Count);

            _currentWaypointIndex = randomIndex;
            aiNavMeshAgent.SetDestination(waypoints[_currentWaypointIndex].position);
        }

        if (target != null)
        {
            // Check the distance to the target, if within chase distance, transition to chase state
            float distanceTotarget = Vector3.Distance(transform.position, target.position);

            _currentState = AlienAIstate.chase;
        }
    }

    // Handles the chase state behavior
    void HandleChaseState()
    {
        // Set the agent's destination to the target's position
        if (target != null) aiNavMeshAgent.SetDestination(target.position);
        else
        {
            _currentState = AlienAIstate.patrol;
            return;
        }

        // Calculate the distance to the target
        float distanceTotarget = Vector3.Distance(transform.position, target.position);
        // If within attack distance, transition to attack state
        if (distanceTotarget <= attackDistance)
        {
            _currentState = AlienAIstate.attack;
            return;
        }
        else if (distanceTotarget > chaseDistance)
        {
            _currentState = AlienAIstate.patrol;
        }
    }

    // Handles the attack state behavior
    void HandleAttackState()
    {
        // Check if the target is not null before proceeding
        if (target == null)
        {
            _currentState = AlienAIstate.patrol;
            return;
        }

        // Rotate towards the target
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3.0f);

        // Fire laser at intervals
        _fireTimer -= Time.deltaTime;
        if (_fireTimer <= 0)
        {
            FireLaser();
            _fireTimer = _fireInterval; // Reset the fire timer
        }

        // Calculate the distance to the target
        float distanceTotarget = Vector3.Distance(transform.position, target.position);
        // If the target moves out of attack distance, transition back to chase state
        if (distanceTotarget > attackDistance)
        {
            _currentState = AlienAIstate.chase;
        }
    }

    // Fires a laser towards the target
    void FireLaser()
    {
        if (laserPrefab != null)
        {
            Vector3 laserDirection =
                (target.position - transform.position).normalized; // Calculate the direction of the laser
            Quaternion laserRotation = Quaternion.LookRotation(laserDirection); // Set the rotation of the laser
            GameObject laser = Instantiate(laserPrefab, transform.position, laserRotation); // Instantiate the laser
            Rigidbody laserRb = laser.GetComponent<Rigidbody>(); // Get the Rigidbody component of the laser
            laserRb.velocity = laserDirection * _laserSpeed; // Set the velocity of the laser
            print ("Laser Fired");
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            print("Detected Human");
            target = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            print("No longer detecting Human");
            target = null;
        }
    }
}