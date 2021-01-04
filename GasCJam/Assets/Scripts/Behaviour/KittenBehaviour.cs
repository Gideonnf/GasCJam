using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KittenBehaviour : Detection
{
    // Start is called before the first frame update
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool DetectRadius()
    {
        // If base detected something in radius
        if (base.DetectRadius())
        {
            //Debug.Log("it detected something useful for the kitten");

        }
        else
        {

        }
            //Debug.Log("Nothing detected on the kitten");

        

        return false;
    }
}
