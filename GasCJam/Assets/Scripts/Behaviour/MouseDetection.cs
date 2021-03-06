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
    [Tooltip("If both are in targets are nearby ")]
    public bool isTrapped = false;
    Vector2Int TrappedTilePosition = Vector2Int.zero;

    float elapsedTime;

    [Header("ShockUI")]
    public ExclaimationMarkUI m_ShockUI;
    public GameObject m_PlayerShockUI;
    public float m_PlayerShockUITime = 0.6f;

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

            if(isTrapped)
            {
                if (elapsedTime >= shockTime)
                {
                    elapsedTime = 0.0f;
                    isShocked = false;

                    characterState = STATE.RUNNING;

                    if (m_ShockUI != null)
                    {
                        m_ShockUI.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if(m_ShockUI != null)
                        m_ShockUI.UpdateExclaimationMarkUI(1.0f - (shockTime - elapsedTime) / shockTime);

                }
            }
            else
            {
                if (elapsedTime >= shockTime || targetObject.tag == "Player")
                {
                    if (characterState != STATE.RUNNING && targetObject.tag == "Player")
                        StartCoroutine(PlayerShockUITime());

                    characterState = STATE.RUNNING;

                    isShocked = false;

                    //elapsedTime = 0;
                    if (m_ShockUI != null)
                    {
                        m_ShockUI.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (m_ShockUI != null)
                        m_ShockUI.UpdateExclaimationMarkUI(1.0f - (shockTime - elapsedTime) / shockTime);
                }
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
    /// 
    /// </summary>
    /// <param name="character">The character to check for</param>
    /// <returns>true if it is within a tile spacing</returns>
    public bool IsEnemyNearby(CHARACTERS character)
    {
        Vector2Int characterTilePosition = Vector2Int.zero;
        Vector2 characterPosition = Vector2.zero ;

        // it is already targetting the player
        if (targetObject == playerObject)
            return false;


        if (character == CHARACTERS.PLAYER)
        {
            characterPosition = playerObject.transform.position;
            characterTilePosition = MapManager.Instance.GetWorldToTilePos(playerObject.transform.position);
        }

        Vector2Int currentTilePosition = MapManager.Instance.GetWorldToTilePos(transform.position);

        DIRECTIONS characterDirection = GetTargetDirection(characterPosition);

        // it is within 1 tile
        if(Vector2Int.Distance(currentTilePosition, characterTilePosition) <= 1)
        {
            // check if its through the wall
            if (CheckIfClear(characterDirection, characterPosition))
                return true;

        }

        return false;
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

        int counter = 0;

        // Dont check for the kitten if its being chased by the kitten
        if (targetObject == kittenObject)
            return false;

        while (MapManager.Instance.IsThereTileOnMap(currentTilePos) == false)
        {

            counter++;

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

            if (counter >= SightDistance)
                break;

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
            // if both are in the list at the same time
            // check if they are both within range to chase the cat
            if (ObjectsInRange.Contains(playerObject) && ObjectsInRange.Contains(kittenObject))
            {
                DIRECTIONS playerDirection = GetTargetDirection(playerObject.transform.position);
                DIRECTIONS kittenDirection = GetTargetDirection(kittenObject.transform.position);

                // if both are clear
                //  then both can chase the cat
                if (CheckIfClear(playerDirection, playerObject.transform.position) && CheckIfClear(kittenDirection, kittenObject.transform.position))
                {

                    // set the kitten object as the target
                    // it should prioritise the kitten
                    targetObject = kittenObject;
                    targetDir = GetTargetDirection();

                    // is shocked should only trigger once
                    // to stop it from repeating multiple times
                    if (isTrapped == false)
                    {
                        isShocked = true;
                        TrappedTilePosition = MapManager.Instance.GetWorldToTilePos(transform.position);
                    }

                    if (m_ShockUI != null)
                        m_ShockUI.gameObject.SetActive(true);

                    isTrapped = true;
                }
                else
                {
                    isTrapped = false;
                }
            }
            else
            {
                isTrapped = false;
            }

            // if it is trapped, it shouldnt be able to do anything else
            if (isTrapped)
                return false;

            //// If it already has a target that it is running from
            //if (targetObject != null)
            //    return false;

            // Loop through the detected objects
            string successfulTag = "";
            foreach (GameObject detectedObj in ObjectsInRange)
            {
                // It will only look for these two objects
                if (detectedObj.tag == "Player" || detectedObj.tag == "Kitten")
                {
                    // Only detect when its idle
                    if (characterState == STATE.IDLE)
                    {
                        //THIS IS A HACK, STORE PREV DATA
                        GameObject PREVtargetObject = targetObject;
                        DIRECTIONS prevTargetDir = targetDir;

                        // Get the target object
                        targetObject = detectedObj;
                        // Check for the target direction
                        targetDir = GetTargetDirection();

                        // Check if the path to the detected object is clear
                        if (CheckIfClear(targetDir, targetObject.transform.position) == false)
                        {
                            // that target isnt available a danger to the mouse
                            if (successfulTag == "") //if theres nothing just reset
                            {
                                StopMovement();
                            }

                            //reset the data
                            targetObject = PREVtargetObject;
                            targetDir = prevTargetDir;

                            // check the next if there is
                            continue;
                        }

                        isShocked = true;

                        if (detectedObj.tag == "Kitten")
                        {
                            if (m_ShockUI != null)
                                m_ShockUI.gameObject.SetActive(true);
                        }
                        else
                        {
                            elapsedTime = 0.0f;
                        }

                        ////HACK PRIROTIZE KITTEN
                        //if (successfulTag == "Kitten" && detectedObj.tag == "Player")
                        //{
                        //    targetObject = PREVtargetObject;
                        //    targetDir = prevTargetDir;
                        //}
                        //else
                        //{
                        //    // reset it if it tracks the player
                        //    if (detectedObj.tag == "Player")
                        //        elapsedTime = 0.0f;
                        //}

                        // Debug.Log("Mouse IsShocked Changed in line 197" + isShocked);
                        successfulTag = detectedObj.tag;
                    }
                    // if it is a running state when it detects a enemy
                    else if (characterState == STATE.RUNNING)
                    {
                        // if it detects a new enemy while running from the player
                        if (detectedObj != targetObject && targetObject.tag == "Player")
                        {
                            DIRECTIONS detectedObjDir = GetTargetDirection(detectedObj.transform.position);

                            // if the object detected is behind a wall
                            // skip this object because it's not a threat
                            if (CheckIfClear(detectedObjDir, detectedObj.transform.position) == false)
                            {
                                continue;
                            }

                            // it has to be shocked
                            isShocked = true;
                            // Debug.Log("Mouse IsShocked Changed in line 219" + isShocked);
                            StartCoroutine(PlayerShockUITime());
                        
                            StopMovement();
                            mouseMovement.StopMovement();
                            mouseMovement.ResetMovementList();
                        }
                    }

                }

                // If it spots the player or kitten
                // it will try to run away
                //if (detectedObj.tag == "Player" || detectedObj.tag == "Kitten" )
                //{
                //    // if it already has a target
                //    if (targetObject != null)
                //    {
                //        // If it detects the kitten while being chased by the player
                //        if (targetObject != detectedObj && targetObject.tag == "Player")
                //        {
                //            // Make it shocked
                //            isShocked = true;

                //            // stop both the movement
                //            StopMovement();
                //            mouseMovement.StopMovement();
                //            mouseMovement.ResetMovementList();
                //        }
                //    }

                //    // Set the player object as it's current target
                //    targetObject = detectedObj;

                //    targetDir = GetTargetDirection();

                //    // Check the direction if its clear
                //    // if it isn't then they detected an enemy through the wall
                //    //if (CheckIfClear(targetDir) == false && targetObject.tag != "Player")
                //    //{
                //    //    if (characterState == STATE.RUNNING)
                //    //    {
                //    //        mouseMovement.ResetMovementList();
                //    //        mouseMovement.StopMovement();
                //    //    }

                //    //    StopMovement();

                //    //    continue;
                //    //}

                //    // shocked
                //    if (characterState != STATE.RUNNING)
                //        isShocked = true;

                //}
            }

            //if (isShocked && characterState == STATE.IDLE)
            //{
            //    if (targetObject.tag == "Player")
            //        StartCoroutine(PlayerShockUITime());
            //}
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


            yield return new WaitForSeconds(.1f);
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

                if (characterState != STATE.RUNNING)
                    StartCoroutine(PlayerShockUITime());

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

            yield return new WaitForSeconds(.1f);
        }
    }

    IEnumerator PlayerShockUITime()
    {
        if (m_PlayerShockUI != null)
            m_PlayerShockUI.gameObject.SetActive(true);

        yield return new WaitForSeconds(m_PlayerShockUITime);

        if (m_PlayerShockUI != null)
            m_PlayerShockUI.gameObject.SetActive(false);
    }

}
