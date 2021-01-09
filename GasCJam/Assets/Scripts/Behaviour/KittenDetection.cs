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
    [Tooltip("Detection circle for Player")]
    public float PlayerDetectionRadius;

    bool mouseInSight = false;

    float elapsedTime;

    [Header("ShockUI")]
    public ExclaimationMarkUI m_ShockUI;

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
            if (characterState == STATE.TIRED)
            {
                isShocked = false;
            }

            //Debug.Log(elapsedTime);
            elapsedTime += Time.deltaTime;

            //Debug.Log(elapsedTime);

            // if its done getting shocked
            if (elapsedTime >= shockTime)
            {
                // change the state
                characterState = STATE.CHASING;

                isShocked = false;

                elapsedTime = 0;

                //get mouse pos
                Vector2Int ratGridPos = MapManager.Instance.GetWorldToTilePos(ratObject.transform.position);
                Vector2Int kittenGridPos = MapManager.Instance.GetWorldToTilePos(gameObject.transform.position);

                //check which direction its facing
                Vector2Int diff = ratGridPos - kittenGridPos;
                
                if (targetDir == DIRECTIONS.LEFT || targetDir == DIRECTIONS.RIGHT)
                {
                    int xOffset = 1;
                    if (targetDir == DIRECTIONS.LEFT)
                        xOffset = -1;

                    int diffX = Mathf.Abs(diff.x);
                    for (int i = 1; i <= diffX; ++i)
                    {
                        //Debug.Log("Added rat tile in Detection" + new Vector2Int(kittenGridPos.x + i * xOffset, kittenGridPos.y));
                        Vector2Int tileToAdd = new Vector2Int(kittenGridPos.x + i * xOffset, kittenGridPos.y);
                        if (kittenMovement.ListOfRatTiles.Contains(tileToAdd) == false)
                        {
                            kittenMovement.ListOfRatTiles.Add(new Vector2Int(kittenGridPos.x + i * xOffset, kittenGridPos.y));
                        }

                        //                      kittenMovement.ListOfTilesTravelled.Add(new Vector2Int(kittenGridPos.x + i * xOffset, kittenGridPos.y));
                    }
                }
                else
                {
                    int yOffset = 1;
                    if (targetDir == DIRECTIONS.DOWN)
                        yOffset = -1;

                    int diffY = Mathf.Abs(diff.y);
                    for (int i = 1; i <= diffY; ++i)
                    {
                        //Debug.Log("Added rat tile in Detection" + new Vector2Int(kittenGridPos.x, kittenGridPos.y + i * yOffset));
                        Vector2Int tileToAdd = new Vector2Int(kittenGridPos.x, kittenGridPos.y + i * yOffset);
                        if (kittenMovement.ListOfRatTiles.Contains(tileToAdd) == false)
                        {
                            kittenMovement.ListOfRatTiles.Add(new Vector2Int(kittenGridPos.x, kittenGridPos.y + i * yOffset));
                        }
                        //                       kittenMovement.ListOfTilesTravelled.Add(new Vector2Int(kittenGridPos.x, kittenGridPos.y + i * yOffset));
                    }
                }

                if (m_ShockUI != null)
                    m_ShockUI.gameObject.SetActive(false);
            }
            else
            {
                if (m_ShockUI != null)
                    m_ShockUI.UpdateExclaimationMarkUI(1.0f - (shockTime - elapsedTime) / shockTime);
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
                        if (m_ShockUI != null)
                            m_ShockUI.gameObject.SetActive(true);

                        kittenMovement.UpdateAnimation(false, targetDir);

                       // Debug.Log("Cat IsShocked Changed in line 123" + isShocked);

                    }

                    return true;
                }
            }
        }
        else
        {
            if (mouseInSight == false)
            {
                isShocked = false;
                if (m_ShockUI != null)
                    m_ShockUI.gameObject.SetActive(false);
            }

            //  Debug.Log("Cat IsShocked Changed in line 134" + isShocked);
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

        targetObject.transform.position = MapManager.Instance.GetTileToWorldPos(kittenMovement.startingPos);

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

    public override bool CheckForCharacters(CHARACTERS characterToCheck, Vector2Int tilePosition)
    {
        Vector2Int mouseTilePos;
        Vector2Int playerTilePos;

        if (characterToCheck == CHARACTERS.MOUSE)
        {
            mouseTilePos = MapManager.Instance.GetWorldToTilePos(ratObject.transform.position);
            if (mouseTilePos == tilePosition)
                return true;
        }
        else if (characterToCheck == CHARACTERS.PLAYER)
        {
            playerTilePos = MapManager.Instance.GetWorldToTilePos(playerObject.transform.position);
            if (playerTilePos == tilePosition)
                return true;
        }



        return false;
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
                if (!kittenMovement.m_Stop)
                {
                    kittenMovement.m_rigidBody.MovePosition(collider.gameObject.transform.position);

                    //if(DetectPlayer())
                    //{
                        SoundManager.Instance.Play("CaughtRat");
                        GameLevelManager.Instance.Win();
                    //}
                }
            }


        }

        return false;
    }

    public bool DetectPlayer()
    {
        // get the tile position
        Vector2Int playerTilePosition = MapManager.Instance.GetWorldToTilePos(playerObject.transform.position);

        // get the current tile position
        Vector2Int kittenTilePosition = MapManager.Instance.GetWorldToTilePos(transform.position);

        DIRECTIONS playerDirection = GetTargetDirection(playerObject.transform.position);

        if (CheckIfClear(playerDirection, playerObject.transform.position))
        {

            // if within 1 tiles 
            if (Vector2Int.Distance(playerTilePosition, kittenTilePosition) <= 1)
            {
                if (MapManager.Instance.IsThereTileOnMap(playerTilePosition) == false)
                {
                    DetectPlayerBehvaiour();
                    return true;
                }
            }
            // Checking for the diagonal
            else if (Vector2Int.Distance(playerTilePosition, kittenTilePosition) <= 2)
            {
                // Check if its above or below
                // if player  is less than the kitten y pos
                if (playerTilePosition.y < kittenTilePosition.y)
                {
                    // player is below
                    // Check if the tile below is empty
                    Vector2Int tileToCheck = kittenTilePosition;
                    tileToCheck.y -= 1;
                    if (MapManager.Instance.IsThereTileOnMap(tileToCheck) == false)
                    {
                        // if that tile is clear
                        // check if the player tile is clear
                        if (MapManager.Instance.IsThereTileOnMap(playerTilePosition) == false)
                        {
                            DetectPlayerBehvaiour();
                            return true;
                        }
                    }
                }
                if (playerTilePosition.y > kittenTilePosition.y)
                {
                    // it is ontop
                    Vector2Int tileToCheck = kittenTilePosition;
                    tileToCheck.y += 1;
                    if (MapManager.Instance.IsThereTileOnMap(tileToCheck) == false)
                    {
                        // if that tile is clear
                        // check if the player tile is clear
                        if (MapManager.Instance.IsThereTileOnMap(playerTilePosition) == false)
                        {
                            DetectPlayerBehvaiour();
                            return true;
                        }
                    }
                }
                if (playerTilePosition.x < kittenTilePosition.x)
                {
                    // it is ontop
                    Vector2Int tileToCheck = kittenTilePosition;
                    tileToCheck.x -= 1;
                    if (MapManager.Instance.IsThereTileOnMap(tileToCheck) == false)
                    {
                        // if that tile is clear
                        // check if the player tile is clear
                        if (MapManager.Instance.IsThereTileOnMap(playerTilePosition) == false)
                        {
                            DetectPlayerBehvaiour();
                            return true;
                        }
                    }
                }
                if (playerTilePosition.x > kittenTilePosition.x)
                {
                    // it is ontop
                    Vector2Int tileToCheck = kittenTilePosition;
                    tileToCheck.x += 1;
                    if (MapManager.Instance.IsThereTileOnMap(tileToCheck) == false)
                    {
                        // if that tile is clear
                        // check if the player tile is clear
                        if (MapManager.Instance.IsThereTileOnMap(playerTilePosition) == false)
                        {
                            DetectPlayerBehvaiour();
                            return true;
                        }
                    }
                }
                Debug.Log("its 2");
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

            DetectPlayer();

            // Check if the rat is touching the cat
            DetectRat();

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
            if (DetectInView(CHARACTERS.PLAYER) == true)
            {
                DetectPlayerBehvaiour();
            }

            if (DetectInView(CHARACTERS.MOUSE) == true)
            {
                if (characterState != STATE.TIRED)
                {

                    mouseInSight = true;

                    isShocked = true;

                    //Debug.Log("Cat IsShocked Changed in line 227" + isShocked);

                    targetObject = ratObject;

                    targetDir = GetTargetDirection();

                    if (characterState == STATE.IDLE)
                        kittenMovement.UpdateAnimation(false, targetDir);
                    //TODO:: put the exlaimation mark here
                }
            }
            else
            {
                mouseInSight = false;
            }
            yield return new WaitForSeconds(.2f);
        }
    }

    public void DetectPlayerBehvaiour()
    {
        if (kittenMovement.m_Stop)
            return;

        // lose the game
        targetObject = playerObject;
        targetDir = GetTargetDirection();

        kittenMovement.m_Stop = true;

        //show animation
        kittenMovement.UpdateAnimation(false, targetDir);
        kittenMovement.m_Animator.SetTrigger("Shock");

        GameLevelManager.Instance.KittenSeesCat();
    }
}
