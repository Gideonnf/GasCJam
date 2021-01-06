using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KittenMovement : MonoBehaviour
{
    [Tooltip("Kitten movement speed")]
    public float moveSpeed;
    [Tooltip("Reference to the rigid body")]
    Rigidbody2D rigidBody;
    [Tooltip("Store the vector position to move to")]
    Vector2 targetVector;
    [Tooltip("Boolean flag for if it is currenly moving")]
    public bool isMoving = true;
    public bool isStuck = false;
    [Tooltip("Stores the direction that it is moving in")]
    public Detection.DIRECTIONS movingDir;
    [Tooltip("Temporary moving direction when stuck")]
    public Detection.DIRECTIONS tempMovingDir;

    bool targetReached = false;

    // Store reference to the behaviour script
    KittenBehaviour kittenBehaviour;

    bool juststopplease = false;


    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        kittenBehaviour = GetComponent<KittenBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //TODO: Stop it from moving small diagonals
        // i could just make it so if it moves along x or y axis
        // i set the other axis to 0
        // i.e if its moving left or right, targetvector Y will be 0 to stop it from moving up or down
        // or if its moving up and down, target vector X will be 0 to stop it from moving side ways while going up or down

        if (juststopplease)
            return;

        if (isMoving)
        {
            Vector3 direction = (targetVector - (Vector2)transform.position).normalized;

            float targetXPos = targetVector.x;
            float targetYPos = targetVector.y;

            if (isStuck)
            {
                // if its moving to the left
                if (tempMovingDir == Detection.DIRECTIONS.LEFT)
                {
                    // Set the y axis to 0
                    // it doesn't need to move up and down
                    direction.y = 0;
                    // when it reaches less than or equal to the target  xposition
                    // left is negative for x axis
                    if (transform.position.x <= targetXPos)
                    {
                        transform.position = targetVector;
                        kittenBehaviour.isRunning = false;
                        isMoving = false;
                        tempMovingDir = Detection.DIRECTIONS.NONE;
                        targetReached = true;

                        if (targetReached == false)
                        {
                            targetReached = true;
                            return;
                        }
                    }
                }
                else if (tempMovingDir == Detection.DIRECTIONS.RIGHT)
                {
                    // Set the y axis to 0
                    // it doesn't need to move up and down
                    direction.y = 0;
                    // when it reaches more than or equal to the target x position
                    // right is positive for x axis
                    if (transform.position.x >= targetXPos)
                    {
                        transform.position = targetVector;
                        kittenBehaviour.isRunning = false;
                        isMoving = false;
                        tempMovingDir = Detection.DIRECTIONS.NONE;
                        targetReached = true;

                        if (targetReached == false)
                        {
                            targetReached = true;
                            return;
                        }
                    }
                }
                else if (tempMovingDir == Detection.DIRECTIONS.UP)
                {
                    // Set the x axis to 0
                    // it doesn't need to move left and right
                    direction.x = 0;

                    // when it reaches more than or equal to the target y position
                    // moving up is positive for y axis
                    if (transform.position.y >= targetYPos)
                    {
                        transform.position = targetVector;
                        kittenBehaviour.isRunning = false;
                        isMoving = false;
                        tempMovingDir = Detection.DIRECTIONS.NONE;
                        targetReached = true;

                        if (targetReached == false)
                        {
                            targetReached = true;
                            return;
                        }
                    }
                }
                else if (tempMovingDir == Detection.DIRECTIONS.DOWN)
                {
                    // Set the x axis to 0
                    // it doesn't need to move left and right
                    direction.x = 0;

                    // when it reaches less than or equal to the target y position
                    // moving down is negative for y axis
                    if (transform.position.y <= targetYPos)
                    {
                        transform.position = targetVector;
                        kittenBehaviour.isRunning = false;
                        isMoving = false;
                        tempMovingDir = Detection.DIRECTIONS.NONE;

                        if (targetReached == false)
                        {
                            targetReached = true;
                            return;
                        }
                    }
                }
            }
            // if it isnt stuck
            // continue with the normal movement
            else
            {
                // if its moving to the left
                if (movingDir == Detection.DIRECTIONS.LEFT)
                {
                    // Set the y axis to 0
                    // it doesn't need to move up and down
                    direction.y = 0;
                    // when it reaches less than or equal to the target  xposition
                    // left is negative for x axis
                    if (transform.position.x <= targetXPos)
                    {
                        transform.position = targetVector;
                        kittenBehaviour.isRunning = false;
                        ResetMovement();
                        targetReached = true;

                        if (targetReached == false)
                        {
                            targetReached = true;
                            return;
                        }
                    }
                }
                else if (movingDir == Detection.DIRECTIONS.RIGHT)
                {
                    // Set the y axis to 0
                    // it doesn't need to move up and down
                    direction.y = 0;
                    // when it reaches more than or equal to the target x position
                    // right is positive for x axis
                    if (transform.position.x >= targetXPos)
                    {
                        transform.position = targetVector;
                        kittenBehaviour.isRunning = false;
                        ResetMovement();
                        targetReached = true;

                        if (targetReached == false)
                        {
                            targetReached = true;
                            return;
                        }
                    }
                }
                else if (movingDir == Detection.DIRECTIONS.UP)
                {
                    // Set the x axis to 0
                    // it doesn't need to move left and right
                    direction.x = 0;

                    // when it reaches more than or equal to the target y position
                    // moving up is positive for y axis
                    if (transform.position.y >= targetYPos)
                    {
                        transform.position = targetVector;
                        kittenBehaviour.isRunning = false;
                        ResetMovement();
                        targetReached = true;

                        if (targetReached == false)
                        {
                            targetReached = true;
                            return;
                        }
                    }
                }
                else if (movingDir == Detection.DIRECTIONS.DOWN)
                {
                    // Set the x axis to 0
                    // it doesn't need to move left and right
                    direction.x = 0;

                    // when it reaches less than or equal to the target y position
                    // moving down is negative for y axis
                    if (transform.position.y <= targetYPos)
                    {
                        transform.position = targetVector;
                        kittenBehaviour.isRunning = false;
                        ResetMovement();
                        if (targetReached == false)
                        {
                            targetReached = true;
                            return;
                        }
                    }
                }
            }


            // Return when they reach the target to prevent any extra movements

            //update it's position
            rigidBody.MovePosition(transform.position + direction * moveSpeed * Time.fixedDeltaTime);

            //Debug.Log("Target reached " + targetReached);

            // If the target already reached
            // and it looped back through then that means that it hasnt move
            // and that should mean that its stuck

            if (targetReached)
            {
                isStuck = true;
            }

        }
    }

    public void SetKittenDirection(Detection.DIRECTIONS targetDir)
    {
        // if it is stuck
        // it has to find another way to move to the rat
        // (i.e another direction)
        if (isStuck == true)
        {

            if (kittenBehaviour.isTired)
            {
                ResetMovement();
                return;
            }

            Vector2 Dir = kittenBehaviour.GetTargetDirVector();

            // get the original moving direction
            movingDir = kittenBehaviour.GetTargetDirection();

            // if it is moving up or down and it got stuck
            // it'll check left and right
            if (movingDir == Detection.DIRECTIONS.UP || movingDir == Detection.DIRECTIONS.DOWN)
            {
                // if its positive then move right
                if (Dir.x >= 0.0f)
                {
                    tempMovingDir = Detection.DIRECTIONS.RIGHT;
                }
                // if its negative then move left
                else if (Dir.x <= 0.0f)
                {
                    tempMovingDir = Detection.DIRECTIONS.LEFT;
                }
            }
            // if its moving left and right
            // then it'll check up and down
            else if (movingDir == Detection.DIRECTIONS.LEFT || movingDir == Detection.DIRECTIONS.RIGHT)
            {
                if (Dir.y >= 0.0f)
                {
                    tempMovingDir = Detection.DIRECTIONS.UP;
                }
                else if (Dir.y <= 0.0f)
                {
                    tempMovingDir = Detection.DIRECTIONS.DOWN;
                }
            }

            if (targetVector != FindEndTile(tempMovingDir))
            {
                targetVector = FindEndTile(tempMovingDir);
                isMoving = true;
                targetReached = false;
            }

            // Check the original moving direction if the path is clear
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
                    Debug.LogError("pee pee poo poo");
                    break;
            }

            movingDir = Detection.DIRECTIONS.NONE;

            // if the path infront is clear
            if (MapManager.Instance.IsThereTileOnMap(currentTilePos) == false)
            {
                currentTilePos = MapManager.Instance.GetWorldToTilePos(transform.position);
                Vector2 tilePosition = MapManager.Instance.GetTileToWorldPos(currentTilePos);
                transform.position = tilePosition;
                

                tempMovingDir = Detection.DIRECTIONS.NONE;
                isStuck = false;
            }
        }
        else
        {
            //set the moving direction to where it should go
            // it should always be moving in the direction of the cat
            movingDir = targetDir;

            // if the target vector changes

            if (targetVector != FindEndTile(movingDir))
            {
                targetVector = FindEndTile(movingDir);

                targetReached = false;
            }


            isMoving = true;

        }
        // if its already moving
        //if (isMoving == true)
        //{
        //    return;
        //}


        // How should the kitten move after the rat
        // it'll chase in the direction that the cat is in
        // it shouldn't need to check which direction is clear like the rat

        // it will keep moving until it hits a wall
        // when it hits the wall, and it didn't hit the rat
        // it will try to find how to get to the rat
        // it will have to move another axis

    }

    /// <summary>
    /// Resets the movement variables of ktiten movement
    /// </summary>
    public void ResetMovement()
    {
        isMoving = false;
        movingDir = Detection.DIRECTIONS.NONE;
    }

    /// <summary>
    /// Uses the Map manager to find the end tile in the direction it needs to move in
    /// It'll make it so the cat will run all the way to the end
    /// </summary>
    /// <param name="directionToCheck">Direction to check</param>
    /// <returns> Returns the vector 2 position </returns>
    public Vector2 FindEndTile (Detection.DIRECTIONS directionToCheck)
    {
        // Get the current tile position in the map
        Vector2Int currentTilePos = MapManager.Instance.GetWorldToTilePos(transform.position);
        //Create a variable to store the vector
        Vector2 targetTilePos = Vector2.zero;

        while(MapManager.Instance.IsThereTileOnMap(currentTilePos) == false)
        {
            // store the current tile it checked as the target tile pos
            targetTilePos = MapManager.Instance.GetTileToWorldPos(currentTilePos);

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
                case Detection.DIRECTIONS.NONE:
                    break;
                default:
                    Debug.Log("It shouldn't reach here omo, u fuked up ASNfgioahOI");
                    break;
            }
        }

       
        return targetTilePos;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if it collides with the rat
        if (collision.gameObject.tag == "Prey")
        {
            juststopplease = true;
            isMoving = false;
            // end the round?
            // victory
            Debug.Log("Touched the mouse already lmao");
            return;
        }
    }
}
