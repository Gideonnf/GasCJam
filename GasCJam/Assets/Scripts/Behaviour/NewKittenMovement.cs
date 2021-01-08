using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewKittenMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Kitten movement Speed")]
    public float moveSpeed;
    [Tooltip("Starting position of the Cat")]
    public Vector2Int startingPos;
    [Tooltip("The max distance it can travel")]
    public int maxTravelDistance;
    [Tooltip("Minimum travel distance")]
    public int minTravelDistance;
    // Keep track if it reached the tile
    bool targetReached = false;
    // Keep track of the rigid body
    Rigidbody2D m_rigidBody;
    // Keep track of the kitten detection script
    KittenDetection kittenDetection;
    // Keep Track of mouse movement
    MouseMovement mouseMovement;
    // Keep track of the target tile position
    Vector2Int targetTilePosition = Vector2Int.zero;
    // Keep track of hte current moving direction
    Detection.DIRECTIONS movingDir;


    List<Vector2Int> ListOfTilesTravelled = new List<Vector2Int>();

    // Keep track of the tiles that the rat travelled
    List<Vector2Int> ListOfRatTiles = new List<Vector2Int>();

    int currentIndex = 0;

    int currentRatIndex = 0;

    Vector3 directionVector;

    [Header("Visual")]
    public Animator m_Animator;

    public bool m_Stop = false;

    // Start is called before the first frame update
    void Start()
    {
        kittenDetection = GetComponent<KittenDetection>();
        m_rigidBody = GetComponent<Rigidbody2D>();

        if (kittenDetection.ratObject != null)
            mouseMovement = kittenDetection.ratObject.GetComponent<MouseMovement>();

        startingPos = MapManager.Instance.GetWorldToTilePos(transform.position);

        ListOfTilesTravelled.Add(startingPos);

        Vector2 startingDir = Vector2.zero;
        switch (kittenDetection.startingDir)
        {
            case Detection.DIRECTIONS.UP:
                startingDir = new Vector2(0,1);
                break;
            case Detection.DIRECTIONS.DOWN:
                startingDir = new Vector2(0, -1);
                break;
            case Detection.DIRECTIONS.LEFT:
                startingDir = new Vector2(-1, 0);
                break;
            case Detection.DIRECTIONS.RIGHT:
                startingDir = new Vector2(1, 0);
                break;
        }

        UpdateAnimation(false, startingDir);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_Stop)
            return;

        // theres some issues with restarting
        // where this ends up being null
        if (mouseMovement == null)
            mouseMovement = kittenDetection.ratObject.GetComponent<MouseMovement>();


        // if the cat is currently chasing 
        if (kittenDetection.characterState == Detection.STATE.CHASING || kittenDetection.characterState == Detection.STATE.TIRED)
        {
            //Vector2 distanceVector = (Vector2)transform.position - startingPos;
            //float distanceFromStarting = distanceVector.magnitude;

            // Only check if it can get tired while chasing
            // this is so that it wont repeatedly check to see if its tired
            if (kittenDetection.characterState == Detection.STATE.CHASING)
            {
                // Check rat movement
                // If the target's tile position isn't a zero vector
                // it means it has a path
                if (mouseMovement.targetTilePosition != Vector2.zero)
                {
                    // if it doesnt contain the target tile position
                    if (ListOfRatTiles.Contains(mouseMovement.targetTilePosition) == false)
                    {
                        // add it to the list
                        ListOfRatTiles.Add(mouseMovement.targetTilePosition);
                    }
                }

                if (currentIndex  > maxTravelDistance)
                {
                    // if the mouse is tired
                    // clear the list of tiles because it doesnt need to chase anymore
                    ListOfRatTiles.Clear();
                    currentRatIndex = 0;

                    // To clear target object and reset character state
                    kittenDetection.StopMovement();

                    kittenDetection.characterState = Detection.STATE.TIRED;
                    
                    // Take away the last tile 
                    // the last tile is its last position
                    //currentIndex -= 2;

                    //currentIndex = ListOfTilesTravelled.Count - 1;

                    StopMovement();

                    UpdateAnimation(false);

                    return;
                }
            }

            // if there is no target tile yet
            // find the target tile
            if (targetTilePosition == Vector2.zero)
            {
                // If the kitten is tired
                // move based on the list of tiles instead of a target object
                if (kittenDetection.characterState == Detection.STATE.TIRED)
                {
                    // im gonna need to change this later probably
                    if (currentIndex <= 0)
                    {
                        transform.position = MapManager.Instance.GetTileToWorldPos(startingPos);

                        UpdateAnimation(false);
                        kittenDetection.characterState = Detection.STATE.IDLE;

                        //Clear the list
                        ListOfTilesTravelled.Clear();
                        // add the starting position
                        ListOfTilesTravelled.Add(startingPos);
                        // reset the index
                        //currentIndex = 1;

                        StopMovement();

                        return;
                    }
                    // set the target to the last position on the list
                    targetTilePosition = ListOfTilesTravelled[currentIndex - 1];

                    Vector2Int Vec2Direction = (targetTilePosition - MapManager.Instance.GetWorldToTilePos(transform.position));
                    directionVector = new Vector3(Vec2Direction.x, Vec2Direction.y, 0);

                    UpdateAnimation(true);

                    currentIndex--;

                    // Get the direction to the next tile
                    movingDir = kittenDetection.GetTargetDirection(targetTilePosition);
                    // Set the viewing direction to be the same as the kitten's moving dir
                    kittenDetection.SetViewDirection(movingDir);
                }
                else
                {
                    // if there are tiles to follow
                    if(ListOfRatTiles.Count > 0)
                    {
                        if (currentRatIndex < ListOfRatTiles.Count)
                        {
                            // set the target tiles
                            targetTilePosition = ListOfRatTiles[currentRatIndex];
                            Vector2Int Vec2Direction = (targetTilePosition - MapManager.Instance.GetWorldToTilePos(transform.position));
                            directionVector = new Vector3(Vec2Direction.x, Vec2Direction.y, 0);
                            UpdateAnimation(true);

                            // increment the rat index to keep track 
                            currentRatIndex++;
                            
                            // Add it to the list of travelled tiles
                            ListOfTilesTravelled.Add(targetTilePosition);
                            // increment the curent index also
                            currentIndex++;

                            // Get the moving direction
                            movingDir = kittenDetection.GetTargetDirection(targetTilePosition);

                            kittenDetection.SetViewDirection(movingDir);
                        }
                    }
                    else
                    {
                        // if it is not in its starting pos
                        if (MapManager.Instance.GetWorldToTilePos(transform.position) != startingPos)
                        {
                            kittenDetection.characterState = Detection.STATE.TIRED;
                        }
                    }

                    ////Get the moving Direction
                    //if (GetMovingDirection())
                    //{
                    //    targetTilePosition = GetNextTile();
                    //    directionVector = (targetTilePosition - (Vector2)transform.position).normalized;
                    //    UpdateAnimation(true);

                    //    // For checking if the kitten is tired
                    //    // Store the tiles it traversed in a list
                    //    ListOfTilesTravelled.Add(targetTilePosition);
                    //    // Increment the index everytime it changes
                    //    currentIndex++;
                        
                    //}
                }
            }
            else
            {
                // If it is moving along the X Axis
                if (movingDir == Detection.DIRECTIONS.LEFT)
                {
                    float targetXPos = targetTilePosition.x;
                    float currentXPos = MapManager.Instance.GetWorldToTilePos(transform.position).x;

                    //Debug.Log("kitten target tile position" + targetTilePosition);
                    //Debug.Log("kitten position" + transform.position);

                    if (currentXPos <= targetXPos)
                    {
                        // it reached the tile
                        targetReached = true;
                    }
                }
                else if (movingDir == Detection.DIRECTIONS.RIGHT)
                {
                    float targetXPos = targetTilePosition.x;
                    float currentXPos = MapManager.Instance.GetWorldToTilePos(transform.position).x;

                    if (currentXPos >= targetXPos)
                    {
                        // it reached the tile
                        targetReached = true;
                    }
                }
                else if (movingDir == Detection.DIRECTIONS.UP)
                {
                    float targetYPos = targetTilePosition.y;
                    float currentYPos = MapManager.Instance.GetWorldToTilePos(transform.position).y;

                    if (currentYPos >= targetYPos)
                    {
                        targetReached = true;
                    }
                }
                else if (movingDir == Detection.DIRECTIONS.DOWN)
                {
                    float targetYPos = targetTilePosition.y;
                    float currentYPos = MapManager.Instance.GetWorldToTilePos(transform.position).y;

                    if (currentYPos <= targetYPos)
                    {
                        targetReached = true;
                    }
                }
                else if (movingDir == Detection.DIRECTIONS.NONE)
                {
                    // it shouldn't reach here lol
                    targetTilePosition = Vector2Int.zero;
                }

                if (targetReached == true)
                {
                    // it reached the tile
                   // transform.position = MapManager.Instance.GetTileToWorldPos(targetTilePosition);
                    StopMovement();
                }


                

                m_rigidBody.MovePosition(transform.position + directionVector * moveSpeed * Time.fixedDeltaTime);
            }
        }
    }

    /// <summary>
    /// for resetting movements
    /// </summary>
    public void StopMovement()
    {
        targetReached = false;
        movingDir = Detection.DIRECTIONS.NONE;
        targetTilePosition = Vector2Int.zero;
    }

    /// <summary>
    /// Check which direction to move inbetween every tile
    /// </summary>
    /// <returns> true if it found a direction </returns>
    bool GetMovingDirection()
    {
        if(CheckDirection(kittenDetection.targetDir))
        {
            movingDir = kittenDetection.targetDir;
        }
        else
        {
            // Check the direction of the target
            // there is no point checking the opposite of the target
            // the kitten is suppose to be chasing

            switch (kittenDetection.targetDir)
            {
                case Detection.DIRECTIONS.UP:
                    movingDir = GetAltDirection(Detection.DIRECTIONS.DOWN, kittenDetection.targetDir);
                    
                    break;
                case Detection.DIRECTIONS.DOWN:
                    movingDir = GetAltDirection(Detection.DIRECTIONS.UP, kittenDetection.targetDir);

                    break;
                case Detection.DIRECTIONS.LEFT:
                    movingDir = GetAltDirection(Detection.DIRECTIONS.RIGHT, kittenDetection.targetDir);
                    
                    break;
                case Detection.DIRECTIONS.RIGHT:
                    movingDir = GetAltDirection(Detection.DIRECTIONS.LEFT, kittenDetection.targetDir);
                   
                    break;
                case Detection.DIRECTIONS.NONE:
                    break;
                default:
                    break;
            }
        }

        if (movingDir != Detection.DIRECTIONS.NONE)
            return true;

        return false;
    }


    /// <summary>
    /// Checks a direction
    /// </summary>
    /// <param name="dirToCheck">The direction to check</param>
    /// <returns>True if the path is clear</returns>
    bool CheckDirection(Detection.DIRECTIONS dirToCheck)
    {
        // Get the current tile position
        Vector2Int currentTilePos = MapManager.Instance.GetWorldToTilePos(transform.position);

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

        if (MapManager.Instance.IsThereTileOnMap(currentTilePos) == false)
            return true;

        return false;
    }

    /// <summary>
    /// Check how many empty tiles there are in a direction
    /// </summary>
    /// <returns>The number of empty tiles</returns>
    int CheckDirectionSize(Detection.DIRECTIONS dirToCheck)
    {
        int currentTileCounter = 0;
        Vector2Int currentTilePos = MapManager.Instance.GetWorldToTilePos(transform.position);

        // while the path is clear
        // keep checking to find out how many
        // will take the direction with the longest clear path
        while (MapManager.Instance.IsThereTileOnMap(currentTilePos) == false)
        {
            currentTileCounter++;

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
                default:
                    Debug.LogError("It shouldn't reach here lol");
                    break;
            }
        }

        return currentTileCounter;
    }

    Vector2 GetNextTile()
    {
        // get the current tile position
        Vector2Int currentTilePos = MapManager.Instance.GetWorldToTilePos(transform.position);

        switch (movingDir)
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

        // returns the next tile in that direction
        return MapManager.Instance.GetTileToWorldPos(currentTilePos);


    }

    Detection.DIRECTIONS GetAltDirection(Detection.DIRECTIONS movingDir, Detection.DIRECTIONS targetDir)
    {

        Detection.DIRECTIONS tempDirection = Detection.DIRECTIONS.NONE;
        Vector2Int currentTilePos = MapManager.Instance.GetWorldToTilePos(transform.position);

        // loop through the directions
        for(int index = 0; index < (int)Detection.DIRECTIONS.NONE; ++index)
        {
            // dont check the other 2 directions
            if (index == (int)movingDir)
                continue;
            else if (index == (int)targetDir)
                continue;

            Detection.DIRECTIONS currDirection = (Detection.DIRECTIONS)index;

            Vector2Int targetTilePos = Vector2Int.zero;

            if (kittenDetection.targetObject != null)
                targetTilePos = MapManager.Instance.GetWorldToTilePos(kittenDetection.targetObject.transform.position);
            else
                return Detection.DIRECTIONS.NONE;


            // if its only chcking along the X Axis
            if (currDirection == Detection.DIRECTIONS.LEFT || currDirection == Detection.DIRECTIONS.RIGHT)
            {
                // if its more than 2 blocks to the left or more than 2 blocks to the right
                // then it can go left or right depending on which has more space
                //                if (Mathf.Abs(targetTilePos.x) < Mathf.Abs(currentTilePos.x - 1) || Mathf.Abs(targetTilePos.x) > Mathf.Abs(currentTilePos.x + 1))
                if (targetTilePos.x > (currentTilePos.x - 1) && targetTilePos.x < (currentTilePos.x + 1))
                {
                    // if the number of empty tiles on the left is less than the right
                    if (CheckDirectionSize(Detection.DIRECTIONS.LEFT) < CheckDirectionSize(Detection.DIRECTIONS.RIGHT))
                    {
                        // the moving direction should be right
                        Debug.Log("Moving Right");
                        tempDirection = Detection.DIRECTIONS.RIGHT;
                    }
                    else
                    {
                        // else the moving direction should be left
                        Debug.Log("Moving Left");
                        tempDirection = Detection.DIRECTIONS.LEFT;
                    }
                }
                // else it will need to go in the direction that is further away from the target object
                else
                {
                    // if the target is on the left side
                    if (targetTilePos.x <= transform.position.x)
                    {
                        Debug.Log("Moving Right 2");

                        tempDirection = Detection.DIRECTIONS.RIGHT;
                        //return Detection.DIRECTIONS.RIGHT;
                    }
                    else
                    // the target is on the right
                    {
                        Debug.Log("Moving Left 2");

                        tempDirection = Detection.DIRECTIONS.LEFT;

                        //return Detection.DIRECTIONS.LEFT;
                    }
                }
            }
            // if its only checking along the Y Axis
            else if (currDirection == Detection.DIRECTIONS.UP || currDirection == Detection.DIRECTIONS.DOWN)
            {
                // if its more than 2 blocks to the left or more than 2 blocks to the right
                // then it can go left or right depending on which has more space
                if (targetTilePos.y < (currentTilePos.y - 1) || targetTilePos.y > (currentTilePos.y + 1))
                {
                    // if the number of empty tiles on above is less than below
                    if (CheckDirectionSize(Detection.DIRECTIONS.UP) < CheckDirectionSize(Detection.DIRECTIONS.DOWN))
                    {
                        // the moving direction should be right
                        tempDirection = Detection.DIRECTIONS.DOWN;
                    }
                    else
                    {
                        // else the moving direction should be left
                        tempDirection = Detection.DIRECTIONS.UP;
                    }
                }
                else
                {
                    // the target is below
                    if (targetTilePos.y <= transform.position.y)
                    {
                        tempDirection = Detection.DIRECTIONS.UP;
                    }
                    // the target is ontop
                    else
                    {
                        tempDirection = Detection.DIRECTIONS.DOWN;
                    }
                }
            }

            // might not need this
            // might need this
            // i dont know
            break;
        }


        switch (tempDirection)
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

        if (MapManager.Instance.IsThereTileOnMap(currentTilePos) == false)
            return tempDirection;

        return Detection.DIRECTIONS.NONE;
    }

    public void UpdateAnimation(bool isMoving)
    {
        m_Animator.SetBool("IsMoving", isMoving);
        m_Animator.SetFloat("Horizontal", directionVector.x);
        m_Animator.SetFloat("Vertical", directionVector.y);

        m_Animator.SetBool("Crying", kittenDetection.characterState == Detection.STATE.TIRED);
    }

    public void UpdateAnimation(bool isMoving, Vector2 dirFacing)
    {
        m_Animator.SetBool("IsMoving", isMoving);
        m_Animator.SetFloat("Horizontal", dirFacing.x);
        m_Animator.SetFloat("Vertical", dirFacing.y);

        m_Animator.SetBool("Crying", kittenDetection.characterState == Detection.STATE.TIRED);
    }

    public void UpdateAnimation(bool isMoving, Detection.DIRECTIONS dir)
    {
        Vector2 startingDir = Vector2.zero;
        switch (kittenDetection.startingDir)
        {
            case Detection.DIRECTIONS.UP:
                startingDir = new Vector2(0, 1);
                break;
            case Detection.DIRECTIONS.DOWN:
                startingDir = new Vector2(0, -1);
                break;
            case Detection.DIRECTIONS.LEFT:
                startingDir = new Vector2(-1, 0);
                break;
            case Detection.DIRECTIONS.RIGHT:
                startingDir = new Vector2(1, 0);
                break;
        }

        UpdateAnimation(isMoving, startingDir);
    }

    public void Shock()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Prey")
        {
            Debug.Log("TOUCHING THE MOUSE AHFAUHAHFH");

            return;
        }
    }
}
