using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KittenDetection : Detection
{
    NewKittenMovement kittenMovement;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        kittenMovement = GetComponent<NewKittenMovement>();

        // Set the position of the sprite into the center of the tile
        // incase its alittle off center
        Vector2Int currentTilePos = MapManager.Instance.GetWorldToTilePos(transform.position);
        Vector2 tilePosition2D = MapManager.Instance.GetTileToWorldPos(currentTilePos);
        // set the position
        transform.position = tilePosition2D;

        StartCoroutine("CheckForObjectsInRange");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool DetectRadius()
    {
        // Clear the objets in the list incase
        if (ObjectsInRange.Count > 0)
            ObjectsInRange.Clear();

        // call the base class detect radius to get the list of objects in range
        if (base.DetectRadius())
        {
            // Loop through the detected objects
            foreach (GameObject detectedObj in ObjectsInRange)
            {
                if (detectedObj.tag == "Prey")
                {
                    // Set the character state
                    characterState = STATE.CHASING;

                    // Set the mouse object as it's current target
                    targetObject = detectedObj;

                    // Get the target's direc tion
                    targetDir = GetTargetDirection();
                }
            }
        }

        return false;
    }

    public void StopMovement()
    {
        characterState = STATE.IDLE;
        targetObject = null;
        targetDir = DIRECTIONS.NONE;
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
}
