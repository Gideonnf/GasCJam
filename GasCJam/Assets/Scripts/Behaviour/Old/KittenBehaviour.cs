using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KittenBehaviour : Detection
{
    // to check how far it travelled from its original position
    Vector2 startingPos;
    [Header("Cat Behaviour settings")]
    [Tooltip("How far the cat can go before it gets tired")]
    public float maxTravelDistance = 0.0f;
    [Tooltip("How far it can be from the starting position after going back")]
    public float minTravelDistance = 0.0f;
    bool itDoBLikeThat;

    public bool isRunning = false;

    // If it is tired, it'll go back to it's starting position
    public bool isTired = false;

    KittenMovement kittenMovement;


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        kittenMovement = GetComponent<KittenMovement>();

        // Store the starting position
        startingPos = transform.position;

        // Start the coroutine
        // running it outside of update so it doesnt lag anything
        StartCoroutine("CheckForObjectsInRange");
        StartCoroutine("CheckForObjectsInView");
    }

    // Update is called once per frame
    void Update()
    {
        if (isRunning)
        {
            Vector2 distanceVector = (Vector2)transform.position - startingPos;
            float distanceFromStarting = distanceVector.magnitude;

            // the cat becomes too tired
            if (distanceFromStarting >= maxTravelDistance)
            {
                // set is tired to true
                isTired = true;
                // reset the movement
                kittenMovement.ResetMovement();
                
                // reset the movement
            }

            // if it is tired
            if (isTired)
            {
                if (distanceFromStarting <= minTravelDistance)
                {
                    // it returned back to the starting position
                    isTired = false;
                    kittenMovement.ResetMovement();
                    isRunning = false;

                    return;
                }

                targetDir = GetTargetDirection();

                Debug.Log("Returning Direction" + targetDir);
            }

            kittenMovement.SetKittenDirection(targetDir);


            //if (targetDir == DIRECTIONS.DOWN)
            //    kittenMovement.SetKittenDirection(DIRECTIONS.UP, targetDir);
            //else if (targetDir == DIRECTIONS.UP)
            //    kittenMovement.SetKittenDirection(DIRECTIONS.DOWN, targetDir);
            //else if (targetDir == DIRECTIONS.LEFT)
            //    kittenMovement.SetKittenDirection(DIRECTIONS.RIGHT, targetDir);
            //else if (targetDir == DIRECTIONS.RIGHT)
            //    kittenMovement.SetKittenDirection(DIRECTIONS.LEFT, targetDir);
        }
    }

    public override Vector2 GetTargetDirVector()
    {
        if (isTired)
        {
            Vector2 currentPos = transform.position;
            Vector2 Dir = (startingPos - currentPos).normalized;

            return Dir;
        }
        else
        {
            return base.GetTargetDirVector();
        }
    }

    public override DIRECTIONS GetTargetDirection()
    {
       // return base.GetTargetDirection();
       if (isTired)
        {
            Vector2 currentPos = transform.position;

            Vector2 Dir = (startingPos - currentPos).normalized;

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

        }
        else
        {
            return base.GetTargetDirection();
        }

        return DIRECTIONS.NONE;
    }

    public override bool DetectRadius()
    {
        // Clear the list before checking everytime
        ObjectsInRange.Clear();

        // If base detected something in radius
        if (base.DetectRadius())
        {
            // Loop through the detected objects
            foreach(GameObject detectedObj in ObjectsInRange)
            {
                // if target object already exist
                if (targetObject != null)
                {
                    if (targetObject == detectedObj)
                    {
                        // make it run
                        isRunning = true;
                        return true;
                    }
                }
                
                // if it detects the prey
                if (detectedObj.tag == "Prey")
                {
                    targetObject = detectedObj;
                    isRunning = true;

                    return true;
                }
                
                // if it detects the player
                // it'll priorities the prey first 
                if (detectedObj.tag == "Player")
                {
                    // the cat is gonna be sad cause it sees its mother
                    // i do this later
                    // gsihgioahshapsdighaioghasdihpgioh
                }
            }
        }

        return false;
    }

    public override CHARACTERS CheckForCharacters(Vector2Int tilePosition)
    {
        Vector2Int ratTilePos = MapManager.Instance.GetWorldToTilePos(ratObject.transform.position);
        Vector2Int playerTilePos = MapManager.Instance.GetWorldToTilePos(playerObject.transform.position);

        if (ratTilePos == tilePosition)
            return CHARACTERS.MOUSE;
        else if (playerTilePos == tilePosition)
            return CHARACTERS.PLAYER;

        return CHARACTERS.NONE;
    }

    // A couroutine to run for checking of objects
    // Instead of checking every frame it checks every second
    IEnumerator CheckForObjectsInRange()
    {
        // NOTE: I might change this to check if the rat is nearby
        // if it is then it'll win
        // nearby as in really fuking close

        for (; ; )
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

    IEnumerator CheckForObjectsInView()
    {
        for (; ; )
        {
            if (DetectInView() == CHARACTERS.MOUSE)
            {
               // if (isRunning == false)
               // {
                    targetObject = ratObject;
                    targetDir = GetTargetDirection();
                    isRunning = true;
               // }
            }
            else if (DetectInView() == CHARACTERS.PLAYER)
            {
                // Detected the mother cat
            }

            yield return new WaitForSeconds(.2f);
        }
    }
}
