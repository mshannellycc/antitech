using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelegateExample : MonoBehaviour
{

    delegate float DoMath(float A, float B);

    


//keyword delegate returns after taking in two floats
//delegate ReturnType DelegateName(parameters)

// Start is called before the first frame update
private float Add(float A, float B)//ooooooh
    {
        return A + B;
    }


    private float Subtract(float A, float B)
    {
        return A - B;
    }

void Start()
    {
        
       DoMath domathvaraible = new DoMath(Add); //gets overridden if you set domathvarible += somethingelse
        //domathvaraible(4,6) => (A * B);
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}

