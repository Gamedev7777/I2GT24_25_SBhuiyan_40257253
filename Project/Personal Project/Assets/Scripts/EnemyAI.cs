using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIstate {idle, patrol, chase, attack}
public class EnemyAI : MonoBehaviour
{
    private AIstate _currentState = AIstate.idle;
    
    public float idleTime = 5.0f;
    public NavMeshAgent aiNavMeshAgent;
    public List<Transform> waypoints = new List<Transform>();
    private int _currentWaypointIndex = 0;
    private float _idleTimer = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        aiNavMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (_currentState)
        {
            case AIstate.idle:
                print ("Idle state");
                _idleTimer += Time.deltaTime;
                if (_idleTimer >= idleTime)
                {
                    
                    _currentState = AIstate.patrol;
                    _idleTimer = 0.0f;
                    break;
                }

                break;
            case AIstate.patrol:
                print ("Patrol state");
                int randomIndex = Random.Range(0, waypoints.Count);
                if (randomIndex == _currentWaypointIndex)
                {
                    break;
                }

                if (aiNavMeshAgent.pathStatus == NavMeshPathStatus.PathComplete &&
                    aiNavMeshAgent.remainingDistance <= 2.0f)
                {
                    aiNavMeshAgent.SetDestination(waypoints[randomIndex].position);
                }
                    
                break;
            case AIstate.chase:
                break;
            case AIstate.attack:
                break;
        }
        
        
    }
}
