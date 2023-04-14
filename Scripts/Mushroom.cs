using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    ScoreManager Manager;


    // Start is called before the first frame update
    void Start()
    {
        Manager = GameObject.Find("GameManager").GetComponent<ScoreManager>();
        if (Manager == null)
        {
            Debug.LogError("Could not find scoremanager!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Add an ObstacleMove Script to mushroom
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Manager.addlives(1);
            Destroy(this.gameObject);
        }
       
    }

}
