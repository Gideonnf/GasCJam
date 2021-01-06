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
    Vector2 targetTilePosition = Vector2.zero;
    // Keep track of the current moving direction
    Detection.DIRECTIONS movingDir;
    // How many spaces the current moving direction has
    int currentDirTileCounter = 0;


    // Start is called before the first frame update
    void Start()
    {
        mouseDetection = GetComponent<MouseDetection>();
        m_rigidBody = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        // if it is running
        if (mouseDetection.characterState == Detection.STATE.RUNNING)
        {
            // if no target tile position was assigned yet
            // set the target tile
            if (targetTilePosition == Vector2.zero)
            {
                // Get the moving Direction
                if (GetMovingDirection())
                    targetTilePosition = GetNextTile();

            }
            else
            {
                // get the direction to the next tile
                Vector3 direction = (targetTilePosition - (Vector2)transform.position).normalized;

                // it already has a target to move to

                // If it is moving along the X Axis
                if (movingDir == Detection.DIRECTIONS.LEFT || movingDir == Detection.DIRECTIONS.RIGHT)
                {
                    // get the x Position of the tile
                    float targetXPos = targetTilePosition.x;
                    // if it is negative
                    if (targetXPos <= 0.0f)
                    {
                        if (transform.position.x <= targetXPos)
                        {
                            // it reached the tile
                            targetReached = true;
                        }
                    }
                    // if its positive
                    else
                    {
                        if (transform.position.x >= targetXPos)
                        {
                            // it reached the tile
                            targetReached = true;
                        }
                    }
                }
                // if it is moving along the Y Axis
                else if (movingDir == Detection.DIRECTIONS.UP || movingDir == Detection.DIRECTIONS.DOWN)
                {
                    float targetYPos = targetTilePosition.y;

                    // if its negative
                    if (targetYPos <= 0.0f)
                    {
                        if (transform.position.y <= targetYPos)
                        {
                            // it reached the tile
                            targetReached = true;
                        }
                    }
                    // if its positive
                    else
                    {
                        if (transform.position.y >= targetYPos)
                        {
                            // it reached the tile
                            targetReached = true;
                        }
                    }
                }

                // it reached the tile
                if (targetReached == true)
                {
                    transform.position = targetTilePosition;
                    StopMovement();
                }


                // move it move it to the limit limit
                m_rigidBody.MovePosition(transform.position + direction * moveSpeed * Time.fixedDeltaTime);
            }

        }
    }

    public void StopMovement()
    {
        targetReached = false;
        movingDir = Detection.DIRECTIONS.NONE;
        targetTilePosition = Vector2.zero;
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
        if (mouseDetection.CheckForEnemies() == false)
        {
            StopMovement();
            mouseDetection.StopMovement();

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
                //if (targetTilePos.x < (currentTilePos.x - 1) || targetTilePos.x > (currentTilePos.x + 1))
                //{
                //    // if the number of empty tiles on the left is less than the right
                //    if (CheckDirectionSize(Detection.DIRECTIONS.LEFT) < CheckDirectionSize(Detection.DIRECTIONS.RIGHT))
                //    {
                //        // the moving direction should be right
                //        return Detection.DIRECTIONS.RIGHT;
                //    }
                //    else
                //    {
                //        // else the moving direction should be left
                //        return Detection.DIRECTIONS.LEFT;
                //    }
                //}
                //// else it will need to go in the direction that is further away from the target object
                //else
                //{
                    // if the target is on the left side
                    if (targetTilePos.x <= 0.0f)
                    {
                        tempDirection = Detection.DIRECTIONS.RIGHT;
                        //return Detection.DIRECTIONS.RIGHT;
                    }
                    else
                    // the target is on the right
                    {
                        tempDirection = Detection.DIRECTIONS.LEFT;

                        //return Detection.DIRECTIONS.LEFT;
                    }
                //}
            }
            // if its only checking along the Y Axis
            else if (currDirection == Detection.DIRECTIONS.UP || currDirection == Detection.DIRECTIONS.DOWN)
            {
                // if its more than 2 blocks to the left or more than 2 blocks to the right
                // then it can go left or right depending on which has more space
                //if (targetTilePos.y < (currentTilePos.y - 1) || targetTilePos.y > (currentTilePos.y + 1))
                //{
                //    // if the number of empty tiles on above is less than below
                //    if (CheckDirectionSize(Detection.DIRECTIONS.UP) < CheckDirectionSize(Detection.DIRECTIONS.DOWN))
                //    {
                //        // the moving direction should be right
                //        return Detection.DIRECTIONS.DOWN;
                //    }
                //    else
                //    {
                //        // else the moving direction should be left
                //        return Detection.DIRECTIONS.UP;
                //    }
                //}
                //else
                //{
                    // the target is below
                    if(targetTilePos.y <= 0.0f)
                    {
                        tempDirection = Detection.DIRECTIONS.UP;
                    }
                // the target is ontop
                else
                    {
                        tempDirection = Detection.DIRECTIONS.DOWN;
                    }
                //}
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
            return tempDirection;
        }


        return Detection.DIRECTIONS.NONE;
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
}