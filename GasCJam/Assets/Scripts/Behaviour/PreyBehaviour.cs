using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyBehaviour : Detection
{
    GameObject targetObject = null;
    DIRECTIONS targetDir = DIRECTIONS.NONE;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("CheckForObjects");
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public override bool DetectRadius()
    {
        // Runs the base function to check for objects in radius
        if (base.DetectRadius())
        {
            foreach (GameObject detectedObj in ObjectsInRange)
            {
                // if the player is detected in the objects in range
                if (detectedObj.tag == "Player")
                {
                    // If the target object already exists
                    // Don't have to reset the detected object
                    // Focuses on one object at a time

                    Debug.Log("Target acquired");

                    if (targetObject == null)
                        // Store the target object
                        targetObject = detectedObj;

                    return true;
                }
                // if the kitten is detected in the objects in range
                else if (detectedObj.tag == "Kitten")
                {
                    // idk what to do with this part yet lol
                    // fuk i haven't coded in a long time
                    // pee pee poo poo

                    //if (targetObject != null)

                        
                }
                // if not then it shouldn't care what objects it detected
            }
        }

        //// If it reaches here then no objects of importance was detected
        //// clear the list of objects
        //ObjectsInRange.Clear();
        //// if it no longer detec ts anything then set the target object back to null
        //targetObject = null;

        ResetTarget();

        return false;
    }

    void ResetTarget()
    {
        // Reset the target object to null
        targetObject = null;
        // Clear the list of objects
        ObjectsInRange.Clear();
        // set the direction back to none
        targetDir = DIRECTIONS.NONE;

        Debug.Log("We'll get em next time");
    }

    /// <summary>
    /// Gets the direction of the target object
    /// If the prey has to run, it will run in the opposite direction
    /// </summary>
    /// <returns>Returns the direction of the object</returns>
    public DIRECTIONS GetTargetDirection()
    {
        // Get the two positions we need
        Vector2 TargetPos = targetObject.transform.position;
        Vector2 CurrentPos = transform.position;

        // Get the direction of the target
        Vector2 Dir = (TargetPos - CurrentPos).normalized;

        Debug.Log(targetDir);
        Debug.Log(Dir);

        // Checking for Up and Down first
        // if we're checking for up and down 
        // the X dir will be within a small buffer
        // 0.3f is just a number i used as a buffer
        // anything more than that means its on a diagonal
        // and player cant move in diagonal
        if (Dir.x <= 0.3f || Dir.x >= -0.3f)
        {
            // 0.5f is a buffer i used
            if (Dir.y > 0.5f)
            {
                return DIRECTIONS.UP;
            }
            else if (Dir.y < -0.5f)
            {
                return DIRECTIONS.DOWN;
            }
        }
        if (Dir.y <= 0.3f || Dir.y >= -0.3f)
        {
            if (Dir.x > 0.5f)
            {
                return DIRECTIONS.LEFT;
            }
            else if (Dir.x < -0.5f)
            {
                return DIRECTIONS.RIGHT;
            }
        }


        return DIRECTIONS.NONE;
    }

    // A couroutine to run for checking of objects
    // Instead of checking every frame it checks every 10th
    IEnumerator CheckForObjects()
    {
        for(;;)
        {
            // If it successfully detected something in it's radius
            // Check for what direction it is in
            if (DetectRadius())
                // Set the target direction here
                targetDir = GetTargetDirection();
            yield return new WaitForSeconds(.5f);
        }
    }

    
}
