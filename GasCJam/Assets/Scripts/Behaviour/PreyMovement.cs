using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyMovement : MonoBehaviour
{
    Rigidbody2D rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Moves the prey in the target direction
    /// They should move in the opposite direction of the target object
    /// If that direction isn't available then the next best direction
    /// </summary>
    /// <param name="dir">target Direction to move towards to</param>
    public void MovePrey(Detection.DIRECTIONS dir, Detection.DIRECTIONS targetDir)
    {
        Vector2 movingDir;
        switch (dir)
        {
            case Detection.DIRECTIONS.UP:
                movingDir = new Vector2(0, 1.0f);
                break;
            case Detection.DIRECTIONS.DOWN:
                movingDir = new Vector2(0, -1.0f);
                break;
            case Detection.DIRECTIONS.LEFT:
                movingDir = new Vector2(-1.0f, 0);
                break;
            case Detection.DIRECTIONS.RIGHT:
                movingDir = new Vector2(1.0f, 0);
                break;
        }

        // Check the diection it wants to move in

        // If that direction is blocked/not very long

        // Move in another direction

    }

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

        return false;
    }
}
