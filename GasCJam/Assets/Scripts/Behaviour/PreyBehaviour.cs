using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyBehaviour : Detection
{
    // Stores the target's direction
    // DIRECTIONS targetDir = DIRECTIONS.NONE;
    PreyMovement preyMovement;
    // Checks if the prey is running
    public bool isRunning = false;

    [SerializeField] DIRECTIONS startingDir;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        preyMovement = GetComponent<PreyMovement>();

        viewDir = startingDir;

        // Start the coroutine at the start
        // the prey will always be checking every second instead of every frame
        StartCoroutine("CheckForObjectsInRange");
        StartCoroutine("CheckForObjectsInView");
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


    public override CHARACTERS CheckForCharacters(Vector2Int tilePosition)
    {
        // Get the player position in tiles
        Vector2Int playerTilePos = MapManager.Instance.GetWorldToTilePos(playerObject.transform.position);

        // if the player is within that tile
        if (playerTilePos == tilePosition)
            return CHARACTERS.PLAYER;

        return CHARACTERS.NONE;
    }

    // A couroutine to run for checking of objects
    // Instead of checking every frame it checks every second
    IEnumerator CheckForObjectsInRange()
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

    // i dont know if i should put this in 1 coroutine or separate ones
    // but im doing them in different ones for now
    IEnumerator CheckForObjectsInView()
    {
        for (; ;)
        {
            // if the player is found in its iew
            if (DetectInView() == CHARACTERS.PLAYER)
            {
                // if it isnt running then detect the player in it's view
                if (isRunning == false)
                {
                    // NOTE: I dont know if this gonna work yet lol
                    // EDIT: i think this works already
                    targetObject = playerObject;
                    targetDir = GetTargetDirection();
                    isRunning = true;
                }
            }

            yield return new WaitForSeconds(.2f);
        }
    }
    
}
