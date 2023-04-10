using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;

public class BallPhysics : MonoBehaviour
{
    public float speed = 20f;
    public float boost = 1f;
    private Rigidbody rigidbody;

    Transform tCamera;



    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        tCamera = GameObject.Find("Main Camera").transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        Vector3 forward = tCamera.forward;
        Vector3 right = tCamera.right;

        if (Input.GetKey(KeyCode.Space))
        {
            boost = 2f;
        }
        else 
        {
            boost = 1f;
        }

        if(Input.GetKey(KeyCode.UpArrow))
        {
            Debug.Log("Move Up");
            rigidbody.AddForce(forward * speed * boost);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            Debug.Log("Move Down");
            rigidbody.AddForce(-forward * speed * boost);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Debug.Log("Move Left");
            rigidbody.AddForce(-right * speed * boost);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            Debug.Log("Move Right");
            rigidbody.AddForce(right * speed * boost);
        }
    }
}
