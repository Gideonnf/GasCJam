using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyBehaviour : Detection
{
    // Stores the target object
    GameObject targetObject = null;
    // Stores the target's direction
    DIRECTIONS targetDir = DIRECTIONS.NONE;
    PreyMovement preyMovement;
    // Checks if the prey is running
    public bool isRunning = false;

    [SerializeField] DIRECTIONS startingDir;

    // Start is called before the first frame update
    void Start()
    {
        preyMovement = GetComponent<PreyMovement>();

        // Start the coroutine at the start
        // the prey will always be checking every second instead of every frame
        StartCoroutine("CheckForObjects");
    }

    // Update is called once per frame
    void Update()
    {
        // If they're running away        
        if (isRunning)
        {
            // Move in the opposite direction first
           
            if (targetDir == DIRECTIONS.DOWN)
                preyMovement.MovePrey(DIRECTIONS.UP, targetDir);
            else if (targetDir == DIRECTIONS.UP)
                preyMovement.MovePrey(DIRECTIONS.DOWN, targetDir);
            else if (targetDir == DIRECTIONS.LEFT)
                preyMovement.MovePrey(DIRECTIONS.RIGHT, targetDir);
            else if (targetDir == DIRECTIONS.RIGHT)
                preyMovement.MovePrey(DIRECTIONS.LEFT, targetDir);
        }
        else
        {
            //if (DetectRadius())
            //{
            //    // Set the target direction here
            //    targetDir = GetTargetDirection();
            //}
        }
    }

    public override bool DetectRadius()
    {
        // Runs the base function to check for objects in radius
        // Clear the list before checking everytime
        ObjectsInRange.Clear();

        if (base.DetectRadius())
        {
            foreach (GameObject detectedObj in ObjectsInRange)
            {
                if (targetObject != null)
                {
                    if (targetObject == detectedObj)
                    {
                        isRunning = true;
                        return true;
                    }    
                }

                // if the player is detected in the objects in range
                if (detectedObj.tag == "Player")
                {
                    // If the target object already exists
                    // Don't have to reset the detected object
                    // Focuses on one object at a time

                    //Debug.Log("Target acquired");

                    targetObject = detectedObj;

                    isRunning = true;

                    return true;
                }
                // if the kitten is detected in the objects in range
                else if (detectedObj.tag == "Kitten")
                {
                    // idk what to do with this part yet lol
                    // fuk i haven't coded in a long time
                    // pee pee poo poo
                        
                }
                // if not then it shouldn't care what objects it detected
            }
        }

        // if it isnt detecting anything
        // and it isnt moving
        // then reset it
        if (preyMovement.isMoving == false)
        {
            ResetTarget();
            preyMovement.ResetMovement();
        }


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

        isRunning = false;
        
       
       // Debug.Log("We'll get em next time");
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

        //Debug.Log(targetDir);
       // Debug.Log(Dir);

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
                return DIRECTIONS.RIGHT;
            }
            else if (Dir.x < -0.5f)
            {
                return DIRECTIONS.LEFT;
            }
        }


        return DIRECTIONS.NONE;
    }

    // A couroutine to run for checking of objects
    // Instead of checking every frame it checks every second
    IEnumerator CheckForObjects()
    {
        for(;;)
        {
            // If it successfully detected something in it's radius
            // Check for what direction it is in
            if (DetectRadius())
            {
                // Set the target direction here
                targetDir = GetTargetDirection();

            }
            yield return new WaitForSeconds(.2f);
        }
    }


    
}
