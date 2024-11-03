using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovingWall : MonoBehaviour
{
    private NavMeshObstacle obstacle;

    void Start()
    {
        obstacle = GetComponent<NavMeshObstacle>();
    }

    void Update()
    {
        // Force the nav mesh to update to the new position of the moving wall
        obstacle.carving = false;
        obstacle.carving = true;
    }
}
