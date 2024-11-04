using UnityEngine;
using UnityEngine.AI;

public class MovingWall : MonoBehaviour
{
    // private variable
    // Reference to the NavMeshObstacle component attached to the wall
    private NavMeshObstacle _obstacle;

    void Start()
    {
        // Gets the NavMeshObstacle component from the current GameObject
        _obstacle = GetComponent<NavMeshObstacle>();
    }

    void Update()
    {
        // Disables and re-enables carving to force the NavMesh to update
        // This allows the NavMesh to account for the new position of the wall
        _obstacle.carving = false;
        _obstacle.carving = true;
    }
}