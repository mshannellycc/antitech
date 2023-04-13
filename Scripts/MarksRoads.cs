using System.Collections.Generic;
using UnityEngine;

public class MarksRoads : MonoBehaviour
{
    public GameObject roadPrefab;
    public int initialRoadCount = 20;

    public List<GameObject> roadObjects = new List<GameObject>();
    private float playerDistance = 0f;

    public Renderer roadRenderer;
    public Vector3 roadRendererSize;
    public float roadLength;

    public float roadSpeed = 5f;

    public GameObject roadFolder;

    private GameObject player;

    public int roadsPassed = 0;

    private void Start()
    {
        player = GameObject.Find("Player");
        roadFolder = GameObject.Find("RoadsFolder");

        // Create initial road objects
        for (int i = 0; i < initialRoadCount; i++)
        {
            Vector3 position = new Vector3(0f, 0f, i * roadLength);
            GameObject road = Instantiate(roadPrefab, position, Quaternion.identity, roadFolder.transform);
            road.name = "Basic Road";
            roadObjects.Add(road);

            //To get length of a road, and also size!
            roadRenderer = roadPrefab.GetComponent<Renderer>();
            roadRendererSize = roadRenderer.bounds.size;
            roadLength = roadRendererSize.z;

            //Make sure the first two roads don't spawn obstacles.
            //If it's at least the third road, call SpawnObstacle() on this road piece.
            if (i > 1)
            {
                road.GetComponent<PropLogic>().SpawnObstacle();
            }
        }
    }

    private void Update()
    {
        // Update player distance based on position.
        //This is a bit redundant since the player never moves, but it's here if we want to get crazy
        playerDistance = player.transform.position.z;

        // Move road objects towards the camera
        foreach (GameObject road in roadObjects)
        {
            Vector3 position = road.transform.position;
            position.z -= roadSpeed * Time.deltaTime;
            road.transform.position = position;
        }

        //If any of the road pieces get past wherever the player is -50, destroy them, remove them from the List and increase the counter
        //The counter is there to know when to create more roads.
        for (int i = 0; i < roadObjects.Count; i++)
        {
            GameObject road = roadObjects[i];

            if (road.transform.position.z < playerDistance - 50f)
            {
                Destroy(road);
                roadObjects.Remove(road);
                roadsPassed++;
            }
        }

        

        //This gets weirdly recursive if I make it roadsPassed % 5 == 0
        //After the player has successfully cleared 5 roads, make 10 more
        if (roadsPassed == 5)
        {
            for (int i = 0; i < 10; i++)
            {
                CreateNewRoads();
            }

            roadsPassed = 0;
        }
    }

    void CreateNewRoads(int prefabIndex = -1)
    {
        GameObject road;

        //Kind of redundant, but prevents weirdness if the object is outside of the List at any point.
        //If it is, pick a new road, if it isn't, just Instantiate the one it was given.
        if (prefabIndex == -1)
        {
            //TO-DO - this will have to change based on theme. Wrap this in an if statement to see which theme is active!
            //And also if we want variations of the roads with different aspects (streetlights, etc.)
            road = Instantiate(roadObjects[Random.Range(0, roadObjects.Count)], roadFolder.transform) as GameObject;
        }
        else
        {
            road = Instantiate(roadObjects[prefabIndex], roadFolder.transform) as GameObject;
        }

        // Calculate the total length of the road objects
        float totalRoadLength = (roadObjects.Count * roadLength) - roadLength; 



        //Place the new road at the end of that total length
        //Vector3.forward (0,0,1) * whatever that is.
        //This allows for the number of initial roads to change at any point.
        road.transform.position = Vector3.forward * totalRoadLength;
        road.name = "Basic Road";

        roadObjects.Add(road);
    }
}