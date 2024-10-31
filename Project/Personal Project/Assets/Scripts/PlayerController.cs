using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float _playerspeed = 10.0f;
    private float _horizontalMovement;
    private float _verticalMovement;
    private Vector3 _playerMovement;
    public GameObject bulletPrefab;
    private float _bulletSpeed = 50.0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        _horizontalMovement = Input.GetAxis("Horizontal");
        _verticalMovement = Input.GetAxis("Vertical");

        _playerMovement = new Vector3(_horizontalMovement, 0, _verticalMovement).normalized;

        transform.Translate(_playerspeed * Time.deltaTime * _playerMovement, Space.World);
        if (Input.GetMouseButtonDown(0))
        {
            FireBullet();
        }
    }
    void FireBullet()
    {
        // Instantiate the bullet at the player's position
        GameObject bullet = Instantiate(bulletPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        
        // Get the Rigidbody component
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            // Convert mouse position to world position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Perform a raycast to determine where the mouse cursor intersects with the game world
            if (Physics.Raycast(ray, out hit))
            {
                // Calculate direction towards the mouse cursor position
                Vector3 direction = (hit.point - transform.position).normalized;
                bulletRb.velocity = direction * _bulletSpeed; // Move the bullet towards the target
            }
        }
        
        // Destroy the bullet after 1 second to avoid clutter
        Destroy(bullet, 1.0f);
    }
}