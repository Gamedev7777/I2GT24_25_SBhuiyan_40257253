using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMultiplayer2 : MonoBehaviour
{
    // public variables
    public float playerSpeed = 5.0f; // Speed at which the player moves
    public GameObject bulletPrefab; // Bullet prefab used for shooting

    // private variables
    private float _horizontalMovement; // Horizontal movement input
    private float _verticalMovement; // Vertical movement input
    private Vector3 _playerMovement; // Player movement vector
    private float _bulletSpeed = 30.0f; // Speed at which the bullet moves
    private float _fireThreshold = 0.1f; // Threshold value for firing
    private float _fireCooldown = 0.5f; // Cooldown time between firing bullets
    private float _lastFireTime; // Tracks the time of the last fired bullet

    // Update is called once per frame
    void Update()
    {
        // Gets player input for horizontal movement (left/right)
        _horizontalMovement = Input.GetAxis("HorizontalPlayer2Xbox");
        // Gets player input for vertical movement (up/down)
        _verticalMovement = Input.GetAxis("VerticalPlayer2Xbox");

        // Creates a new movement vector based on the player's input
        _playerMovement = new Vector3(_horizontalMovement, 0, _verticalMovement).normalized;

        // Moves the player based on the input and playerSpeed
        transform.Translate(playerSpeed * Time.deltaTime * _playerMovement, Space.World);

        // Gets input from the right stick for aiming direction
        float rightStickHorizontal = Input.GetAxis("RightStickHorizontal");
        float rightStickVertical = Input.GetAxis("RightStickVertical");
        Vector3 aimDirection = DetermineDirection(new Vector2(rightStickHorizontal, rightStickVertical));

        // Fires a bullet if aiming direction is provided and cooldown time has passed
        if (aimDirection != Vector3.zero && Time.time >= _lastFireTime + _fireCooldown)
        {
            FireBulletXbox(aimDirection);
            _lastFireTime = Time.time; // Updates the last fire time
        }
    }

    // Determines the direction based on input from the right stick
    Vector3 DetermineDirection(Vector2 _stickInput)
    {
        // If the input magnitude is below the threshold, returns zero vector which is no direction
        if (_stickInput.sqrMagnitude < _fireThreshold * _fireThreshold)
        {
            return Vector3.zero; // No firing direction
        }

        // Normalizes the stick input to get a direction which ensures consistent speed
        _stickInput.Normalize();

        // Converts the input angle to a direction vector based on the stick position
        float angle = Mathf.Atan2(_stickInput.y, _stickInput.x) * Mathf.Rad2Deg;
        if (angle >= -22.5f && angle < 22.5f) return Vector3.right; // Right direction
        else if (angle >= 22.5f && angle < 67.5f) return new Vector3(1, 0, 1).normalized; // Diagonal right-forward
        else if (angle >= 67.5f && angle < 112.5f) return Vector3.forward; // Forward direction
        else if (angle >= 112.5f && angle < 157.5f) return new Vector3(-1, 0, 1).normalized; // Diagonal left-forward
        else if ((angle >= 157.5f && angle < 180f) || (angle < -157f && angle >= -180f))
            return Vector3.left; // Left direction
        else if (angle >= -157.5f && angle < -112.5f)
            return new Vector3(-1, 0, -1).normalized; // Diagonal left-backward
        else if (angle >= -112.5f && angle < -67.5f) return Vector3.back; // Backward direction
        else if (angle >= -67.5f && angle < -22.5f) return new Vector3(1, 0, -1).normalized; // Diagonal right-backward
        return Vector3.zero; // Default case (no direction)
    }

    // Fires a bullet in a specific direction based on controller input
    void FireBulletXbox(Vector3 _direction)
    {
        // Instantiates the bullet prefab at the player's current position
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        // Gets the Rigidbody component of the bullet to control its movement
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

        if (bulletRb != null)
        {
            // Sets the bullet's velocity in the specified direction
            bulletRb.velocity = _direction * _bulletSpeed;
        }

        // Destroys the bullet after 1 second to avoid cluttering the scene with unused bullets
        Destroy(bullet, 1.0f);
    }
}