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
}
