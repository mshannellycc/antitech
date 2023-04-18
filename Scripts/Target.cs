using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    //My EPITHET IS
    public GameObject BARRIER;

    // Start is called before the first frame update
    void Start()
    {
    // make the barrier the child object by default here
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouse = Input.mousePosition;


        mouse = Camera.main.ScreenToWorldPoint(mouse);

        if(mouse.x == transform.position.x && mouse.y == transform.position.y)
        {
            Destroy(BARRIER);
        }
    }
}
