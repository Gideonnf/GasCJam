using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyMovement : MonoBehaviour
{
    [Tooltip("Prey Movement Speed")]
    public float moveSpeed;
    [Tooltip("Reference to the rigid body")]
    Rigidbody2D rigidBody;
    [Tooltip("Stores the target position that the prey has to move to")]
    Vector2 targetVector;
    [Tooltip("Boolean flag for if it is moving")]
    public bool isMoving = false;
    [Tooltip("Stores the direction it is moving in")]
    public Detection.DIRECTIONS movingDir;
    [Tooltip("Number of empty spaces in moving dir")]
    int currentDirTileCount = 0;

    [Header("Visual")]
    public Animator m_Animator;

    PreyBehaviour preyBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        preyBehaviour = GetComponent<PreyBehaviour>();

        UpdateAnimation();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            // Move to the target vector
            Vector3 direction = (targetVector - (Vector2)transform.position).normalized;
            rigidBody.MovePosition(transform.position + direction * moveSpeed * Time.fixedDeltaTime);

            if (movingDir == Detection.DIRECTIONS.LEFT || movingDir == Detection.DIRECTIONS.RIGHT)
            {
                float targetXPos = targetVector.x;

                // if its negative
                if (targetXPos <= 0.0f)
                {
                    if (transform.position.x <= targetXPos)
                    {
                        transform.position = targetVector;
                        preyBehaviour.isRunning = false;
                        ResetMovement();
                    }
                }
                // if its positive
                else if (targetXPos >= 0.0f)
                {
                    if (transform.position.x >= targetXPos)
                    {
                        transform.position = targetVector;
                        preyBehaviour.isRunning = false;
                        ResetMovement();
                    }
                }
             

            }
            else if (movingDir == Detection.DIRECTIONS.UP || movingDir == Detection.DIRECTIONS.DOWN)
            {
                float targetYPos = targetVector.y;

                // if its negative
                if (targetYPos <= 0.0f)
                {
                    if (transform.position.y <= targetYPos)
                    {
                        transform.position = targetVector;
                        ResetMovement();
                        preyBehaviour.isRunning = false;
                    }
                }
                // if its positive
                else if (targetYPos >= 0.0f)
                {
                    if (transform.position.y >= targetYPos)
                    {
                        transform.position = targetVector;
                        preyBehaviour.isRunning = false;
                        ResetMovement();
                    }
                }
            }
        }
    }

    public void ResetMovement()
    {
        isMoving = false;
        //movingDir = Detection.DIRECTIONS.NONE;
        currentDirTileCount = 0;

        UpdateAnimation();
    }

    /// <summary>
    /// Moves the prey in the target direction
    /// They should move in the opposite direction of the target object
    /// If that direction isn't available then the next best direction
    /// </summary>
    /// <param name="dir">target Direction to move towards to</param>
    public void MovePrey(Detection.DIRECTIONS dir, Detection.DIRECTIONS targetDir)
    {
        // if it is already moving then the theres no point checking
        if (isMoving == true)
            return;

        movingDir = dir;
        UpdateAnimation();
        bool directionClear = false;

        // Check the first direction to run towards
        CheckDirection(dir);

        Vector2 targetDirVector = preyBehaviour.GetTargetDirVector();

        // Check alternative paths
        // if the target is coming from the left or right
        if (targetDir == Detection.DIRECTIONS.LEFT || targetDir == Detection.DIRECTIONS.RIGHT)
        {
            // check whether the object is coming at a diagonal
            // example, to prevent it from running downwards if the target is below as well

            // if it is negative y vector
            // if the target is below the prey
            if (targetDirVector.y <= 0.0f)
            {
                CheckDirection(Detection.DIRECTIONS.UP);
            }
            // if the target is above the prey
            else if (targetDirVector.y >= 0.0f)
            {
                CheckDirection(Detection.DIRECTIONS.DOWN);
            }
        }
        else if (targetDir == Detection.DIRECTIONS.UP || targetDir == Detection.DIRECTIONS.DOWN)
        {
            // check if the object is coming at a diagonal
            // example, if its coming from the top left, the rat shouldn't run left

            // if it is coming from the left
            if (targetDirVector.x <= 0.0f)
            {
                CheckDirection(Detection.DIRECTIONS.RIGHT);
            }
            // if it is coming from the right
            else if (targetDirVector.x >= 0.0f)
            {
                CheckDirection(Detection.DIRECTIONS.LEFT);
            }
        }


        //for (int index = 0; index < (int)Detection.DIRECTIONS.NONE; ++index)
        //{
        //    if (index == (int)targetDir)
        //        continue;

        //    // check through the 4 directions
        //    CheckDirection((Detection.DIRECTIONS)index);
        //}

        // After finding the moving direction
        // we need to set the view direction
        // we want the rat to look in the opposite direction it ran from

        switch (movingDir)
        {
            case Detection.DIRECTIONS.UP:
                preyBehaviour.viewDir = Detection.DIRECTIONS.DOWN;
                break;
            case Detection.DIRECTIONS.DOWN:
                preyBehaviour.viewDir = Detection.DIRECTIONS.UP;
                break;
            case Detection.DIRECTIONS.LEFT:
                preyBehaviour.viewDir = Detection.DIRECTIONS.RIGHT;
                break;
            case Detection.DIRECTIONS.RIGHT:
                preyBehaviour.viewDir = Detection.DIRECTIONS.LEFT;
                break;
            case Detection.DIRECTIONS.NONE:
                Debug.Log("haha pee pee poo poo");
                break;
            default:
                break;
        }

        //// Check the diection it wants to move in
        //// if the opposite direction is not clear
        //// check all the others
        //if (CheckDirection(dir) == false)
        //{
        //    for (int index = 0; index < (int)Detection.DIRECTIONS.NONE; ++index)
        //    {
        //        // If it reaches the same direction as the direction given earlier
        //        // or if its in the same direction as the cat
        //        // skip it
        //        if (index == (int)dir)
        //            continue;
        //        else if (index == (int)targetDir)
        //            continue;

        //        // If that direction is blocked/not very long
        //        // Check it through the function again
        //        // Convert int to direction
        //        if (CheckDirection((Detection.DIRECTIONS)index))
        //        {
        //            //Change the moving direction to the new one
        //            movingDir = (Detection.DIRECTIONS)index;
        //            directionClear = true;
        //            break;
        //        }

        //        // If it doesnt pass any direction
        //        // direction clear will be set to false
        //        directionClear = false;
        //    }
        //}
        //else
        //    // if the first direction it checked is already clear
        //    directionClear = true;

        //// if the direction isnt clear anywhere
        //// return and not move at all
        //// the prey is probably stuck/bugged
        //// should never reach here
        //if (directionClear == false)
        //    return;

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
        UpdateAnimation();
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
                    Debug.Log("It shouldn't reach here lol");
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
        // EDIT: once does not work. I repeat, once does not work aifioasiofnasionfoashfhio

        int currentTileCounter = 0;

        while (MapManager.Instance.IsThereTileOnMap(currentTilePos) == false)
        {
            currentTileCounter++;

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

        // if the current tile counter is more than the number of empty tiles in the current direction
        // it means that it found a better direction
        if (currentTileCounter > currentDirTileCount)
        {
            // Set the new moving direction
            movingDir = directionToCheck;

            currentDirTileCount = currentTileCounter;

            return true;
        }
        else
        {
            return false;
        }

        //if (MapManager.Instance.IsThereTileOnMap(currentTilePos) == false)
        //    return true;

    }

    public void UpdateAnimation()
    {
        Vector2 dir = Vector2.zero;

        switch (movingDir)
        {
            case Detection.DIRECTIONS.UP:
                dir.y = 1.0f;
                break;
            case Detection.DIRECTIONS.DOWN:
                dir.y = -1.0f;
                break;
            case Detection.DIRECTIONS.LEFT:
                dir.x = -1.0f;
                break;
            case Detection.DIRECTIONS.RIGHT:
                dir.x = 1.0f;
                break;
        }

        m_Animator.SetBool("Moving", isMoving);
        m_Animator.SetFloat("Horizontal", dir.x);
        m_Animator.SetFloat("Vertical", dir.y);
    }
}
