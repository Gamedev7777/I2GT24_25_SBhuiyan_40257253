using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    
    
    public Transform pointA, pointB;

    public float time = 2;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = pointA.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position == pointA.position)
        {
            StopAllCoroutines();
            IEnumerator coroutine = Sliding(pointA, pointB);
            StartCoroutine(coroutine);
        }
        if (transform.position == pointB.position)
        {
            StopAllCoroutines();
            IEnumerator coroutine = Sliding(pointB, pointA);
            StartCoroutine(coroutine);
        }
    }

    IEnumerator Sliding(Transform start, Transform end)
    {
       float timer = 0;
       while (timer <= time)
       {
           transform.position = Vector3.Lerp(start.position, end.position, timer / time);
           timer+=Time.deltaTime;
           yield return new WaitForEndOfFrame();
       }
       transform.position = end.position;
    }
    
}
