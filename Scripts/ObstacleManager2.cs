using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{

    //Load in our obstacles inside of an array
    GameObject[] loadedObstacles;

    // Unused distance from previous piece
    float leftoverDistance = 0;

    // Inspector Parameters
    [SerializeField]
    float distanceInterval = 30f;
    //Obstacles we be spaced out every 30 meters

    // Start is called before the first frame update
    void Awake()
    {
        //Load in Obstacle Prefabs from our "Obstacle"s folder inside the main "Resources" folder
        loadedObstacles = Resources.LoadAll<GameObject>("Obstacles");

        //We are communicating to RoadManager that when the OnAddPiece() event occurs, we want the PlaceObstacles() method to be subscribed to it
        RoadManager.Instance.OnAddPiece += PlaceObstacles;
    }

    //Our method that runs every time our OnAddPiece() Event from RoadManager gets called
    private void PlaceObstacles(GameObject onRoadPiece)
    {
        // We need to get child objects of our road piece, so we can use transform.find
        Transform beginLeft = onRoadPiece.transform.Find("BeginLeft");
        Transform beginRight = onRoadPiece.transform.Find("BeginRight");
        Transform endLeft = onRoadPiece.transform.Find("EndLeft");
        Transform endRight = onRoadPiece.transform.Find("EndRight");

        // Get new road piece length
        float roadPieceLength;
        Vector3 rotationPoint = Vector3.zero;
        float radius = 0f;

        //using tags, we can check if we have a straight or curved road piece
        if (onRoadPiece.tag == Tags.straightPiece)
        {
            roadPieceLength = Vector3.Distance(beginLeft.position, endLeft.position);
        }
        else
        {
            // Get radius or rotation point of our road piece -- similar to how we got the road piece to move!
            rotationPoint = RoadManager.Instance.GetRotationPoint(beginLeft, beginRight, endLeft, endRight);
            // calculating the radius
            radius = Vector3.Distance(onRoadPiece.transform.position, rotationPoint);

            // Get angle
            float angle = Vector3.Angle(beginLeft.position - beginRight.position, endLeft.position - endRight.position);

            //we have to make sure we are using radians instead of degrees in order to get an accurate roadPieceLength
            roadPieceLength = radius * angle * Mathf.Deg2Rad;
        }

        //Calculating half the road length
        float halfRoadLength = roadPieceLength / 2f;
        float curDistance = distanceInterval - halfRoadLength - leftoverDistance;
        if (curDistance >= halfRoadLength)
        {
            leftoverDistance += halfRoadLength * 2f;
        }

        //
        for (; curDistance < halfRoadLength; curDistance += distanceInterval)
        {
            // Obstacle container - Instantiate a GameObject to house our Obstacle Prefabs
            GameObject obstacleRow = new GameObject("ObstacleRow");
            obstacleRow.transform.position = onRoadPiece.transform.position;
            obstacleRow.transform.rotation = onRoadPiece.transform.rotation;
            // compensate for road piece rotation
            obstacleRow.transform.Rotate(90f, 0f, 0f);
            obstacleRow.transform.parent = onRoadPiece.transform;

            //number of lanes we have
            int numberOfLanes = playerController.Instance.NumLanes;



            // We need to check for 3 of the same type of obstacle in a row, or consecutive obstacles
            int sameObstacleCount = 0;

            //we need to add obstacles to every lane in our road so we will need a for loop
            for (int i = numberOfLanes / -2; i <= numberOfLanes / 2; i++)
            {
                //Choose a random obstacle from our loadedObstacles array
                int randomObstacle = Random.Range(0, loadedObstacles.Length);

                //Prevent 3 of the same type of obstacle in a row
                if (this.loadedObstacles[randomObstacle].CompareTag(Tags.wall))
                {
                    if (++sameObstacleCount >= numberOfLanes)
                    {
                        randomObstacle += 3;
                        //make sure to change our randomObstacle to the next one - if we have reached the end of our array, go to start
                        randomObstacle %= loadedObstacles.Length;
                    }
                }
                // Instantiate obstacle prefab
                GameObject obstacle = Instantiate(loadedObstacles[randomObstacle], obstacleRow.transform.position,
                                                   obstacleRow.transform.rotation, obstacleRow.transform);
                //Move obstacle into correct lane - since obstacle is in ObstacleRow this means we can move in local space
                obstacle.transform.Translate(Vector3.right * i * playerController.Instance.Lanewidth, Space.Self);
            }

            if (onRoadPiece.CompareTag(Tags.straightPiece))
            {
                //Translate or slide our obstacles along the z axis
                obstacleRow.transform.Translate(0f, 0f, curDistance);
            }
            else
            {
                float angle = curDistance / radius;
                //much like how we rotated our roads, we need to make sure our obstacles rotate around the angle of the road
                obstacleRow.transform.RotateAround(rotationPoint, Vector3.up, angle * Mathf.Rad2Deg * -Mathf.Sign(onRoadPiece.transform.localScale.x));
            }

            leftoverDistance = halfRoadLength - curDistance;
        }
    }
}
