using UnityEngine;

public class PlayerMultiplayer2 : MonoBehaviour
{
    // public variables
    public float playerSpeed = 6.0f; // Speed at which the player moves
    public GameObject bulletPrefab; // Bullet prefab used for shooting
    public Transform bulletSpawnPosition; // Position from where the bullet will spawn
    public GameObject claireShield; // Shield for the player, not currently used in this script
    public AudioClip fireSound; // Sound effect played when firing

    // private variables
    private float _horizontalMovement; // Horizontal movement input
    private float _verticalMovement; // Vertical movement input
    private Vector3 _playerMovement; // Player movement vector
    private float _bulletSpeed = 30.0f; // Speed at which the bullet moves
    private float _fireThreshold = 0.1f; // Threshold value for firing
    private float _fireCooldown = 0.2f; // Cooldown time between firing bullets
    private float _lastFireTime; // Tracks the time of the last fired bullet
    private Animation animation; // Player animation component
    private Vector3 _localMovement; // Player movement in local space

    void Start()
    {
        // Gets the Animation component attached to the player's child object
        animation = transform.GetChild(0).GetComponent<Animation>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PlayerPrefs.GetInt("Cutscene", 1) == 0)
        {
            // Gets player input for horizontal movement (left/right)
            _horizontalMovement = Input.GetAxis("HorizontalPlayer2Xbox");
            // Gets player input for vertical movement (up/down)
            _verticalMovement = Input.GetAxis("VerticalPlayer2Xbox");

            // Creates a new movement vector based on the player's input
            _playerMovement = new Vector3(_horizontalMovement, 0, _verticalMovement).normalized;
            _localMovement = transform.TransformDirection(_playerMovement);
            // Moves the player based on the input and playerSpeed
            transform.GetComponent<Rigidbody>().velocity = _localMovement * playerSpeed;

            // Gets input from the right stick for aiming direction
            float rightStickHorizontal = Input.GetAxis("RightStickHorizontal");
            float rightStickVertical = Input.GetAxis("RightStickVertical");
            Vector3 aimDirection = DetermineDirection(new Vector2(rightStickHorizontal, rightStickVertical));
            Vector3 worldAimDirection = transform.TransformDirection(aimDirection);

            // Determines if the player is both moving and aiming, and handles animation and firing
            if ((_horizontalMovement != 0 && _verticalMovement != 0) && (aimDirection != Vector3.zero))
            {
                // Keeps the aim direction aligned on the horizontal plane
                worldAimDirection.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(worldAimDirection);
                // Rotates the player towards the aim direction
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 180);

                // Checks if the speed power-up is active and plays appropriate animation
                if (PlayerPrefs.GetInt("SpeedPowerUp", 0) == 1)
                {
                    animation.Play("ClaireRunningFiring"); // Plays running while firing animation
                }
                else
                {
                    animation.Play("ClaireWalkingFiring"); // Plays walking while firing animation
                }

                // Fires a bullet if aiming direction is provided and cooldown time has passed
                if (Time.time >= _lastFireTime + _fireCooldown)
                {
                    FireBulletXbox(worldAimDirection);
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
                        animation.Play("ClaireRunning"); // Plays running animation
                    }
                    else
                    {
                        animation.Play("ClaireWalking"); // Plays walking animation
                    }
                }
                else
                {
                    // Player is idle
                    animation.Play("ClaireIdle"); // Plays idle animation
                }

                // Handles firing when the player is stationary or just aiming
                if (aimDirection != Vector3.zero)
                {
                    // Keeps the aim direction aligned on the horizontal plane
                    worldAimDirection.y = 0;
                    Quaternion targetRotation = Quaternion.LookRotation(worldAimDirection);
                    // Rotates the player towards the aim direction
                    transform.rotation =
                        Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 360);

                    // Plays shooting animation if aiming
                    PlayShootingAnimation();
                    if (Time.time >= _lastFireTime + _fireCooldown)
                    {
                        FireBulletXbox(worldAimDirection);
                        _lastFireTime = Time.time; // Updates the last fire time
                    }
                }
                else
                {
                    // Stops shooting animation if not aiming
                    StopShootingAnimation();
                }
            }
        }
        else
        {
            animation.Play("ClaireIdle"); // Plays idle animation
        }
    }

    // Plays the shooting animation if it is not already playing
    void PlayShootingAnimation()
    {
        if (!animation.IsPlaying("ClaireIdleFiring"))
        {
            Debug.Log("Shooting"); // Logs shooting status
            animation.Play("ClaireIdleFiring"); // Plays idle firing animation
        }
    }

    // Stops the shooting animation if it is currently playing
    void StopShootingAnimation()
    {
        if (animation.IsPlaying("ClaireIdleFiring"))
        {
            animation.Stop("ClaireIdleFiring"); // Stops idle firing animation
        }
    }

    // Determines the direction based on the input from the right stick
    Vector3 DetermineDirection(Vector2 _stickInput)
    {
        // Checks if the stick input magnitude is below the threshold to ignore minor movements
        if (_stickInput.sqrMagnitude < _fireThreshold * _fireThreshold)
            return Vector3.zero;

        _stickInput.Normalize();
        float angle = Mathf.Atan2(_stickInput.y, _stickInput.x) * Mathf.Rad2Deg;

        // Correcting direction determination to properly detect left direction
        if (angle >= -22.5f && angle < 22.5f) return Vector3.right;
        else if (angle >= 22.5f && angle < 67.5f) return new Vector3(1, 0, 1).normalized;
        else if (angle >= 67.5f && angle < 112.5f) return Vector3.forward;
        else if (angle >= 112.5f && angle < 157.5f) return new Vector3(-1, 0, 1).normalized;
        else if ((angle >= 157.5f && angle <= 180f) || (angle < -157.5f && angle >= -180f)) return Vector3.left;
        else if (angle >= -157.5f && angle < -112.5f) return new Vector3(-1, 0, -1).normalized;
        else if (angle >= -112.5f && angle < -67.5f) return Vector3.back;
        else if (angle >= -67.5f && angle < -22.5f) return new Vector3(1, 0, -1).normalized;

        return Vector3.zero;
    }

    // Fires a bullet in a specific direction based on controller input
    void FireBulletXbox(Vector3 _direction)
    {
        // Plays the fire sound effect at the SpawnManager's position
        AudioSource.PlayClipAtPoint(fireSound, SpawnManager.instance.transform.position, 0.4f);

        // Instantiates the bullet prefab at the player's current position
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPosition.position, bulletSpawnPosition.rotation);

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