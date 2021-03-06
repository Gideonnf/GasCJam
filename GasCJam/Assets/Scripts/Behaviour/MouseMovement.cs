﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    [Tooltip("Mouse MovementSpeed")]
    public float moveSpeed;
    // Keep track if it reached the tile
    bool targetReached = false;
    // Keep track of the rigid body
    Rigidbody2D m_rigidBody;
    // Keep track of the detection script
    MouseDetection mouseDetection;
    // Keep track of the target tile position
    [System.NonSerialized] public Vector2Int targetTilePosition = Vector2Int.zero;
    // Keep track of the current moving direction
    public Detection.DIRECTIONS movingDir;

    // Store a list of tiles movable in the moving direction
    List<Vector2Int> ListOfMovableTiles = new List<Vector2Int>();
    int currentIndex;

    Vector3 directionVector;

    [Header("Visual")]
    public Animator m_Animator;

    public bool m_Stop = false;

    bool m_NextTileIsZeroPos = false;

    bool isTrappedMovement = false;

    // Start is called before the first frame update
    void Start()
    {
        mouseDetection = GetComponent<MouseDetection>();
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_Stop = false;


        Vector2 startingDir = Vector2.zero;
        switch (mouseDetection.startingDir)
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

        UpdateAnimation(false, startingDir);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_Stop)
            return;

        //Debug.Log("current position" + transform.position);
        //Debug.Log("target tile position " + targetTilePosition);

        if (mouseDetection.characterState == Detection.STATE.RUNNING)
        {
            // if the list is currently empty
            if (ListOfMovableTiles.Count <= 0)
            {
                if (mouseDetection.isTrapped)
                {
                    // it found an alternate path to run to
                    if (GetTrappedMovingDirection())
                    {
                        GetDirectionalTiles(movingDir);
                        // no trapped movement
                        isTrappedMovement = false;
                    }

                    // if is trapped movement
                    // means we bounce
                    if (isTrappedMovement)
                    {
                        GetNextTrapTilePosition(movingDir);
                    }
                }
                else
                {
                    // Get the direction to move towards
                    if (GetMovingDirection())
                    {
                        // If it managed to find a direction
                        // Fill up the list of tiles in that direction
                        GetDirectionalTiles(movingDir);
                    }
                }
            }

          

            Vector3 nextPos = transform.position + directionVector * moveSpeed * Time.fixedDeltaTime;
            if (targetTilePosition != Vector2.zero || (targetTilePosition == Vector2.zero && m_NextTileIsZeroPos))
            {
                // If it is moving along the X Axis
                if (movingDir == Detection.DIRECTIONS.LEFT)
                {
                    float targetXPos = MapManager.Instance.GetTileToWorldPos(targetTilePosition).x;

                    //Debug.Log("kitten target tile position" + targetTilePosition);
                    //Debug.Log("kitten position" + transform.position);

                    if (nextPos.x <= targetXPos)
                    {
                        // it reached the tile
                        targetReached = true;
                    }
                }
                else if (movingDir == Detection.DIRECTIONS.RIGHT)
                {
                    float targetXPos = MapManager.Instance.GetTileToWorldPos(targetTilePosition).x;

                    if (nextPos.x >= targetXPos)
                    {
                        // it reached the tile
                        targetReached = true;
                    }
                }
                else if (movingDir == Detection.DIRECTIONS.UP)
                {
                    float targetYPos = MapManager.Instance.GetTileToWorldPos(targetTilePosition).y;

                    if (nextPos.y >= targetYPos)
                    {
                        targetReached = true;
                    }
                }
                else if (movingDir == Detection.DIRECTIONS.DOWN)
                {
                    float targetYPos = MapManager.Instance.GetTileToWorldPos(targetTilePosition).y;

                    if (nextPos.y <= targetYPos)
                    {
                        targetReached = true;
                    }
                }
                else if (movingDir == Detection.DIRECTIONS.NONE)
                {
                    targetTilePosition = Vector2Int.zero;
                }
                // it reached the tile
                if (targetReached == true)
                {
                    //transform.position = MapManager.Instance.GetTileToWorldPos(targetTilePosition);
                    m_rigidBody.MovePosition(MapManager.Instance.GetTileToWorldPos(targetTilePosition));
                    StopMovement();
                    return;
                }
            }

            // if it has no target vector position to go to now
            if (targetTilePosition == Vector2.zero && !m_NextTileIsZeroPos)
            {
                // if it reached the end of the movable tiles
                // theres no more to move
                if(currentIndex >= ListOfMovableTiles.Count || mouseDetection.IsEnemyNearby(Detection.CHARACTERS.PLAYER))
                {
                    //transform.position = MapManager.Instance.GetTileToWorldPos(ListOfMovableTiles[currentIndex - 1]);

                    ListOfMovableTiles.Clear();
                    currentIndex = 0;
                    StopMovement();
                    mouseDetection.StopMovement();
                    movingDir = Detection.DIRECTIONS.NONE;

                    UpdateAnimation(false);

                    return;
                }

                // Set the target tile position to the first index of the list
                targetTilePosition = ListOfMovableTiles[currentIndex];

                //TODO:: NEED CHECK IF ITS REALLY JUST 0,0
                m_NextTileIsZeroPos = targetTilePosition == Vector2.zero;

                Vector2Int Vec2Direction = (targetTilePosition - MapManager.Instance.GetWorldToTilePos(transform.position));
                directionVector = new Vector3(Vec2Direction.x, Vec2Direction.y, 0);
                UpdateAnimation(true);

                // increment the index
                currentIndex++;
            }

            if (targetTilePosition != Vector2.zero)
            {
                PlayWalkingSound();
            }

            // move it move it to the limit limit
            m_rigidBody.MovePosition(transform.position + directionVector * moveSpeed * Time.fixedDeltaTime);
        }
    }

    public void ResetMovementList()
    {
        ListOfMovableTiles.Clear();
        currentIndex = 0;
    }

    /// <summary>
    ///  Gets all the tiles in that direction
    /// </summary>
    /// <param name="dir">The direction to check with</param>
    /// <returns>True if they</returns>
    public bool GetDirectionalTiles(Detection.DIRECTIONS dir)
    {
        // Get the curernt tile position
        Vector2Int currentTile = MapManager.Instance.GetWorldToTilePos(transform.position);

        // Moove once in the direction
        switch (dir)
        {
            case Detection.DIRECTIONS.UP:
                currentTile.y++;
                break;
            case Detection.DIRECTIONS.DOWN:
                currentTile.y--;
                break;
            case Detection.DIRECTIONS.LEFT:
                currentTile.x--;
                break;
            case Detection.DIRECTIONS.RIGHT:
                currentTile.x++;
                break;
            case Detection.DIRECTIONS.NONE:
                break;
            default:
                break;
        }

        // Keep on checking until it finds the wall
        while(MapManager.Instance.IsThereTileOnMap(currentTile) == false)
        {
            //Vector2 tileWorldPosition = MapManager.Instance.GetTileToWorldPos(currentTile);
            // Add the tile to the list
            ListOfMovableTiles.Add(currentTile);

            // Increment again by one
            // to check the next tile
            switch (dir)
            {
                case Detection.DIRECTIONS.UP:
                    currentTile.y++;
                    break;
                case Detection.DIRECTIONS.DOWN:
                    currentTile.y--;
                    break;
                case Detection.DIRECTIONS.LEFT:
                    currentTile.x--;
                    break;
                case Detection.DIRECTIONS.RIGHT:
                    currentTile.x++;
                    break;
                case Detection.DIRECTIONS.NONE:
                    break;
                default:
                    break;
            }
        }

        return true;
    }

    public void StopMovement(bool SetToTargetPosition = false)
    {
        if (SetToTargetPosition)
            transform.position = MapManager.Instance.GetTileToWorldPos(targetTilePosition);
        else
            targetTilePosition = Vector2Int.zero;

        m_NextTileIsZeroPos = false;

        targetReached = false;
        //movingDir = Detection.DIRECTIONS.NONE;
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
        while(MapManager.Instance.IsThereTileOnMap(currentTilePos) == false)
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

    Detection.DIRECTIONS GetAltDirection(Detection.DIRECTIONS movingDir, Detection.DIRECTIONS targetDir)
    {
        // Before checking for alternate path
        // check if it still needs to move
        // if there isn't then you don't have to find an alternate path
        if (mouseDetection.CheckForEnemies() == false && mouseDetection.playerInSight == false)
        {
            StopMovement();
            mouseDetection.StopMovement();
            UpdateAnimation(false);

            return Detection.DIRECTIONS.NONE;
        }

        Detection.DIRECTIONS tempDirection = Detection.DIRECTIONS.NONE;
        Vector2Int currentTilePos = MapManager.Instance.GetWorldToTilePos(transform.position);

        // loop through all the 4 directions
        for (int index = 0; index < (int)Detection.DIRECTIONS.NONE; ++index)
        {
            // dont check the other 2 directions
            if (index == (int)movingDir)
                continue;
            else if (index == (int)targetDir)
                continue;

            Detection.DIRECTIONS currDirection = (Detection.DIRECTIONS)index;
           
            Vector2Int targetTilePos = Vector2Int.zero;
            if (mouseDetection.targetObject != null)
                targetTilePos = MapManager.Instance.GetWorldToTilePos(mouseDetection.targetObject.transform.position);
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
//
                        tempDirection = Detection.DIRECTIONS.RIGHT;
                    }
                    else
                    {
                        // else the moving direction should be left
                        //Debug.Log("Moving Left");
                        tempDirection = Detection.DIRECTIONS.LEFT;
                    }
                }
                // else it will need to go in the direction that is further away from the target object
                else
                {
                    // if the target is on the left side
                    if (targetTilePos.x <= transform.position.x)
                    {
                       // Debug.Log("Moving Right 2");

                        tempDirection = Detection.DIRECTIONS.RIGHT;

                        if (CheckDirectionSize(Detection.DIRECTIONS.RIGHT) <= 1)
                            tempDirection = Detection.DIRECTIONS.LEFT;
                        //return Detection.DIRECTIONS.RIGHT;
                    }
                    else
                    // the target is on the right
                    {
                       // Debug.Log("Moving Left 2");

                        tempDirection = Detection.DIRECTIONS.LEFT;

                        if (CheckDirectionSize(Detection.DIRECTIONS.LEFT) <= 1)
                            tempDirection = Detection.DIRECTIONS.RIGHT;

                        //return Detection.DIRECTIONS.LEFT;
                    }
                }
            }
            // if its only checking along the Y Axis
            else if (currDirection == Detection.DIRECTIONS.UP || currDirection == Detection.DIRECTIONS.DOWN)
            {
                // if its more than 2 blocks to the left or more than 2 blocks to the right
                // then it can go left or right depending on which has more space
               // if (targetTilePos.y < (currentTilePos.y - 1) || targetTilePos.y > (currentTilePos.y + 1))
                if (targetTilePos.y > (currentTilePos.y - 1) && targetTilePos.y < (currentTilePos.y + 1))
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
                    //TODO::DUCTTAPE TO THE MAX, MAKE SURE THERES SPACE BEFORE RUNNING
                    if (targetTilePos.y <= transform.position.y)
                    {
                        tempDirection = Detection.DIRECTIONS.UP;

                        //if up no space
                        if (CheckDirectionSize(Detection.DIRECTIONS.UP) <= 1)
                            tempDirection = Detection.DIRECTIONS.DOWN;
                    }
                    // the target is ontop
                    else
                    {
                        tempDirection = Detection.DIRECTIONS.DOWN;

                        if (CheckDirectionSize(Detection.DIRECTIONS.DOWN) <= 1)
                            tempDirection = Detection.DIRECTIONS.UP;
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
        {
            // Check if the kitten is along that direction
            if (mouseDetection.CheckForKitten(tempDirection) == false)
                return tempDirection;
        }


        return Detection.DIRECTIONS.NONE;
    }

    Detection.DIRECTIONS GetTrappedRunningDir(Detection.DIRECTIONS playerDirection, Detection.DIRECTIONS kittenDirection)
    {

        Detection.DIRECTIONS tempDirection = Detection.DIRECTIONS.NONE;
        // Detection.DIRECTIONS tempDirection = Detection.DIRECTIONS.NONE;
        int tempDirectionSize = 0;

        Vector2Int currentTilePos = MapManager.Instance.GetWorldToTilePos(transform.position);

        // loop through all the 4 directions
        for (int index = 0; index < (int)Detection.DIRECTIONS.NONE; ++index)
        {
            // dont check the other 2 directions
            if (index == (int)playerDirection)
                continue;
            else if (index == (int)kittenDirection)
                continue;

            // the current direction being checked
            Detection.DIRECTIONS currDirection = (Detection.DIRECTIONS)index;
            int currDirectionSize = 0;

            currDirectionSize = CheckDirectionSize(currDirection);

            // if theres more spaces in this direction
            if (currDirectionSize >= tempDirectionSize)
            {
                // set the direction size
                tempDirectionSize = currDirectionSize;
                tempDirection = currDirection;
            }
        }

        // if tempDirectionSize is too small
        if (tempDirectionSize > 1)
            return tempDirection;

        return Detection.DIRECTIONS.NONE;
    }


    /// <summary>
    /// only used for when it is trapped
    /// </summary>
    /// <returns></returns>
    bool GetTrappedMovingDirection()
    {
        Detection.DIRECTIONS kittenDirection = mouseDetection.GetTargetDirection(mouseDetection.kittenObject.transform.position);
        Detection.DIRECTIONS playerDirection = mouseDetection.GetTargetDirection(mouseDetection.playerObject.transform.position);

        Detection.DIRECTIONS tempDir = Detection.DIRECTIONS.NONE;

        tempDir = GetTrappedRunningDir(playerDirection, kittenDirection);

        // theres no other path
        if (tempDir == Detection.DIRECTIONS.NONE)
        {
            // idk what to do here yet
            // this is where it needs to do bouncing back and forth
            isTrappedMovement = true;

            // set a random moving direction to start the bounce
            movingDir = playerDirection;
        }
        else
        {
            movingDir = tempDir;
            return true;
        }

        return false;
    }

    /// <summary>
    /// This is for the bouncing back
    /// </summary>
    /// <param name="currentDirection"></param>
    void GetNextTrapTilePosition(Detection.DIRECTIONS currentDirection)
    {
        // Get the first tile in that direction
        // i repeated the lines just in case lol
        Vector2Int targetTilePosition = MapManager.Instance.GetWorldToTilePos(transform.position);
        Vector2Int currentTilePosition = MapManager.Instance.GetWorldToTilePos(transform.position);

        switch (currentDirection)
        {
            case Detection.DIRECTIONS.UP:
                targetTilePosition.y++;
                break;
            case Detection.DIRECTIONS.DOWN:
                targetTilePosition.y--;
                break;
            case Detection.DIRECTIONS.LEFT:
                targetTilePosition.x--;
                break;
            case Detection.DIRECTIONS.RIGHT:
                targetTilePosition.x++;
                break;
            case Detection.DIRECTIONS.NONE:
                break;
            default:
                break;
        }

        // set it to that tile
        // Add the tile that it needs to move to
        ListOfMovableTiles.Add(targetTilePosition);
        // add the original tile it was in
        ListOfMovableTiles.Add(currentTilePosition);

        Detection.DIRECTIONS kittenDirection = mouseDetection.GetTargetDirection(mouseDetection.kittenObject.transform.position);
        Detection.DIRECTIONS playerDirection = mouseDetection.GetTargetDirection(mouseDetection.playerObject.transform.position);

        // change to the next direction
        // loop through all the 4 directions
        for (int index = 0; index < (int)Detection.DIRECTIONS.NONE; ++index)
        {
            // dont check the other 2 directions
            if (index == (int)playerDirection)
                continue;
            else if (index == (int)kittenDirection)
                continue;
            else if (index == (int)movingDir)
                continue;

            // set the direction
            movingDir = (Detection.DIRECTIONS)index;

            break;
        }
    }


        /// <summary>
        /// Checks which direction to move before it starst to move to the next tile
        /// 
        /// </summary>
        bool GetMovingDirection()
    {
        switch (mouseDetection.targetDir)
        {
            case Detection.DIRECTIONS.UP:
                // If the target is coming from Above
                // he'll want to move Down
                if (CheckDirection(Detection.DIRECTIONS.DOWN))
                {
                    movingDir = Detection.DIRECTIONS.DOWN;
                }
                else
                {
                    movingDir = GetAltDirection(Detection.DIRECTIONS.DOWN, mouseDetection.targetDir);
                }
                break;
            case Detection.DIRECTIONS.DOWN:
                // If target is coming from below
                // he'll want to move up
                if (CheckDirection(Detection.DIRECTIONS.UP))
                {
                    movingDir = Detection.DIRECTIONS.UP;
                }
                else
                {
                    movingDir = GetAltDirection(Detection.DIRECTIONS.UP, mouseDetection.targetDir);
                }
                break;
            case Detection.DIRECTIONS.LEFT:
                // If target coming from the left, move right
                if (CheckDirection(Detection.DIRECTIONS.RIGHT))
                {
                    movingDir = Detection.DIRECTIONS.RIGHT;
                }
                else
                {
                    movingDir = GetAltDirection(Detection.DIRECTIONS.RIGHT, mouseDetection.targetDir);
                }
                break;
            case Detection.DIRECTIONS.RIGHT:
                // if target coming from right, move left
                if (CheckDirection(Detection.DIRECTIONS.LEFT))
                {
                    movingDir = Detection.DIRECTIONS.LEFT;
                }
                else
                {
                    movingDir = GetAltDirection(Detection.DIRECTIONS.LEFT, mouseDetection.targetDir);
                }
                break;
            case Detection.DIRECTIONS.NONE:
                Debug.Log("O no it shudnt be here o no no no");
                break;
            default:
                break;
        }

        if (movingDir == Detection.DIRECTIONS.NONE)
            return false;

        return true;
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

    public void UpdateAnimation(bool isMoving)
    {
        m_Animator.SetBool("Moving", isMoving);
        m_Animator.SetFloat("Horizontal", directionVector.x);
        m_Animator.SetFloat("Vertical", directionVector.y);
    }

    public void UpdateAnimation(bool isMoving, Vector2 dirFacing)
    {
        m_Animator.SetBool("Moving", isMoving);
        m_Animator.SetFloat("Horizontal", dirFacing.x);
        m_Animator.SetFloat("Vertical", dirFacing.y);
    }

    public void PlayWalkingSound()
    {
        SoundManager.Instance.Play("RatWalk");
    }

    // TODO: Add a On collide enter with the rock
    // if the rat collides
    // then clear the movable tiles and set its position to the last position
    // this is cause hte cat will keep trying to move into the rock

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if it collided with an obstacle
        if (collision.gameObject.tag == "Obstacles")
        {
            // onl yif it is running
            if (mouseDetection.characterState == Detection.STATE.RUNNING)
            {
                // set the position to the last tile
                // - 1 cause the current index is gonna be pointing to the next tile
                Vector2Int LastTilePosition = ListOfMovableTiles[currentIndex - 1];
                m_rigidBody.MovePosition(MapManager.Instance.GetTileToWorldPos(LastTilePosition));

                // clear the list of movable tiles
                ListOfMovableTiles.Clear();
                currentIndex = 0;
                StopMovement();
                mouseDetection.StopMovement();
                movingDir = Detection.DIRECTIONS.NONE;

                UpdateAnimation(false);

            }
        }
    }
}
