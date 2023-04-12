using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewObstacleManager : MonoBehaviour
{
    private GameObject[] loadedObstacles;

    private float leftOver = 0f;

    private List<GameObject> Obstacles;
    // Transform of the currne Obstacle's corners

    //Variable to represent the rotation point of current obstacle

    // Start is called before the first frame update

    [SerializeField]
    private int numberOfObstacles = 10;

    bool GasEnabled = false;

    public ScoreManager GasManage;

    public GameObject[] GasObstacle;
    private ulong tempscore2;
    [SerializeField]
    private float distInterval = 30f;

    [SerializeField]
    private string hardCodedPiecesName = "";

    [SerializeField]
    private float ObstacleSpeed = 20f;

    private void Awake()
    {
        // Load all files of type GameObject from the RoadPieces folder
        loadedObstacles = Resources.LoadAll<GameObject>("Obstacles");

        //placeobstacles will be subcribed to roadmanager

        RoadManager.Instance.OnAddPiece += placeobstacles;

        // Initialize list in memory
        // Obstacles = new List<GameObject>();

        // Hard-code the first two road pieces
        // Obstacles.Add(Instantiate(Resources.Load("Obstacles/" + hardCodedPiecesName) as GameObject));
        // Obstacles.Add(Instantiate(Resources.Load("Obstacles/" + hardCodedPiecesName) as GameObject));

        // for (int i = 2; i < numberOfObstacles; i++)
        // {
        //AddObstacle();
        //}

        // parent the first road piece to the second
        // Obstacles[0].transform.parent = Obstacles[1].transform;

        // Move the road past the first piece
        // float roadLength = (Obstacles[0].transform.Find("BeginLeft").position - Obstacles[0].transform.Find("EndLeft").position).magnitude;
        //Obstacles[0].transform.Translate(0f, 0f, -roadLength / 2, Space.World);

        //SetCurrentObstacle();
    }

    // Update is called once per frame

    private void Update()
    {
        tempscore2 += (ulong)(Time.deltaTime * GasManage.scoreRate * (RoadManager.Instance.roadSpeed / 10));
       if (tempscore2 / 5000 >= 1)
        {
            GasEnabled = true;
        }



        if (GasEnabled)
        {
            //loadedObstacles.Length += GasObstacle.Length;
        }
    }


    private void placeobstacles(GameObject onroadpiece)
    {
        Transform beginLeft = onroadpiece.transform.Find("BeginLeft");
        Transform endLeft = onroadpiece.transform.Find("EndLeft");
        Transform beginRight = onroadpiece.transform.Find("BeginRight");
        Transform endRight = onroadpiece.transform.Find("EndRight");

        float roadpiecelength;

        Vector3 rotationpoint = Vector3.zero;

        float radius = 0f;

        if (onroadpiece.CompareTag(Tags.straightPiece))
        {
            roadpiecelength = Vector3.Distance(endLeft.position, beginLeft.position);
        }
        else
        {
            // Vector3 endEdge = endRight.position - endLeft.position;
            rotationpoint = RoadManager.Instance.GetRotationPoint(beginLeft, beginRight, endLeft, endRight);

            //Calculate the angle of our road piece in relation to the global x axis
            //float angle = Vector3.Angle(Vector3.right, endEdge);

            // Get the radius of our rotation point so we have a partial circle
            radius = Vector3.Distance(onroadpiece.transform.position, rotationpoint);

            float angle = Vector3.Angle(beginLeft.position - beginRight.position, endLeft.position - endRight.position);

            // convert angle to radians and calculate angular velocity - return total
            roadpiecelength = radius * angle * Mathf.Deg2Rad;
        }

        float halfroadlength = roadpiecelength / 2f;

        float curDistance = distInterval - halfroadlength - leftOver;

        if (curDistance >= halfroadlength)
        {
            leftOver += roadpiecelength;
        }

        for (; curDistance < halfroadlength; curDistance += distInterval)
        {
            GameObject obstaclePlace = new GameObject("ObstaclePlace");
            obstaclePlace.transform.position = onroadpiece.transform.position;
            obstaclePlace.transform.rotation = onroadpiece.transform.rotation;
            obstaclePlace.transform.Rotate(90f, 0f, 0f);
            obstaclePlace.transform.parent = onroadpiece.transform;

            //for preventing 3 in a row
            //int sameObCount = 0; <-------- not being used anymore

            //Get lane count from playerController
            int lanes = playerController.Instance.NumLanes;
           
            //Generate random between lane count / 2 on the left, and lane count / 2 on the right (if 3 lanes, 1.5 lane to the left, 1.5 on the right)
            //This is to pick a random spot
            int randomLane = Random.Range(-lanes / 2, lanes / 2);

            //choose the random obstacles
            int randomObstacle = Random.Range(0, loadedObstacles.Length);

            //Instantiate the random obstacle in that new transform (replaces obstacleRow)
            GameObject obstacle = Instantiate(loadedObstacles[randomObstacle], obstaclePlace.transform.position, obstaclePlace.transform.rotation, obstaclePlace.transform);

            //Instead of multiplying Vector3.right * whatever iteration of the loop we're on, multiply it by that random int
            obstacle.transform.Translate(Vector3.right * randomLane * playerController.Instance.Lanewidth, Space.Self);

            if (onroadpiece.CompareTag("StraightPiece"))
            {
                obstaclePlace.transform.Translate(0f, 0f, curDistance);
            }
            else
            {
                float Angle = curDistance / radius;
                //localscale is the number represents the strech, usually.
                //used because the direction influences it

                obstaclePlace.transform.RotateAround(rotationpoint, Vector3.up, Angle * Mathf.Rad2Deg * -Mathf.Sign(onroadpiece.transform.localScale.x));
            }

            leftOver = halfroadlength - curDistance;
        }
    }
}