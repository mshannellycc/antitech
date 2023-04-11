using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OsbtacleManager : MonoBehaviour
{

    GameObject[] loadedObstacles;

    float leftOver = 0f;


    List<GameObject> Obstacles;
    // Transform of the currne Obstacle's corners
  
    //Variable to represent the rotation point of current obstacle
  
    // Start is called before the first frame update

    [SerializeField]
    int numberOfObstacles = 10;
    [SerializeField]
    float distInterval = 30f;
    [SerializeField]
    string hardCodedPiecesName = "";
    [SerializeField]
    float ObstacleSpeed = 20f;
    void Awake()
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



                GameObject obstacleRow = new GameObject("ObstacleRow");
                obstacleRow.transform.position = onroadpiece.transform.position;
                obstacleRow.transform.rotation = onroadpiece.transform.rotation;
                //
                obstacleRow.transform.Rotate(90f, 0f, 0f);
                obstacleRow.transform.parent = onroadpiece.transform;


                int sameObCount = 0;

                int lanes = playerController.Instance.NumLanes;
                //initalize
                


                for(int i = -lanes/2; i <= lanes/2; i ++)
                {
                    int randomObstacle = Random.Range(0, loadedObstacles.Length);
                    //prevent
                    if(this.loadedObstacles[randomObstacle].CompareTag(Tags.wall))
                    {
                        if(++sameObCount >= lanes)
                        {
                            randomObstacle += 3;
                            //not out of range
                            randomObstacle %= loadedObstacles.Length;


                        }
                    }

                    GameObject obstacle = Instantiate(loadedObstacles[randomObstacle], obstacleRow.transform.position, obstacleRow.transform.rotation, obstacleRow.transform);

                    obstacle.transform.Translate(Vector3.right*i*playerController.Instance.Lanewidth,Space.Self);

                }

                if (onroadpiece.CompareTag("StraightPiece"))
                {
                    obstacleRow.transform.Translate(0f, 0f, curDistance);
                }else
                {
                    float Angle = curDistance / radius;
                    //localscale is the number represents the strech, usually.
                    //used because the direction influences it

                    obstacleRow.transform.RotateAround(rotationpoint, Vector3.up, Angle*Mathf.Rad2Deg * -Mathf.Sign(onroadpiece.transform.localScale.x));
                }


                leftOver = halfroadlength - curDistance;



            }
        }



    }


