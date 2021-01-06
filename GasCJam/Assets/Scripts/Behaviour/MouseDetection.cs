using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDetection : Detection
{
    MouseMovement mouseMovement;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        mouseMovement = GetComponent<MouseMovement>();

        // Set the position of the sprite into the center of the tile
        // incase its alittle off center
        Vector2Int currentTilePos = MapManager.Instance.GetWorldToTilePos(transform.position);
        Vector2 tilePosition2D = MapManager.Instance.GetTileToWorldPos(currentTilePos);
        // set the position
        transform.position = tilePosition2D;

        StartCoroutine("CheckForObjectsInRange");
        //StartCoroutine("CheckForObjectsInView");

    }

    // Update is called once per frame
    void Update()
    {
        //DetectRadius();
    }

    public override bool DetectRadius()
    {
        // Clear the objects in range
        if (ObjectsInRange.Count > 0)
            ObjectsInRange.Clear();

        // If it detects any objects in it's radius
        if (base.DetectRadius())
        {
            // Loop through the detected objects
            foreach(GameObject detectedObj in ObjectsInRange)
            {
                // If it spots the player or kitten
                // it will try to run away
                if (detectedObj.tag == "Player" || detectedObj.tag == "Kitten")
                {
                    // running
                    characterState = STATE.RUNNING;

                    // Set the player object as it's current target
                    targetObject = detectedObj;

                    targetDir = GetTargetDirection();
                }
            }
        }
        //else
        //{
        //    characterState = STATE.IDLE;
        //    targetObject = null;
        //    targetDir = DIRECTIONS.NONE;
        //}


        return false;
    }

    /// <summary>
    ///  For checking nearby enemies
    /// </summary>
    /// <returns>returns true if there are enemies nearby</returns>
    public bool CheckForEnemies()
    {
        // theres enemies in the area
        if (base.DetectRadius() == true)
        {
            return true;
        }

        return false ;
    }

    public void StopMovement()
    {
        characterState = STATE.IDLE;
        targetObject = null;
        targetDir = DIRECTIONS.NONE;
    }

    public override CHARACTERS CheckForCharacters(Vector2Int tilePosition)
    {
        // Get the player position in tiles
        Vector2Int playerTilePos = MapManager.Instance.GetWorldToTilePos(playerObject.transform.position);

        // if the player is within that tile
        if (playerTilePos == tilePosition)
            return CHARACTERS.PLAYER;

        return CHARACTERS.NONE;
    }

    // A couroutine to run for checking of objects
    // Instead of checking every frame it checks every second
    IEnumerator CheckForObjectsInRange()
    {
        for (; ; )
        {
            // If it successfully detected something in it's radius
            // Check for what direction it is in
            DetectRadius();


            yield return new WaitForSeconds(.2f);
        }
    }

    IEnumerator CheckForObjectsInView()
    {
        for (; ;)
        {
            if (DetectInView() == CHARACTERS.PLAYER)
            {
                // running
                characterState = STATE.RUNNING;

                // Set the player object as it's current target
                targetObject = playerObject;

                targetDir = GetTargetDirection();

            }

            yield return new WaitForSeconds(.2f);
        }
    }

}
