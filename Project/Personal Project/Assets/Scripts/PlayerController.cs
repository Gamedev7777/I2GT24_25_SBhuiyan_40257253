using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
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

        float rightStickHorizontal = Input.GetAxis("RightStickHorizontal");
        float rightStickVertical = Input.GetAxis("RightStickVertical");
        Vector3 aimDirection = DetermineDirection(new Vector2(rightStickHorizontal, rightStickVertical));
        if (aimDirection != Vector3.zero && Time.time >= _lastFireTime + _fireCooldown)
        {
            FireBulletXbox(aimDirection);
            _lastFireTime = Time.time;
        }

        // Checks if the left mouse button is pressed to fire a bullet
        if (Input.GetMouseButtonDown(0))
        {
            FireBullet(); // Calls the function to fire a bullet
        }

        HandlePlayerRotation();
    }

    Vector3 DetermineDirection(Vector2 _stickInput)
    {
        if (_stickInput.sqrMagnitude < _fireThreshold * _fireThreshold)
        {
            return Vector3.zero;
        }

        _stickInput.Normalize();

        float angle = Mathf.Atan2(_stickInput.y, _stickInput.x) * Mathf.Rad2Deg;
        if (angle >= -22.5f && angle < 22.5f) return Vector3.right;
        else if (angle >= 22.5f && angle < 67.5f) return new Vector3(1, 0, 1).normalized;
        else if (angle >= 67.5f && angle < 112.5f) return Vector3.forward;
        else if (angle >= 112.5f && angle < 157.5f) return new Vector3(-1, 0, 1).normalized;
        else if ((angle >= 157.5f && angle < 180f) || (angle < -157f && angle >= -180f)) return Vector3.left;
        else if (angle >= -157.5f && angle < -112.5f) return new Vector3(-1, 0, -1).normalized;
        else if (angle >= -112.5f && angle < -67.5f) return Vector3.back;
        else if (angle >= -67.5f && angle < -22.5f) return new Vector3(1, 0, -1).normalized;
        return Vector3.zero;
    }

    void FireBulletXbox(Vector3 _direction)
    {
        // Instantiates the bullet prefab at the player's current position
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Debug.Log("Bullet fired"); // Debug log to indicate a bullet was fired

        // Gets the Rigidbody component of the bullet to control its movement
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

        if (bulletRb != null)
        {
            bulletRb.velocity = _direction * _bulletSpeed;
        }

        // Destroys the bullet after 1 second to avoid cluttering the scene with unused bullets
        Destroy(bullet, 1.0f);
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

    void HandlePlayerRotation()
    {
        {
            // Creates a ray from the camera to the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Performs a raycast to determine where the mouse cursor intersects with the game world
            if (Physics.Raycast(ray, out hit))
            {
                // Calculates the direction from the player's position to the target hit point
                Vector3 direction = (hit.point - transform.position).normalized;
                Quaternion newRotation = Quaternion.LookRotation(direction);
                transform.rotation = newRotation;
            }
        }
    }
}