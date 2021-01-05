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
            rigidBody.MovePosition(transform.position + direction * moveSpeed * Time.fixedDeltaTime);

            float targetXPos = targetVector.x;
            float targetYPos = targetVector.y;

            // if its moving to the left
            if (movingDir == Detection.DIRECTIONS.LEFT)
            {
                // when it reaches less than or equal to the target  xposition
                // left is negative for x axis
                if (transform.position.x <= targetXPos)
                {
                    transform.position = targetVector;
                    kittenBehaviour.isRunning = false;
                    isMoving = false;
                    movingDir = Detection.DIRECTIONS.NONE;
                }
            }
            else if (movingDir == Detection.DIRECTIONS.RIGHT)
            {
                // when it reaches more than or equal to the target x position
                // right is positive for x axis
                if (transform.position.x >= targetXPos)
                {
                    transform.position = targetVector;
                    kittenBehaviour.isRunning = false;
                    isMoving = false;
                    movingDir = Detection.DIRECTIONS.NONE;
                }
            }
            else if (movingDir == Detection.DIRECTIONS.UP)
            {
                // when it reaches more than or equal to the target y position
                // moving up is positive for y axis
                if (transform.position.y >= targetYPos)
                {
                    transform.position = targetVector;
                    kittenBehaviour.isRunning = false;
                    isMoving = false;
                    movingDir = Detection.DIRECTIONS.NONE;
                }
            }
            else if (movingDir == Detection.DIRECTIONS.DOWN)
            {
                // when it reaches less than or equal to the target y position
                // moving down is negative for y axis
                if (transform.position.y <= targetYPos)
                {
                    transform.position = targetVector;
                    kittenBehaviour.isRunning = false;
                    isMoving = false;
                    movingDir = Detection.DIRECTIONS.NONE;
                }
            }

            //if (movingDir == Detection.DIRECTIONS.LEFT || movingDir == Detection.DIRECTIONS.RIGHT)
            //{

            //    // if it is negative
            //    if (targetXPos <= 0.0f)
            //    {
            //        if (transform.position.x <= targetXPos)
            //        {
            //            transform.position = targetVector;
            //            kittenBehaviour.isRunning = false;
            //            isMoving = false;
            //            movingDir = Detection.DIRECTIONS.NONE;
            //        }
            //    }
            //    // else if it is positive number
            //    else if (targetXPos >= 0.0f)
            //    {
            //        if (transform.position.x >= targetXPos)
            //        {
            //            transform.position = targetVector;
            //            kittenBehaviour.isRunning = false;
            //            isMoving = false;
            //            movingDir = Detection.DIRECTIONS.NONE;
            //        }
            //    }

            //}
            //else if (movingDir == Detection.DIRECTIONS.UP || movingDir == Detection.DIRECTIONS.DOWN)
            //{

            //    // if it is a negative number
            //    if (targetYPos <= 0.0f)
            //    {
            //        if (transform.position.y <= targetYPos)
            //        {
            //            transform.position = targetVector;
            //            kittenBehaviour.isRunning = false;
            //            isMoving = false;
            //            movingDir = Detection.DIRECTIONS.NONE;
            //        }
            //    }
            //    // if it is a positive number
            //    else if (targetYPos >= 0.0f)
            //    {
            //        if (transform.position.y >= targetYPos)
            //        {
            //            transform.position = targetVector;
            //            kittenBehaviour.isRunning = false;
            //            isMoving = false;
            //            movingDir = Detection.DIRECTIONS.NONE;
            //        }
            //    }
            //}

        }
    }

    public void SetKittenDirection(Detection.DIRECTIONS targetDir)
    {
        // if it is stuck
        // it has to find another way to move to the rat
        // (i.e another direction)
        if (isStuck == true)
        {

        }
        else
        {
            //set the moving direction to where it should go
            // it should always be moving in the direction of the cat
            movingDir = targetDir;

            targetVector = FindEndTile(movingDir);

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
