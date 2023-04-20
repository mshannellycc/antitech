using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorationHandler : MonoBehaviour
{

    GameObject[] LoadedDecor;
    MarksRoads roadMan;


    //public List<GameObject> spawnedObstacles = new List<GameObject>();
    public Renderer roadRenderer;
    public Vector3 roadRendererSize;
    int numDecor = GameObject.FindGameObjectsWithTag("Decor").Length;
    public float roadLength, roadWidth;

    public float numDecorations;

    public Vector3 offset = new Vector3 (10, 0, 0);

    public float buildingSpacing = 100f;
    public float numBuildingsPerSide = 5f;

    private GameObject decorFolder;
    public int maxDecor;
    //GameObject[] currentProps;

    void Awake()
    {
        LoadedDecor = Resources.LoadAll<GameObject>("Decorations");
        //roadMan  = GameObject.Find("GameManager").GetComponent<MarksRoads>();
        roadRenderer = GetComponent<Renderer>();

        roadRendererSize = roadRenderer.bounds.size;
        roadLength = roadRendererSize.z;
        roadWidth = roadRendererSize.x;
        decorFolder = GameObject.Find("DecorFolder");
       


    }

    // Start is called before the first frame update
    private void Start()
    {
        Vector3 leftEdge = transform.position - transform.right * roadWidth;
        Vector3 rightEdge = transform.position + transform.right * roadWidth;
        //currentProps = GameObject.FindGameObjectsWithTag("Decor");
        float xLeft = leftEdge.x + buildingSpacing / 2f;
        float xRight = rightEdge.x - buildingSpacing / 2f;

        

        for (int i = 0; i < numBuildingsPerSide; i++)
        {
            int rand = Random.Range(0, LoadedDecor.Length);

            GameObject go = LoadedDecor[rand];

            Collider propCollider = go.GetComponentInChildren<Collider>();

            //Store the size of the collider
            Vector3 colliderSize = propCollider.bounds.size;

            // Instantiate building on left side
            Vector3 buildingPos = new Vector3(xLeft - 8, 0f, leftEdge.z + i * buildingSpacing);
            if (Physics2D.OverlapCircle(go.transform.position, colliderSize.x + 100f) == null && numDecor < maxDecor)
            {
                Instantiate(go, buildingPos, Quaternion.identity, decorFolder.transform);
               // spawnedObstacles.Add(go);
            }
            else
            {
                return;
            }



            // Instantiate building on right side
            buildingPos = new Vector3(xRight + 8, 0f, rightEdge.z - i * buildingSpacing);
            if (Physics2D.OverlapCircle(go.transform.position, colliderSize.x + 100f) == null && numDecor < maxDecor)
            {
                Instantiate(go, buildingPos, Quaternion.identity, decorFolder.transform);
               // spawnedObstacles.Add(go);
            }
            else
            {
                return;
            }
        }

         numDecor = GameObject.FindGameObjectsWithTag("Decor").Length;
    }



}
