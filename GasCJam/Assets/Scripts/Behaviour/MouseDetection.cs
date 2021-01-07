﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDetection : Detection
{
    MouseMovement mouseMovement;

    [Tooltip("Keep track if hte player is in sight of the mouse")]
    public bool playerInSight = false;

    

    [Tooltip("How long the spent shocked")]
    public float shockTime;
    [Tooltip("If it is shocked or not")]
    public bool isShocked = false;

    float elapsedTime;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        mouseMovement = GetComponent<MouseMovement>();

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
        //DetectRadius();
        if (isShocked)
        {
            // never update fast enough yet
            if (targetObject == null)
                return;

            elapsedTime += Time.deltaTime;

            //Debug.Log(elapsedTime);

            if (elapsedTime >= shockTime || targetObject.tag == "Player")
            {
                characterState = STATE.RUNNING;

                isShocked = false;

                elapsedTime = 0;
            }
        }
    }
    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, CircleRadius);
    }


    /// <summary>
    ///  For checking nearby enemies
    /// </summary>
    /// <returns>returns true if there are enemies nearby</returns>
    public bool CheckForEnemies()
    {
        // theres enemies in the area
        if (base.DetectRadius() == true)
        {
            return true;
        }

        return false ;
    }

    public void StopMovement()
    {
        characterState = STATE.IDLE;
        targetObject = null;
        targetDir = DIRECTIONS.NONE;
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

    /// <summary>
    /// Checks the direc tion before moving in it if the kitten is there
    /// </summary>
    /// <param name="dirToCheck"></param>
    /// <returns></returns>
    public bool CheckForKitten(DIRECTIONS dirToCheck)
    {
        Vector2Int currentTilePos = MapManager.Instance.GetWorldToTilePos(transform.position);
        Vector2Int kittenTilePos = MapManager.Instance.GetWorldToTilePos(kittenObject.transform.position);

        // Dont check for the kitten if its being chased by the kitten
        if (targetObject == kittenObject)
            return false;

        while (MapManager.Instance.IsThereTileOnMap(currentTilePos) == false)
        {
            switch (dirToCheck)
            {
                case Detection.DIRECTIONS.UP:
                    currentTilePos.y++;
                    break;
                case Detection.DIRECTIONS.DOWN:
                    currentTilePos.y--;
                    break;
                case Detection.DIRECTIONS.LEFT:
                    currentTilePos.x--;
                    break;
                case Detection.DIRECTIONS.RIGHT:
                    currentTilePos.x++;
                    break;
                case Detection.DIRECTIONS.NONE:
                    break;
                default:
                    break;
            }

            // the kitten is along that direction
            if (currentTilePos == kittenTilePos)
            {
                return true;
            }

        }


        return false;
    }

    public override bool DetectRadius()
    {
        // Clear the objects in range
        if (ObjectsInRange.Count > 0)
            ObjectsInRange.Clear();

        // If it detects any objects in it's radius
        if (base.DetectRadius())
        {
            // Loop through the detected objects
            foreach (GameObject detectedObj in ObjectsInRange)
            {
                // If it spots the player or kitten
                // it will try to run away
                if (detectedObj.tag == "Player" || detectedObj.tag == "Kitten" )
                {
                    // if it already has a target
                    if (targetObject != null)
                    {
                        // If it detects the kitten while being chased by the player
                        if (targetObject != detectedObj && targetObject.tag == "Player")
                        {
                            // Make it shocked
                            isShocked = true;

                            // stop both the movement
                            StopMovement();
                            mouseMovement.StopMovement();
                        }
                    }

                    // Set the player object as it's current target
                    targetObject = detectedObj;

                    targetDir = GetTargetDirection();

                    // Check the direction if its clear
                    // if it isn't then they detected an enemy through the wall
                    if (CheckIfClear(targetDir) == false)
                    {
                        StopMovement();

                        continue;
                    }

                    // shocked
                    if (characterState != STATE.RUNNING)
                        isShocked = true;

                }
            }
        }
        else
        {
            isShocked = false;
        }
        //else
        //{
        //    characterState = STATE.IDLE;
        //    targetObject = null;
        //    targetDir = DIRECTIONS.NONE;
        //}


        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetViewDirection(DIRECTIONS movingDirection)
    {
        switch (movingDirection)
        {
            // If it's moving up
            // look down
            case DIRECTIONS.UP:
                viewDir = DIRECTIONS.DOWN;
                break;
            // if its moving down
            // look up
            case DIRECTIONS.DOWN:
                viewDir = DIRECTIONS.UP;
                break;
            // if its moving eft
            // look right
            case DIRECTIONS.LEFT:
                viewDir = DIRECTIONS.RIGHT;
                break;
            // if its moving right
            // look left
            case DIRECTIONS.RIGHT:
                viewDir = DIRECTIONS.LEFT;
                break;
            case DIRECTIONS.NONE:
                break;
            default:
                break;
        }
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

    IEnumerator CheckForObjectsInView()
    {
        for (; ;)
        {
            if (DetectInView() == CHARACTERS.PLAYER)
            {
                // NOTE
                // later it'll change to having a shock animation
                // instead of running straight away

                // running
                characterState = STATE.RUNNING;

                // Set the player object as it's current target
                targetObject = playerObject;

                targetDir = GetTargetDirection();

                playerInSight = true;
            }
            else
            {
                playerInSight = false;
            }

            yield return new WaitForSeconds(.2f);
        }
    }

}
