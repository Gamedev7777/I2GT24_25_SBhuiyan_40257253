using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    // private variables
    [SerializeField] private float speed;
    [SerializeField] private float horsePower = 0;
    [SerializeField] private GameObject centerOfMass;
    [SerializeField] private TextMeshProUGUI speedometerText;
    [SerializeField] private TextMeshProUGUI rpmText;
    [SerializeField] private float rpm;
    private const float _turnSpeed = 45.0f;
    private float _horizontalInput;
    private float _verticalInput;
    private Rigidbody _playerRb;

    // Start is called before the first frame update
    void Start()
    {
        _playerRb = GetComponent<Rigidbody>();
        _playerRb.centerOfMass = centerOfMass.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // This is where we get player input
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        // Move the vehicle forward or backward
        //transform.Translate(Vector3.forward * Time.deltaTime * speed * forwardInput);
        _playerRb.AddRelativeForce(Vector3.forward * _verticalInput * horsePower);
        // Turn the vehicle left or right
        transform.Rotate(Vector3.up, _turnSpeed * _horizontalInput * Time.deltaTime);
        speed = Mathf.RoundToInt(_playerRb.velocity.magnitude * 2.237f); // This shows speed in miles per hour
        speedometerText.SetText("Speed: " + speed + "mph");
        rpm = Mathf.RoundToInt((speed % 30) * 40);
        rpmText.SetText("RPM: " + rpm);
    }
}
