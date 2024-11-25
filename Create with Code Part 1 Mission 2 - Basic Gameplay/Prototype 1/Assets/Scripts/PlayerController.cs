using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // private variables
    //private float _speed = 10.0f;
    [SerializeField] private float _horsePower = 0;
    [SerializeField] private GameObject centerOfMass;
    private float _turnSpeed = 45.0f;
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
        _playerRb.AddRelativeForce(Vector3.forward * _verticalInput * _horsePower);
        // Turn the vehicle left or right
        transform.Rotate(Vector3.up, _turnSpeed * _horizontalInput * Time.deltaTime);
    }
}
