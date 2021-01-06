using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewKittenMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Kitten movement Speed")]
    public float moveSpeed;
    [Tooltip("Starting position of the Cat")]
    public Vector2 startingPos;
    [Tooltip("The max distance it can travel")]
    public float maxTravelDistance;
    [Tooltip("Minimum travel distance")]
    public float minTravelDistance;
    // Keep track if it reached the tile
    bool targetReached = false;
    // Keep track of the rigid body
    Rigidbody2D m_rigidBody;
    // Keep track of the kitten detection script
    KittenDetection kittenDetection;
    // Keep track of the target tile position
    Vector2 targetTilePosition = Vector2.zero;
    // Keep track of hte current moving direction
    Detection.DIRECTIONS movingDir;

    Vector3 directionVector;

    // Start is called before the first frame update
    void Start()
    {
        kittenDetection = GetComponent<KittenDetection>();
        m_rigidBody = GetComponent<Rigidbody2D>();
        startingPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // if the cat is currently chasing 
        if (kittenDetection.characterState == Detection.STATE.CHASING)
        {
            Vector2 distanceVector = (Vector2)transform.position - startingPos;
            float distanceFromStarting = distanceVector.magnitude;
            
            if (distanceFromStarting >= maxTravelDistance)
            {
                kittenDetection.characterState = Detection.STATE.TIRED;

                StopMovement();
            }

            // if there is no target tile yet
            // find the target tile
            if (targetTilePosition == Vector2.zero)
            {
                 //Get the moving Direction
                 if (GetMovingDirection())
                {
                    targetTilePosition = GetNextTile();
                    directionVector = (targetTilePosition - (Vector2)transform.position).normalized;
                }
            }
            else
            {
                // If it is moving along the X Axis
                if (movingDir == Detection.DIRECTIONS.LEFT)
                {
                    float targetXPos = targetTilePosition.x;

                    if (transform.position.x <= targetXPos)
                    {
                        // it reached the tile
                        targetReached = true;
                    }
                }
                else if (movingDir == Detection.DIRECTIONS.RIGHT)
                {
                    float targetXPos = targetTilePosition.x;

                    if (transform.position.x >= targetXPos)
                    {
                        // it reached the tile
                        targetReached = true;
                    }
                }
                else if (movingDir == Detection.DIRECTIONS.UP)
                {
                    float targetYPos = targetTilePosition.y;

                    if (transform.position.y >= targetYPos)
                    {
                        targetReached = true;
                    }
                }
                else if (movingDir == Detection.DIRECTIONS.DOWN)
                {
                    float targetYPos = targetTilePosition.y;

                    if (transform.position.y <= targetYPos)
                    {
                        targetReached = true;
                    }
                }

                if (targetReached == true)
                {
                    // it reached the tile
                    transform.position = targetTilePosition;
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
        targetTilePosition = Vector2.zero;
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
}
