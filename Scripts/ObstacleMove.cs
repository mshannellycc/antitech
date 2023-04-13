using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMove : MonoBehaviour
{
    private float obstacleSpeed;
    MarksRoads roadManager;

    private void Awake()
    {
        //Base the obstacle's movement speed on the roadSpeed
        roadManager = FindObjectOfType<MarksRoads>();
        obstacleSpeed = roadManager.roadSpeed;
    }

    private void Update()
    {
        //Always tracking the roadSpeed, so we only have to change it in one spot
        obstacleSpeed = roadManager.roadSpeed;
        transform.position -= Vector3.forward * obstacleSpeed * Time.deltaTime;

        //If it gets past -70 on z, go away.
        if (transform.position.z < -70f)
        {
            Destroy(gameObject);
        }
    }

  
}
