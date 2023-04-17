using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorationHandler : MonoBehaviour
{

    GameObject[] LoadedDecor;


    void Awake()
    {
        LoadedDecor = Resources.LoadAll<GameObject>("Decoration");

        //add Event that can tell when marksroads places a road here, mark
    }
    // Start is called before the first frame update
    private void PlaceDecor(GameObject RoadPiece)
    {
        int laneIndex = Random.Range(0, 1);


        Transform beginLeft = RoadPiece.transform.Find("BeginLeft");
        Transform endLeft = RoadPiece.transform.Find("EndLeft");
        Transform beginRight = RoadPiece.transform.Find("BeginRight");
        Transform endRight = RoadPiece.transform.Find("EndRight");


        if(laneIndex == 0)
        {
            //place to the left of the road

           

                 int randomObstacle = Random.Range(0, LoadedDecor.Length);

            GameObject Decor = Instantiate(LoadedDecor[randomObstacle], new Vector3(RoadPiece.transform.position.x, beginLeft.transform.position.y - endLeft.transform.position.y, RoadPiece.transform.position.z), RoadPiece.transform.rotation, RoadPiece.transform);


        }
        else if( laneIndex == 1)
        {
            int randomObstacle = Random.Range(0, LoadedDecor.Length);

            GameObject Decor = Instantiate(LoadedDecor[randomObstacle], new Vector3(RoadPiece.transform.position.x, beginRight.transform.position.y - endRight.transform.position.y, RoadPiece.transform.position.z), RoadPiece.transform.rotation, RoadPiece.transform);


            //place to the right of the road
        }
    }
}
