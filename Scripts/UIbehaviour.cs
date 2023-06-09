﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UIbehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    

    public void loadLevel(string levelname = "default")
    {
        SceneManager.LoadScene(levelname);
    }

  

    public void Quit()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }

    public void Menu()
    {
        SceneManager.LoadScene(0);
    }

    public void debug()
    {
        Debug.Log("Works!");
    }

    public void Move(int d)
    {
        playerController.Instance.MovePlayer(d);
    }

    public void jump()
    {
        playerController.Instance.forceJump();
    }

    public void slide()
    {
        playerController.Instance.forceSlide();
    }

    public void shoot()
    {
        Vector3 mouse = Input.mousePosition;


        GameObject Target = GameObject.Find("Target");
        Vector3 TargetCam = Camera.main.WorldToScreenPoint(Target.transform.position);

        if(mouse == TargetCam)
        {
            //Destroy Barrier, this code will probably have to be moved and reworked to work with touch
        }
        

    }
    public void Reset()
    {
        //Scene thisS = SceneManager.GetActiveScene();
        // SceneManager.LoadScene(thisS.name);
        // Debug.Log("Reset: "+ SceneManager.GetActiveScene().name);
        //GameManager.Instance.ResetLevel();
    }

    // Update is called once per frame

}
