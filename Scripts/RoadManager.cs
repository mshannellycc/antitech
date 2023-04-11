using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoadManager : Singleton<RoadManager>
{

    public delegate void AddPieceHandler(GameObject piece);

    public event AddPieceHandler OnAddPiece;
   // This is the array of GameObjects with no predetermined size
   GameObject[] loadedPieces;
    // List of GameObject for instantiated road pieces
    List<GameObject> roadPieces;
    // Transform of the currne road pieces corners
    Transform beginLeft, beginRight, endLeft, endRight;
    //Variable to represent the rotation point of current road piece
    Vector3 rotationPoint = Vector3.zero;


    // The [SerializeField] tag allows us to change a variables from the Inspector, whether or not the variable is public, private, or protected
    [SerializeField]
    int numberOfPieces = 10;
    [SerializeField]
    string hardCodedPiecesName = "Straight60m";
    [SerializeField]
   public float roadSpeed = 20f;


    public int theme = 0; 

    public void changeTheme()
    {
        if (theme > 2)
        {
            theme++;
        }
        else
        {
            theme = 0;
        }
    }
    // Start is called before the first frame update
    void Start()
    {

        OnAddPiece += x => { };
        // Load all files of type GameObject from the RoadPieces folder
        loadedPieces = Resources.LoadAll<GameObject>("RoadPieces");

        // Initialize list in memory
        roadPieces = new List<GameObject>();

        // Hard-code the first two road pieces
        roadPieces.Add(Instantiate(Resources.Load("RoadPieces/" + hardCodedPiecesName) as GameObject));
    
        roadPieces.Add(Instantiate(Resources.Load("RoadPieces/" + hardCodedPiecesName) as GameObject));

       // if (CompareTag(null))

            for (int i = 2; i < numberOfPieces; i++)
        {
            AddPiece();
           
        }

        // parent the first road piece to the second
        roadPieces[0].transform.parent = roadPieces[1].transform;

        // Move the road past the first piece
        float roadLength = (roadPieces[0].transform.Find("BeginLeft").position - roadPieces[0].transform.Find("EndLeft").position).magnitude;
        roadPieces[0].transform.Translate(0f, 0f, -roadLength/2, Space.World);

        SetCurrentRoadPiece();
    
    }

    void Update()
    {
        MoveRoadPieces(roadSpeed * Time.deltaTime);

        //To delete roads that pass the robot
        //step1 - Determine when the parent road peice passes the origin
        if (endLeft.position.z < 0f || endRight.position.z < 0f)
        {
            //Snap current road piece to the x axis
            float resetDistance = GetResetDistance();

           // Debug.Log(GetResetDistance());
            
            //Move road pieces into position
            MoveRoadPieces(-resetDistance);

            CycleRoadPieces();

            //Re-align  road piece to x axis when pieces get deleted
            MoveRoadPieces(resetDistance);

            if (roadPieces[1].CompareTag(Tags.straightPiece)){

                roadPieces[1].transform.rotation = new Quaternion(roadPieces[1].transform.rotation.x,0f,0f,roadPieces[1].transform.rotation.w);
                roadPieces[1].transform.position = new Vector3(0f, 0f, roadPieces[1].transform.position.z);

            }

        }
    }

    private void AddPiece()
    {
        // Generate random number
        int randomIndex = Random.Range(0, loadedPieces.Length);
        // Instantiate random road and add to list
        roadPieces.Add(Instantiate(loadedPieces[randomIndex], roadPieces[roadPieces.Count - 1].transform.position, roadPieces[roadPieces.Count - 1].transform.rotation));

        //if (CompareTag(null))

            // Get references to the last two pieces we are processing - the newest and the previous
            Transform newPiece = roadPieces[roadPieces.Count - 1].transform;
        Transform previousPiece = roadPieces[roadPieces.Count - 2].transform;

        // Move new piece into position by calculating the displacement
        beginLeft = newPiece.Find("BeginLeft");
        beginRight = newPiece.Find("BeginRight");
        endLeft = previousPiece.Find("EndLeft");
        endRight = previousPiece.Find("EndRight");

        // Compute the edges
        Vector3 beginEdge = beginRight.position - beginLeft.position;
        Vector3 endEdge = endRight.position - endLeft.position;

        // Compute angle between edges
        float angle = Vector3.Angle(beginEdge, endEdge) * Mathf.Sign(Vector3.Cross(beginEdge, endEdge).y);

        // Rotate new road
        newPiece.Rotate(0f, angle, 0f, Space.World);

        // Move new road to end of previous road
        Vector3 displacement = endLeft.position - beginLeft.position;

        // Transform.Translate the new piece's GameObject to the new position
        // Space.World reflects the Scene's or Global axis and not the local piece's axis
        newPiece.Translate(displacement, Space.World);

        // Parent the newly created piece to the second road piece
        newPiece.parent = roadPieces[1].transform;

        //pass the event
        OnAddPiece(newPiece.gameObject);

    }

    // Update is called once per frame
   

    private void CycleRoadPieces()
    {
        //step2 - delete the peice behing the oring using GameObject.Destroy() and remove from array
        Destroy(roadPieces[0]);
        roadPieces.RemoveAt(0);

        //step3 - add a new track to the opposite end
        AddPiece();

        //step4 - reparent roads to second indexed peice in array
        for (int i = roadPieces.Count - 1; i >= 0; i--)
        {
            //step5 - unparent GameObjects and reparent them to the second road peice in list
            roadPieces[i].transform.parent = null;
            roadPieces[i].transform.parent = roadPieces[1].transform;
        }
        //step6 - Get the corner markers for current parent road
        SetCurrentRoadPiece();
    }

    private void MoveRoadPieces(float distance)
    {
        // Compute the edges
        Vector3 beginEdge = beginRight.position - beginLeft.position;
        Vector3 endEdge = endRight.position - endLeft.position;

        if (roadPieces[1].CompareTag(Tags.straightPiece))
        {
            roadPieces[1].transform.Translate(0f, 0f, -distance, Space.World);
        }
        else
        {
            float radius = Mathf.Abs(rotationPoint.x);
            float omega = (distance * 180) / (radius * Mathf.PI);

            if (Mathf.Sign(Vector3.Cross(beginEdge, endEdge).y) < 0)
            {
                roadPieces[1].transform.RotateAround(rotationPoint, Vector3.up, omega);
            }
            else
            {
                roadPieces[1].transform.RotateAround(rotationPoint, Vector3.up, -omega);
            }
        }
    }

    public Vector3 GetRotationPoint(Transform beginLeft, Transform beginRight, Transform endLeft, Transform endRight)
    {
        // Compute the two edge vectors
        Vector3 endEdge = endLeft.position - endRight.position;
        Vector3 beginEdge = beginLeft.position - beginRight.position;

        // square magnitude of begin edge
        float a = Vector3.Dot(beginEdge, beginEdge);
        // project begin edge onto end edge
        float b = Vector3.Dot(beginEdge, endEdge);
        // square magnitude of end edge
        float e = Vector3.Dot(endEdge, endEdge);

        // difference between square magnitudes of begin and end minus the square mof ther dot product
        float difference = a * e - b * b;

        // Vector3 between the beginLeft and endLeft position of road piece
        Vector3 r = beginLeft.position - endLeft.position;

        float c = Vector3.Dot(beginEdge, r);
        float f = Vector3.Dot(endEdge, r);
        float s = (b * f - c * e) / difference;
        float t = (a * f - c * b) / difference;

        Vector3 rotationPointBegin = beginLeft.position + beginEdge * s;
        Vector3 rotationPointEnd = endLeft.position + endEdge * t;

        // return midpoint between two closest points
        return (rotationPointBegin + rotationPointEnd)/2f;
    }


    void SetCurrentRoadPiece()
    {
        // Get all 4 corners of start road
        beginLeft = roadPieces[1].transform.Find("BeginLeft");
        beginRight = roadPieces[1].transform.Find("BeginRight");
        endLeft = roadPieces[1].transform.Find("EndLeft");
        endRight = roadPieces[1].transform.Find("EndRight");
        // calculate rotation point
        rotationPoint = GetRotationPoint(beginLeft, beginRight, endLeft, endRight);
    }

    // distance required move piece to align with world x-axis
    public float GetResetDistance()
    {
        //Check to see which type of road piece we are dealing with - through use of Unity Tags
        if (roadPieces[1].CompareTag(Tags.straightPiece))
        {
            //return the position of our straight piece - where it is on the z axis
            return -endLeft.position.z;
        }
        else
        {
            //Get the End Edge of the curved road piece
            Vector3 endEdge = endRight.position - endLeft.position;

            //Calculate the angle of our road piece in relation to the global x axis
            float angle = Vector3.Angle(Vector3.right, endEdge);

            // Get the radius of our rotation point so we have a partial circle
            float radius = Mathf.Abs(rotationPoint.x);

            // convert angle to radians and calculate angular velocity - return total
            return angle * Mathf.Deg2Rad * radius;
        }
    }
}
