using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _playerRb;
    private Animator _playerAnim;
    public ParticleSystem explosionParticle;
    public float jumpForce = 10;
    public float gravityModifier;
    public bool isOnGround = true;
    public bool gameOver;

    // Start is called before the first frame update
    void Start()
    {
        _playerRb = GetComponent<Rigidbody>();
        _playerAnim = GetComponent<Animator>();
        Physics.gravity *= gravityModifier;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isOnGround && !gameOver)
        {
           _playerRb.AddForce(Vector3.up * 10, ForceMode.Impulse);
           isOnGround = false;
           _playerAnim.SetTrigger("Jump_trig");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {


        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {       
            Debug.Log("Game Over!");
            gameOver = true;
            _playerAnim.SetBool("Death_b", true);
            _playerAnim.SetInteger("DeathType_int",1);
            explosionParticle.Play();
        }
    }
        
}
