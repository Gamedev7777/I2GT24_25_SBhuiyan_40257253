using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float Speed = 3.0f;
    private Rigidbody _enemyRb;
    private GameObject _player;
    // Start is called before the first frame update
    void Start()
    {
        _enemyRb = GetComponent<Rigidbody>();
        _player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        _enemyRb.AddForce((_player.transform.position - transform.position).normalized * Speed);
    }
}
