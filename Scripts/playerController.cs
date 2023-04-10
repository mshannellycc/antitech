using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : Singleton<playerController>
{
    //NOTICE: ask how to use coroutines, WRITE IT DOWN.

    PlayerInputActions inputaction;

    State CurrentState;

    public bool isusingdiffinput = true;

    float horizontal = 0f;
   public float jump{ get; private set;} = 0f;


int jumppara;
    Animator anim;
    int slidepara;

    [SerializeField]
    float gravity = -9.81f;
    [SerializeField]
    float initalvelocity = 5f;

    public float slide { get; private set; } = 0f;
    int currentlane = 0;
    int previouslane = 0;





    [SerializeField]
    public int numOfLanes = 5;

    public int NumLanes { get { return numOfLanes; } }
    

    float lanewidth;
    public float Lanewidth { get { return lanewidth; } }

    public float hPrev { get; private set; } = 0f;
    public float hNew { get; private set; } = 0f;
    public float vPrev { get; private set; } = 0f;
    public float vNew { get; private set; } = 0f;

    public float strafespeed = 5f;

    int directionbuffer = 0;


    Coroutine currentlanechange;


    int lanechangestackcalls = 0;
    // Start is called before the first frame update
    // Awake is called before the first frame.
    void Awake()
    {
        



       // StartCoroutine(testCoroutine());

        transform.position = Vector3.zero;

        //determine the center of each lane
        lanewidth = 7.5f / numOfLanes;


        inputaction = new PlayerInputActions();


        if (isusingdiffinput == false)
        {
            return;
        }

        inputaction.Player.Horizontal.performed += ctx => horizontal = ctx.ReadValue<float>();
        //keyup and keydown for jump
        inputaction.Player.Jump.performed += ctx => jump = ctx.ReadValue<float>();
        inputaction.Player.Jump.canceled += ctx => jump = ctx.ReadValue<float>();
        //keydown for slide
        inputaction.Player.Slide.performed += ctx => slide = ctx.ReadValue<float>();
        //keyup for slide
        inputaction.Player.Slide.canceled += ctx => slide = ctx.ReadValue<float>();

        anim = GetComponent<Animator>();

        //jumppara is string?
        jumppara = Animator.StringToHash("jump");
        slidepara = Animator.StringToHash("Slide");
        
    }

    // Update is called once per frame
    void Update()
    {


        if (isusingdiffinput == true)
        {
            usenewinputsystems();
        }
        else
        {
            useoldinputsystems();
        }


    }


    private void OnEnable()
    {
        inputaction.Player.Enable();
    }

    private void OnDisable()
    {
        inputaction.Player.Disable();
    }


    private void usenewinputsystems()
    {
        //Debug.log
        hNew = horizontal;
        float hDelta = hNew - hPrev;

      

        if (Mathf.Abs(hDelta) > 0f && Mathf.Abs(hNew) > 0f && CurrentState == State.Run)
        {
            MovePlayer((int)hNew);
        }



        if (slide == 1f && CurrentState == State.Run)
        {
            CurrentState = State.Slide;

            anim.SetTrigger(slidepara);
            
        }

        if (jump == 1f && CurrentState == State.Run)
        {
            CurrentState= State.Jump;
            StartCoroutine(Jump());
        }


        horizontal = 0f;
        slide = 0f;
        jump = 0f;

        hPrev = hNew;


    }


    public void forceJump()
    {
        CurrentState = State.Jump;
        StartCoroutine(Jump());
    }

    public void forceSlide()
    {
        CurrentState = State.Slide;

        anim.SetTrigger(slidepara);
    }

    public void MovePlayer(int d) //since this is public, it can be used by UI
    {

        if (currentlanechange != null)
        {
            if (currentlane + d != previouslane)
            {
                directionbuffer = d;
                return;
            }
            StopCoroutine(currentlanechange);
            directionbuffer = 0;
        }
      



        previouslane = currentlane;

        //keeps a value between the min and max, if it goes over then it will be clamped within them. 
        currentlane = Mathf.Clamp(currentlane + d, numOfLanes / -2, numOfLanes / 2);

        currentlanechange = StartCoroutine(laneChange());
    }


      

    private void useoldinputsystems()
    {

         hNew = Input.GetAxisRaw(inputNames.horizontalAxis);

        vNew = Input.GetAxisRaw(inputNames.verticalAxis);

        float vDelta = vNew - vPrev;
        //must getrawaxis
        //math.abs = math.absolute
        //! = not

        float hDelta = hNew - hPrev;

        if (Mathf.Abs(hDelta) > 0f && Mathf.Abs(hNew) > 0f && CurrentState == State.Run)
        {
            MovePlayer((int)hNew);
        }

        horizontal = hNew;

        int v = 0;


        if (Mathf.Abs(vDelta) > 0f && Mathf.Abs(vNew) > 0f && CurrentState != State.Run)
        {
            v = (int)vNew;
        }


        if ((Input.GetButtonDown(inputNames.jumpAxis) || v== 1) && CurrentState == State.Run)
        {
            CurrentState = State.Jump;
            StartCoroutine(Jump());
        }

        if ((Input.GetButtonDown(inputNames.slideAxis) || v == -1) && CurrentState == State.Run)
        {
            CurrentState = State.Slide;

            anim.SetTrigger(slidepara);
         
        }

        hPrev = hNew;
        vPrev = vNew;
    }

  

    IEnumerator laneChange()
    {

        Vector3 fromPosition = Vector3.right * previouslane * lanewidth;

        Vector3 toPosition = Vector3.right * currentlane * lanewidth;

        float t = (lanewidth - Vector3.Distance(transform.position.x * Vector3.right, toPosition))/lanewidth;

        //perform a gradual transistion to the next lane
        for ( ;t< 1f; t += strafespeed*Time.deltaTime/lanewidth)
        {
            transform.position = Vector3.Lerp(fromPosition + Vector3.up*transform.position.y, toPosition + Vector3.up*transform.position.y, t);
            yield return null;

        }

        transform.position = toPosition + Vector3.up*transform.position.y;
        currentlanechange = null;


        if (directionbuffer != 0 && ++lanechangestackcalls < 2)
        {
            MovePlayer(directionbuffer);
            directionbuffer = 0;
        }
        lanechangestackcalls = 0;
    }

    IEnumerator testCoroutine()
    {
        Debug.Log("Wait for 2 seconds");


        yield return new WaitForSeconds(2);

        Debug.Log("thank you, wait 3 more");
        yield return new WaitForSeconds(3);

        Debug.Log("have some factorials now");

        for (int i = 0; i < 10; i++)
        {
            int factorial = i;
            for(int j = i -1; j > 1; j -- )
            {
                factorial *= j;
            }
            int framecount = 2 - i;
            Debug.Log("frame: " + framecount + ":factorial of: " + i + " is " + factorial);
            //return from this point
            yield return null;
        }

        Debug.Log("Subroutine is done");
    }

    IEnumerator Jump()
    {
        //y = gt^2/2 + V*T
        // g = -9.81
        // V = 5
        // y = 0
        // t = -b +- sqrt( b^2 - 4av)/2

        //  double pos = (-5 + Mathf.Sqrt( Mathf.Pow(5, 2) - (float)(4 * -9.81))) / 2;
        // double neg = (-5 - Mathf.Sqrt(Mathf.Pow(5, 2) - (float)(4 * -9.81))) / 2;

        //0.509


        anim.SetBool(jumppara, true);

        float num = initalvelocity*2f / -gravity;

        float tland = num - 0.125f;

        float t = Time.deltaTime;

       for ( ; t < tland; t+= Time.deltaTime)
        {
            float ypos = gravity * Mathf.Pow( t, 2) / 2f + initalvelocity * t;
            Helpers.setpostionY(transform, ypos);
            yield return null;
        }



       
        anim.SetBool(jumppara, false);

        for (; t < num; t+= Time.deltaTime)
        {
            float ypos = gravity * Mathf.Pow(t, 2) / 2f + initalvelocity * t;
            Helpers.setpostionY(transform, ypos);
            yield return null;
        }

        Helpers.setpostionY(transform, 0f);
        CurrentState = State.Run;
    }
       
    void Finishslide()
    {
        CurrentState = State.Run;
    }
  
    void PauseEditor()
    {
        UnityEditor.EditorApplication.isPaused = true;
    }
}
