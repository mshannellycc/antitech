using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyFirstScript : MonoBehaviour
{
    // On creation
    void Awake()
    {
        Debug.Log("Awake");
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
    }

    // On enabled
    void OnEnable()
    {
        Debug.Log("On Enable");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Update");
    }

    void FixedUpdate()
    {
        Debug.Log("Fixed Update");
    }

    void LateUpdate()
    {
        Debug.Log("Late Update");
    }

}
