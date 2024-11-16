using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMultiplayer2 : MonoBehaviour
{
    // public variables
    public float playerSpeed = 5.0f; // Speed at which the player moves
    public GameObject bulletPrefab; // Bullet prefab used for shooting
    public Transform bulletSpawnPosition;
    // private variables
    private float _horizontalMovement; // Horizontal movement input
    private float _verticalMovement; // Vertical movement input
    private Vector3 _playerMovement; // Player movement vector
    private float _bulletSpeed = 30.0f; // Speed at which the bullet moves
    private float _fireThreshold = 0.1f; // Threshold value for firing
    private float _fireCooldown = 0.2f; // Cooldown time between firing bullets
    private float _lastFireTime; // Tracks the time of the last fired bullet
    private Animation animation; // Player animation component
    private Vector3 aimDirection; // Direction the player is aiming

    void Start()
    {
        // Gets the Animation component attached to the player's child object
        animation = transform.GetChild(0).GetComponent<Animation>();
    }

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
        transform.GetComponent<Rigidbody>().velocity = _playerMovement * playerSpeed;

        // Gets input from the right stick for aiming direction
        float rightStickHorizontal = Input.GetAxis("RightStickHorizontal");
        float rightStickVertical = Input.GetAxis("RightStickVertical");
        aimDirection = DetermineDirection(new Vector2(rightStickHorizontal, rightStickVertical));

        // Determines if the player is both moving and aiming, and handles animation and firing
        if ((_horizontalMovement != 0 && _verticalMovement != 0) && (aimDirection != Vector3.zero))
        {
            // Checks if the speed power-up is active and plays appropriate animation
            if (PlayerPrefs.GetInt("SpeedPowerUp", 0) == 1)
            {
                animation.Play("ClaireRunningFiring");
            }
            else
            {
               animation.Play("ClaireWalkingFiring");
            }

            // Fires a bullet if aiming direction is provided and cooldown time has passed
            if (Time.time >= _lastFireTime + _fireCooldown)
            {
                FireBulletXbox(aimDirection);
                _lastFireTime = Time.time; // Updates the last fire time
            }
        }
        else
        {
            // Handles movement when player is not aiming
            if (_horizontalMovement != 0 || _verticalMovement != 0)
            {
                // Checks if the speed power-up is active and plays appropriate animation
                if (PlayerPrefs.GetInt("SpeedPowerUp", 0) == 1)
                {
                    animation.Play("ClaireRunning");
                }
                else
                {
                    animation.Play("ClaireWalking");
                }
            }
            else
            {
                // Player is idle
                animation.Play("ClaireIdle");
            }

            // Handles firing when the player is stationary or just aiming
            if (aimDirection != Vector3.zero)
            {
                PlayShootingAnimation();
                if (Time.time >= _lastFireTime + _fireCooldown)
                {
                    FireBulletXbox(aimDirection);
                    _lastFireTime = Time.time; // Updates the last fire time
                }
            }
            else
            {
                StopShootingAnimation();
            }
        }

        // Handles the rotation of the player based on movement or aiming direction
        HandlePlayerRotation();
    }

    // Rotates the player to face the movement or aiming direction
    void HandlePlayerRotation()
    {
        Vector3 direction;
        if (aimDirection != Vector3.zero)
        {
            direction = aimDirection.normalized;
        }
        else
        {
            direction = _playerMovement;
        }

        direction.y = 0; // Keeps the player's rotation on the horizontal plane to avoid tilting
        Quaternion newRotation = Quaternion.LookRotation(direction); // Creates a new rotation to face the target
        transform.rotation = newRotation; // Applies the new rotation to the player
    }

    // Plays the shooting animation if it is not already playing
    void PlayShootingAnimation()
    {
        if (!animation.IsPlaying("ClaireIdleFiring"))
        {
            Debug.Log("Shooting");
            animation.Play("ClaireIdleFiring");
        }
    }

    // Stops the shooting animation if it is currently playing
    void StopShootingAnimation()
    {
        if (animation.IsPlaying("ClaireIdleFiring"))
        {
            Debug.Log("Not Shooting");
            animation.Stop("ClaireIdleFiring");
        }
    }

    // Determines the direction based on input from the right stick
    Vector3 DetermineDirection(Vector2 _stickInput)
    {
        // If the input magnitude is below the threshold, returns zero vector which means no direction
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
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPosition.position, Quaternion.identity);

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
