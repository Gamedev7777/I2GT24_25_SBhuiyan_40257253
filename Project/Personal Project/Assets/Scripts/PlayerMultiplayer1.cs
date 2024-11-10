using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMultiplayer1 : MonoBehaviour
{
    // public variables
    public float playerSpeed = 5.0f; // Speed at which the player moves
    public GameObject bulletPrefab; // Bullet prefab used for shooting


    // private variables
    private float _horizontalMovement; // Horizontal movement input
    private float _verticalMovement; // Vertical movement input
    private Vector3 _playerMovement; // Player movement vector
    private float _bulletSpeed = 30.0f; // Speed at which the bullet moves
    private float _fireThreshold = 0.1f;
    private float _fireCooldown = 0.5f;
    private float _lastFireTime;


    // Update is called once per frame
    void Update()
    {
        // Gets player input for horizontal movement (left/right)
        _horizontalMovement = Input.GetAxis("Horizontal");
        // Gets player input for vertical movement (up/down)
        _verticalMovement = Input.GetAxis("Vertical");

        // Creates a new movement vector based on the player's input
        _playerMovement = new Vector3(_horizontalMovement, 0, _verticalMovement).normalized;

        // Moves the player based on the input and playerSpeed
        transform.Translate(playerSpeed * Time.deltaTime * _playerMovement, Space.World);


        // Checks if the left mouse button is pressed to fire a bullet
        if (Input.GetMouseButtonDown(0))
        {
            FireBullet(); // Calls the function to fire a bullet
        }
    }


    // Function to fire a bullet from the player's position
    void FireBullet()
    {
        // Instantiates the bullet prefab at the player's current position
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Debug.Log("Bullet fired"); // Debug log to indicate a bullet was fired

        // Gets the Rigidbody component of the bullet to control its movement
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            // Creates a ray from the camera to the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Performs a raycast to determine where the mouse cursor intersects with the game world
            if (Physics.Raycast(ray, out hit))
            {
                // Calculates the direction from the player's position to the target hit point
                Vector3 direction = (hit.point - transform.position).normalized;
                // Sets the bullet's velocity in the direction of the target
                bulletRb.velocity = direction * _bulletSpeed;
            }
        }

        // Destroys the bullet after 1 second to avoid cluttering the scene with unused bullets
        Destroy(bullet, 1.0f);
    }
}