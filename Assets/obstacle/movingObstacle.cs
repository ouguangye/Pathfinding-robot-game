using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingObstacle : MonoBehaviour
{
    public Vector3 endPoint;
    
    public float speed = 3f;

    private Vector3 start;
    private Vector3 end;
    private Vector3 temp = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        start = transform.position;
        end = endPoint;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(start + " " + end);
        // Debug.Log("aaa: " + Vector3.MoveTowards(transform.position, end,speed * Time.deltaTime));
        transform.position = Vector3.MoveTowards(transform.position, end,speed * Time.deltaTime);
        if(transform.position == end) {
            // Debug.Log("moving obstacle reach the end");
            temp = start;
            start = end;
            end = temp;
            // Debug.Log("point: " + start+ " " + end);
        }
    }
}
