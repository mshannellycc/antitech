using UnityEngine;

public class PropLogic : MonoBehaviour
{
    //Possible objects to spawn
    public GameObject[] obstacles;

    //100% chance an obstacle will appear, this can be tweaked for Easy mode or whatever
    private float obstacleProbability = 1f;

    //Need all this to get the size of the road
    private Renderer rend;
    private Vector3 roadRendererSize;
    public float roadWidth;

    //Default is 3 lanes. If you change this, the player controller will have to be updated.
    public int lanes = 3;

    //Need access to the roadManager to 
   
    private GameObject obstacleFolder;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        roadRendererSize = rend.bounds.size;
        roadWidth = roadRendererSize.x;

        //Just for organizing the hierarchy
        obstacleFolder = GameObject.Find("ObstacleFolder");

        //SpawnObstacle();
    }


    public Transform getObstacleFolder()
    {
        return obstacleFolder.transform;
    }


    public void SpawnObstacle()
    {
        //If a random number between 0 and 1 is less than whatever probability is, make that boi
        if (Random.value < obstacleProbability)
        {
            //Pick a random obstacle
            int obstacleIndex = Random.Range(0, obstacles.Length);
            GameObject obs = obstacles[obstacleIndex];

            //Pick a random lane
            int laneIndex = Random.Range(0, lanes);

            //Offset the size of a lane, so you don't have things appearing between lanes
            float laneOffset = (laneIndex - 1) * roadWidth / lanes;

            //Instantiate at current road position + offset * Vector3.right (1,0,0)
            Vector3 obstaclePos = transform.position + laneOffset * Vector3.right;
            Instantiate(obs, obstaclePos, Quaternion.identity, obstacleFolder.transform);
        }
    }
}