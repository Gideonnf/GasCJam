using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KittenDetection : Detection
{
    NewKittenMovement kittenMovement;

    [Tooltip("How long the spent shocked")]
    public float shockTime;
    [Tooltip("If it is shocked or not")]
    public bool isShocked = false;
    [Tooltip("Detection circle for rat")]
    public float RatDetectionRadius;

    float elapsedTime;
    

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        kittenMovement = GetComponent<NewKittenMovement>();

        // Set the position of the sprite into the center of the tile
        // incase its alittle off center
        Vector2Int currentTilePos = MapManager.Instance.GetWorldToTilePos(transform.position);
        Vector2 tilePosition2D = MapManager.Instance.GetTileToWorldPos(currentTilePos);
        // set the position
        transform.position = tilePosition2D;

        StartCoroutine("CheckForObjectsInRange");
        StartCoroutine("CheckForObjectsInView");

    }

    // Update is called once per frame
    void Update()
    {
        if (isShocked)
        {
            Debug.Log(elapsedTime);
            elapsedTime += Time.deltaTime;

            //Debug.Log(elapsedTime);

            // if its done getting shocked
            if (elapsedTime >= shockTime)
            {
                // change the state
                characterState = STATE.CHASING;

                isShocked = false;

                elapsedTime = 0;
            }
        }
        else
        {
            elapsedTime = 0;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, CircleRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, RatDetectionRadius);
    }

    public void SetViewDirection(DIRECTIONS movingDirection)
    {
        viewDir = movingDirection;

        
    }


    public override bool DetectRadius()
    {

        // Clear the objets in the list incase
        if (ObjectsInRange.Count > 0)
            ObjectsInRange.Clear();

        // Dont chase for targets when it's tired
        if (characterState == STATE.TIRED)
        {
            //ReturnToStart();
            return false;
        }

        // call the base class detect radius to get the list of objects in range
        if (base.DetectRadius())
        {
            // Loop through the detected objects
            foreach (GameObject detectedObj in ObjectsInRange)
            {
                if (detectedObj.tag == "Prey")
                {
                    // Only detect when idle
                    if (characterState == STATE.IDLE)
                    {
                        targetObject = detectedObj;

                        targetDir = GetTargetDirection();

                        // Check if the path to the detected object is clear
                        if (CheckIfClear(targetDir, targetObject.transform.position) == false)
                        {
                            // that target isnt available a danger to the mouse
                            StopMovement();
                            // check the next if there is
                            continue;
                        }

                        isShocked = true;
                    }


                    //// Set the mouse object as it's current target
                    //targetObject = detectedObj;

                    //// Get the target's direc tion
                    //targetDir = GetTargetDirection();

                    //// Check the direction if its clear
                    //// if it isn't then they detected an enemy through the wall
                    ////if (CheckIfClear(targetDir) == false)
                    ////{
                    ////    StopMovement();
                    ////    return false;
                    ////}
                    //// if they are already chasing
                    //// they cant really get shocked
                    //if (characterState != STATE.CHASING)
                    //    isShocked = true;

                    //// Set the character state
                    ////characterState = STATE.CHASING;


                    return true;
                }
            }
        }
        else
        {
           // Debug.Log("Not in range");
            // if theres nothing in it's range anymore
            isShocked = false;
        }

        return false;
    }

    public void ReturnToStart()
    {
        // Create a blank game object
        if (targetObject == null)
            targetObject = new GameObject("StartingPosition");
        // Set the starting position

        //characterState = STATE.TIRED;

        targetObject.transform.position = kittenMovement.startingPos;

        targetDir = GetTargetDirection();
    }

    public void StopMovement()
    {
        characterState = STATE.IDLE;
        targetObject = null;
        targetDir = DIRECTIONS.NONE;
    }

    public override CHARACTERS CheckForCharacters(Vector2Int tilePosition)
    {
        Vector2Int mouseTilePos = MapManager.Instance.GetWorldToTilePos(ratObject.transform.position);
        Vector2Int playerTilePos = MapManager.Instance.GetWorldToTilePos(playerObject.transform.position);

        if (mouseTilePos == tilePosition)
            return CHARACTERS.MOUSE;
        if (playerTilePos == tilePosition)
            return CHARACTERS.PLAYER;

        return CHARACTERS.NONE;
    }

    public bool DetectRat()
    {
        Collider2D[] ListOfColliders = Physics2D.OverlapCircleAll(transform.position, RatDetectionRadius);

        foreach (Collider2D collider in ListOfColliders)
        {
            // don't check for itself
            if (collider.gameObject == this.gameObject)
                continue;

            // if it collides with the prey
            if (collider.gameObject.tag == "Prey")
            {
                // Win the game
                //Debug.LogError("Game ended");
            }
        }

        return false;
    }

    // A couroutine to run for checking of objects
    // Instead of checking every frame it checks every second
    IEnumerator CheckForObjectsInRange()
    {
        for (; ; )
        {
            // Check if the rat is touching the cat
            DetectRat();
            // If it successfully detected something in it's radius
            // Check for what direction it is in
            DetectRadius();


            yield return new WaitForSeconds(.2f);
        }
    }

    IEnumerator CheckForObjectsInView()
    {
        for (; ;)
        {
            if (DetectInView() == CHARACTERS.PLAYER)
            {
                // lose the game
            }

            if (DetectInView() == CHARACTERS.MOUSE)
            {
                if (characterState != STATE.TIRED)
                {
                    isShocked = true;

                    targetObject = ratObject;

                    targetDir = GetTargetDirection();
                }
            }
            //else
            //{
            //    isShocked = false;
            //}

            yield return new WaitForSeconds(.2f);
        }
    }
}
