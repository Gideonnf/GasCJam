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
    /// Checks which direction to move before it starst to move to the next tile
    /// 
    /// </summary>
    void GetMovingDirection()
    {
        // 
        switch (mouseDetection.targetDir)
        {
            case Detection.DIRECTIONS.UP:
                // If the target is coming from above
                // he'll want to move down
                if(CheckDirection(Detection.DIRECTIONS.DOWN) == false)
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dirToCheck">Direction to Check</param>
    /// <returns>If the path is clear</returns>
    bool CheckDirection(Detection.DIRECTIONS dirToCheck)
    {


        return false;
    }

    Vector2 GetNextTile()
}
