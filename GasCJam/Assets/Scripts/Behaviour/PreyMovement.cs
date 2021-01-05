using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyMovement : MonoBehaviour
{
    [Tooltip("Reference to the rigid body")]
    Rigidbody2D rigidBody;
    [Tooltip("Stores the target position that the prey has to move to")]
    Vector2 targetVector;
    [Tooltip("Boolean flag for if it is moving")]
    bool isMoving = false;
    PreyBehaviour preyBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        preyBehaviour = GetComponent<PreyBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            // Move to the target vector
            //TODO: Move to the target vector lol
            // i finna sleep soon

            transform.position = targetVector;




            //Set isMoving to false when it reaches the end
            isMoving = false;
            preyBehaviour.isRunning = false;

        }
    }

    /// <summary>
    /// Moves the prey in the target direction
    /// They should move in the opposite direction of the target object
    /// If that direction isn't available then the next best direction
    /// </summary>
    /// <param name="dir">target Direction to move towards to</param>
    public void MovePrey(Detection.DIRECTIONS dir, Detection.DIRECTIONS targetDir)
    {
        
        Detection.DIRECTIONS movingDir = dir;
        bool directionClear = false;

        // if it is already moving then the theres no point checking
        if (isMoving == true)
            return;

        // Check the diection it wants to move in
        // if the opposite direction is not clear
        // check all the others
        if (CheckDirection(dir) == false)
        {
            for (int index = 0; index < (int)Detection.DIRECTIONS.NONE; ++index)
            {
                // If it reaches the same direction as the direction given earlier
                // or if its in the same direction as the cat
                // skip it
                if (index == (int)dir)
                    continue;
                else if (index == (int)targetDir)
                    continue;

                // If that direction is blocked/not very long
                // Check it through the function again
                // Convert int to direction
                if (CheckDirection((Detection.DIRECTIONS)index))
                {
                    //Change the moving direction to the new one
                    movingDir = (Detection.DIRECTIONS)index;
                    directionClear = true;
                    break;
                }

                // If it doesnt pass any direction
                // direction clear will be set to false
                directionClear = false;
            }
        }
        else
            directionClear = true;

        // if the direction isnt clear anywhere
        // return and not move at all
        // the prey is probably stuck/bugged
        // should never reach here
        if (directionClear == false)
            return;

        // Move in the direction
        // Find the end tile position to move towards 
        targetVector = FindEndTile(movingDir);

        // Debug the target vector to see if its working
        Debug.Log("tile position" + targetVector);
        Debug.Log("world position" + transform.position);
        Debug.Log("Is Moving");
        Debug.Log(movingDir);
        //Toggle the isMoving flag to true 
        // this is to start movement in update
        isMoving = true;
    }

    /// <summary>
    /// Searches through the map manager to find the end tile in a direction
    /// If its running, it will run all the way until it hits a wall
    /// </summary>
    /// <param name="directionToCheck">The direction it wants to move in</param>
    /// <returns>Return the vector of the last tile</returns>
    public Vector2 FindEndTile (Detection.DIRECTIONS directionToCheck)
    {
        // Get the tile position of the current tile
        Vector2Int currentTilePos = MapManager.Instance.GetWorldToTilePos(transform.position);
        Vector2 TargetTilePos = Vector2.zero;

        //switch (directionToCheck)
        //{
        //    case Detection.DIRECTIONS.UP:
        //        currentTilePos.y++;
        //        break;
        //    case Detection.DIRECTIONS.DOWN:
        //        currentTilePos.y--;
        //        break;
        //    case Detection.DIRECTIONS.LEFT:
        //        currentTilePos.x--;
        //        break;
        //    case Detection.DIRECTIONS.RIGHT:
        //        currentTilePos.x++;
        //        break;
        //    default:
        //        Debug.LogError("It shouldn't reach here lol");
        //        break;
        //}

        while (MapManager.Instance.IsThereTileOnMap(currentTilePos) == false)
        {
            // store the current tile it checked as the target tile pos
            TargetTilePos = MapManager.Instance.GetTileToWorldPos(currentTilePos);

            // Move the currentTilePos
            switch (directionToCheck)
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

        return TargetTilePos;
    }

    /// <summary>
    /// Takes in the direction, passes in the position to the map manager
    /// Checks if that direction is clear and returns true if it is
    /// </summary>
    /// <param name="directionToCheck">The direction to check against the map manager</param>
    /// <returns>Return true if the path is clear</returns>
    public bool CheckDirection(Detection.DIRECTIONS directionToCheck)
    {
        // Get its current tile pos
        Vector2Int currentTilePos = MapManager.Instance.GetWorldToTilePos(transform.position);

        // idk if i should check just the first tile or more than 1
        // for now i'll just check once

        switch (directionToCheck)
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
        }

        if (MapManager.Instance.IsThereTileOnMap(currentTilePos) == false)
            return true;

        return false;
    }
}
