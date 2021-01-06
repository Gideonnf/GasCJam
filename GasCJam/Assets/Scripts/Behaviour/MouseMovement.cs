using System.Collections;
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
            // if it hasn't reached the target spot yet
            if (targetReached == false)
            {
                // if no target tile position was assigned yet
                // set the target tile
                if (targetTilePosition == Vector2.zero)
                {

                }

            }
        }
    }

    /// <summary>
    /// Checks all the direction for the longest path
    /// </summary>
    /// <returns></returns>
    Detection.DIRECTIONS CheckDirection()
    {
        int currentTileCounter = 0;

        Detection.DIRECTIONS tempDir = Detection.DIRECTIONS.NONE;

        // Check the direction it needs to move in first
        for (int index = 0; index < (int)Detection.DIRECTIONS.NONE; ++index)
        {
            if (index == (int)mouseDetection.targetDir)
                continue;

            Vector2Int currentTilePos = MapManager.Instance.GetWorldToTilePos(transform.position);

            // while the path is clear
            // keep checking to find out how many
            // will take the direction with the longest clear path
            while(MapManager.Instance.IsThereTileOnMap(currentTilePos) == false)
            {
                currentTileCounter++;

                switch ((Detection.DIRECTIONS)index)
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

            // if the current direction has more space than the previous one
            if (currentTileCounter > currentDirTileCounter)
            {
                // set them
                tempDir = (Detection.DIRECTIONS)index;

                currentDirTileCounter = currentTileCounter;

                // reset
                currentTileCounter = 0;
            }
        }

        return tempDir;
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
    /// Checks which direction to move before it starst to move to the next tile
    /// 
    /// </summary>
    void GetMovingDirection()
    {
        movingDir = CheckDirection();

        // 
        switch (mouseDetection.targetDir)
        {
            case Detection.DIRECTIONS.UP:
                // If the target is coming from above
                // he'll want to move down
                if (CheckDirection(Detection.DIRECTIONS.DOWN))
                {

                }
                break;
            case Detection.DIRECTIONS.DOWN:
                break;
            case Detection.DIRECTIONS.LEFT:
                break;
            case Detection.DIRECTIONS.RIGHT:
                break;
            case Detection.DIRECTIONS.NONE:
                break;
            default:
                break;
        }
    }

    Vector2 GetNextTile()
    {

        return Vector2.zero;
    }
}
