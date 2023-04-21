
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundScript : Singleton<SoundScript>
{
    // Start is called before the first frame update
    public static AudioClip playerJumpSound, Playerdeath, coin, spring;

    static AudioSource audiop;

    private IEnumerator currentlanechange;

    void Start()
    {
        playerJumpSound = Resources.Load<AudioClip>("Stinger");

       // coin = Resources.Load<AudioClip>("Coin");

        Playerdeath = Resources.Load<AudioClip>("Hit_Hurt2");

       // spring = Resources.Load<AudioClip>("Spring");

        audiop = GetComponent<AudioSource>();

        //currentlanechange = Ensureplayed();
        // StartCoroutine(currentlanechange);


    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void playeffect(string clip)
    {
        switch (clip)
        {
            case "playerJumpSound":

                audiop.PlayOneShot(playerJumpSound);
                // currentlanechange = Ensureplayed();
                //new WaitForSeconds(audiop.clip.length);
                break;

            case "Playerdeath":
                audiop.PlayOneShot(Playerdeath);
                //new WaitForSeconds(audiop.clip.length);
                break;

           

        }
    }

    IEnumerator Ensureplayed()
    {
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(audiop.clip.length);

        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }


}

