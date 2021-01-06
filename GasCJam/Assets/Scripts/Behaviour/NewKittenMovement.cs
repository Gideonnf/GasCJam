using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewKittenMovement : MonoBehaviour
{
    [Tooltip("Kitten movement Speed")]
    public float moveSpeed;
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

    // Start is called before the first frame update
    void Start()
    {
        kittenDetection = GetComponent<KittenDetection>();
        m_rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // if the cat is currently chasing 
        if (kittenDetection.characterState == Detection.STATE.CHASING)
        {
            // if there is no target tile yet
            // find the target tile
            if (targetTilePosition == Vector2.zero)
            {
                 //Get the moving Direction
            }
            else
            {
                // it moves
            }

            if (targetReached == true)
            {
                // it reached the tile
                transform.position = targetTilePosition;
                StopMovement();
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
        switch (kittenDetection.targetDir)
        {
            case Detection.DIRECTIONS.UP:
                // Check the opposite diection of the target
                // if hes coming from above, check to move downwards
                if (CheckDirection(Detection.DIRECTIONS.DOWN))
                {
                    movingDir = Detection.DIRECTIONS.DOWN;
                }
                else
                {
                    movingDir = GetAltDirection(Detection.DIRECTIONS.DOWN, kittenDetection.targetDir);
                }
                break;
            case Detection.DIRECTIONS.DOWN:
                // if target coming from below, move up
                if (CheckDirection(Detection.DIRECTIONS.UP))
                {
                    movingDir = Detection.DIRECTIONS.UP;
                }
                else
                {
                    movingDir = GetAltDirection(Detection.DIRECTIONS.UP, kittenDetection.targetDir);
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
                    movingDir = GetAltDirection(Detection.DIRECTIONS.RIGHT, kittenDetection.targetDir);
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
                    movingDir = GetAltDirection(Detection.DIRECTIONS.LEFT, kittenDetection.targetDir);
                }
                break;
            case Detection.DIRECTIONS.NONE:
                break;
            default:
                break;
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
                // if the target is on the left of the object
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
            }
            else if (currDirection == Detection.DIRECTIONS.UP || currDirection == Detection.DIRECTIONS.DOWN)
            {
                // the target is below
                if (targetTilePos.y <= 0.0f)
                {
                    tempDirection = Detection.DIRECTIONS.UP;
                }
                // the target is ontop
                else
                {
                    tempDirection = Detection.DIRECTIONS.DOWN;
                }
            }

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
