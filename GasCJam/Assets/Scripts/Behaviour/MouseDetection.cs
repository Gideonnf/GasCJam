using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDetection : Detection
{
    MouseMovement mouseMovement;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        mouseMovement = GetComponent<MouseMovement>();

        StartCoroutine("CheckForObjectsInRange");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool DetectRadius()
    {
        // Clear the objects in range
        ObjectsInRange.Clear();

        // If it detects any objects in it's radius
        if (DetectRadius())
        {
            // Loop through the detected objects
            foreach(GameObject detectedObj in ObjectsInRange)
            {
                // If it spots the player or kitten
                // it will try to run away
                if (detectedObj.tag == "Player" || detectedObj.tag == "Kitten")
                {
                    // running
                    characterState = STATE.RUNNING;

                    // Set the player object as it's current target
                    targetObject = detectedObj;

                    targetDir = GetTargetDirection();
                }
            }
        }
        else
        {
            characterState = STATE.IDLE;
            targetObject = null;
            targetDir = DIRECTIONS.NONE;
        }


        return false;
    }

    // A couroutine to run for checking of objects
    // Instead of checking every frame it checks every second
    IEnumerator CheckForObjectsInRange()
    {
        for (; ; )
        {
            // If it successfully detected something in it's radius
            // Check for what direction it is in
            DetectRadius();


            yield return new WaitForSeconds(.2f);
        }
    }

}
