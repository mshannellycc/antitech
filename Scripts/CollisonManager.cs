using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisonManager : MonoBehaviour
{

    int collisionmask;

    GameObject player;

    Vector3[] slidespheres;

    SkinnedMeshRenderer rend;

    bool invincible = false;

    [SerializeField]
    float blinkrate = 0.2f, blinkDuration = 3f;

    Animator playeranim;

    int lives;
    public int maxlives;

    int SlideCurvepara;

    [SerializeField]
    bool debugspheres = false;

    public delegate void obstcol();
    public event obstcol onObstacleCollision;

    public delegate void Outofliveshandler();
    public event Outofliveshandler outoflives;
    struct CollisonSphere
    {



        public Vector3 offset;

        public float radius;
        public CollisonSphere(Vector3 offset, float radius)
        {
        


            this.offset = offset;
            this.radius = radius;



        }
        /// <summary>
        ///Overloads the operator greater than to take in two CollisionSpheres - returning the comparison between two CollisionSpheres: lhs and rhs.
        /// </summary>
        /// <param name="lhs">The one we're checking</param>
        /// <param name="rhs">The one being compared to</param>
        /// <returns>The result if lhs offset.y value is greater than rhs offset.y</returns>
        public static bool operator >(CollisonSphere lhs, CollisonSphere rhs)
        {
            return lhs.offset.y > rhs.offset.y;
        }
        /// <summary>
        ///Overloads the operator greater than to take in two CollisionSpheres - returning the comparison between two CollisionSpheres: lhs and rhs.
        /// </summary>
        /// <param name="lhs">The one we're checking</param>
        /// <param name="rhs">The one being compared to</param>
        /// <returns>The result if lhs offset.y value is greater than rhs offset.y</returns>

        public static bool operator <(CollisonSphere lhs, CollisonSphere rhs)
        {
            return lhs.offset.y < rhs.offset.y;
        }
        
        
        

    }

    struct CollisonSphereComparer : IComparer
    {
        public int Compare(object A, object B)
        {
           if (!(A is CollisonSphere )||!(B is CollisonSphere))
            {
                Debug.LogError(Environment.StackTrace);

                throw new ArgumentException("Cannot compare collisionsphers to noncollisonspheres");

            }

            CollisonSphere lhs = (CollisonSphere)A;
            CollisonSphere rhs = (CollisonSphere)B;

            if( lhs < rhs)
            {
                return -1;
            }else if (lhs > rhs)
            {
                return 1;
            }else
            {
                return 0;
            }

        }
    }

    CollisonSphere[] collisonSpheres;
    


    // Start is called before the first frame update
    void Start()
    {
        lives = maxlives;

        onObstacleCollision += ObstacleCollision;
        outoflives += Gameover;

        collisionmask = GetlayerMask((int)Layer.Obstacle);

        player = GameObject.Find("Player");

        

        if (!player)
        {
            Debug.LogError("Could not find player GameObject (Searched\"Player\")");
            Destroy(this);
        }

        rend = player.GetComponentInChildren<SkinnedMeshRenderer>();

        playeranim = player.GetComponent<Animator>();

        if (!playeranim)
        {
            Debug.LogError("Could not find player animator under player.");
            Destroy(this);
        }

        SlideCurvepara = Animator.StringToHash("Slide curve");

        if (!rend)
        {
            Debug.LogError("Could not find player renderer GameObject");
            Destroy(this);
        }
        SphereCollider[] colliders = player.GetComponents<SphereCollider>();

        collisonSpheres = new CollisonSphere[colliders.Length];

        for(int i = 0; i < colliders.Length; i++)
        {
            collisonSpheres[i].offset = colliders[i].center;
            collisonSpheres[i].radius = colliders[i].radius;
        }

        Array.Sort(collisonSpheres, new CollisonSphereComparer());


        slidespheres = new Vector3[4];
        slidespheres[0] = new Vector3(-0.02f, 0.15f, 0.61f);
        slidespheres[1] = new Vector3(-0.12f,0.3f,0.1f);
        slidespheres[2] = new Vector3(0.14f, 0.46f, -0.18f);
        slidespheres[3] = new Vector3(0.42f, 0.64f, -0.21f);
    }

    public void Addlives(int Lives)
    {
        if( lives + Lives >= maxlives + 1)
        {
            lives += Lives;
        }
    }


    public void Gameover()
    {
        Debug.Log("Out of lives");
    }
    public int Getlives()
    {
        return lives;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        List<Collider> collisions = new List<Collider>();

        for (int i = 0; i < collisonSpheres.Length; i++)
        {

            Vector3 displacement = slidespheres[i] - collisonSpheres[i].offset;

            displacement *= playeranim.GetFloat(SlideCurvepara);

            Vector3 offset = collisonSpheres[i].offset + displacement;


            foreach (Collider c in Physics.OverlapSphere(player.transform.position + offset, collisonSpheres[i].radius, collisionmask)){
                collisions.Add(c);
            }


            if(lives <= 0)
            {
                outoflives();
            }
        }

        if (collisions.Count > 0)
        {
            onObstacleCollision();

        }
       // Debug.Log($"collisionMask: {collisionmask}");
    }

    private void ObstacleCollision()
    {
        if (!invincible){
            lives--;
            StartCoroutine(Hit());
            
        }
    }

   IEnumerator Hit()
    {
        invincible = true;

       
        for (int i = 0; i < 2; i++)
        {
            rend.enabled = false;
            yield return new WaitForSeconds(blinkrate);
            rend.enabled = true;
            yield return new WaitForSeconds(blinkrate);
        }        
        invincible = false;
    }

    int GetlayerMask(params int [] Indexs)
    {
        int mask = 0;

        for (int i = 0; i < Indexs.Length;i++)
        {
            mask |= 1 << Indexs[i]; 
        }

        return mask;
        //return 1 << Index;
    }


    int GetLayerIngoreMask(params int[] Indexs)
    {


        return ~GetlayerMask(Indexs);
    }

    // ref takes in a variable and allows modifying it
   void Addlayer (ref int mask,params int[] Indexs)
    {

        mask |= GetlayerMask(Indexs);
        
    }

   void removeLayer(ref int mask,params int[] Indexs)
    {
        mask &= ~GetlayerMask(Indexs);
    }

     private void OnDrawGizmos()

    {
       
        if (!Application.isPlaying || !debugspheres)
        {
            return;
        }

        for (int i = 0; i < collisonSpheres.Length; i++)
        {
            //vector that moves it to final position
            Vector3 displacement = slidespheres[i] - collisonSpheres[i].offset;


            displacement *= playeranim.GetFloat(SlideCurvepara);

            Vector3 offset = collisonSpheres[i].offset + displacement;

            Gizmos.color = Color.red;

            Gizmos.DrawSphere(player.transform.position + offset, collisonSpheres[i].radius);
        }

    }

}
