using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    //My EPITHET IS
    public GameObject BARRIER;
    public Renderer roadRenderer;
    public Vector3 roadRendererSize;

    // Start is called before the first frame update
    void Start()
    {
        // make the barrier the child object by default here
        roadRenderer = gameObject.GetComponent<Renderer>();
        roadRendererSize = roadRenderer.bounds.size;
       
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouse = Input.mousePosition;


        mouse = Camera.main.ScreenToWorldPoint(mouse);

        // this if statement uses the position - the renderer size so that you can click anywhere on the target
        //i'm doing /2 because the size is probably the size of the whole thing and we want the size from the middle to the edge
        if(mouse.x > transform.position.x - roadRendererSize.x/2 && mouse.x < transform.position.x + roadRendererSize.x/2 && mouse.y < transform.position.y + roadRendererSize.y / 2 && mouse.y > transform.position.y - roadRendererSize.y / 2)
        {
            Destroy(BARRIER);
        }
    }
}
