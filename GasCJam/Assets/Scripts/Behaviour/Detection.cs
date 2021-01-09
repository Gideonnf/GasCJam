using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detection : MonoBehaviour
{
    public enum DIRECTIONS
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        NONE
    }

    public enum CHARACTERS
    {
        PLAYER,
        KITTEN,
        MOUSE,
        NONE
    }

    public enum STATE
    {
        IDLE,
        RUNNING,
        CHASING,
        TIRED,
        NONE
    }

    [Header("Detection settings")]
    [Tooltip("Detection Radius")]
    public float CircleRadius = 1;
    [Tooltip("Line of Sight Distance?")]
    public float SightDistance;
    [Tooltip("Stores the direction that the character is facing")]
    public DIRECTIONS targetDir = DIRECTIONS.NONE;
    [Tooltip("The character's view direction")]
    public DIRECTIONS viewDir = DIRECTIONS.NONE;
    [Tooltip("Starting direction")]
    public DIRECTIONS startingDir;
    [Tooltip("Current state of the object")]
    public STATE characterState = STATE.NONE;

    // Stores the target object
    [System.NonSerialized] public GameObject targetObject = null;

    [Header("References to objects")]
    // idk if we should make a data manager to store the player, mouses and kitten
    // so for now i'll just use this to store a reference to the objects
    [Tooltip("Reference to player object")]
    public GameObject playerObject = null;
    [Tooltip("Reference to mouse object")]
    public GameObject ratObject = null;
    [Tooltip("Reference to the kitten object")]
    public GameObject kittenObject = null;


    protected List<GameObject> ObjectsInRange;


    public virtual void Start()
    {
        ObjectsInRange = new List<GameObject>();
        // get the player object at start
        playerObject = GameObject.FindGameObjectWithTag("Player");
        ratObject = GameObject.FindGameObjectWithTag("Prey");
        kittenObject = GameObject.FindGameObjectWithTag("Kitten");

        viewDir = startingDir;
    }

    /// <summary>
    /// Detect Colliders around the object
    /// </summary>
    /// <returns>true if there is objects found</returns>
    public virtual bool DetectRadius()
    {
        // Debug.Log("parent function does a thing");

        // Get a list of all the colliders within the area
        Collider2D[] ListOfColliders = Physics2D.OverlapCircleAll(transform.position, CircleRadius);
        
        // Scan through the list of collided objects to find the game objects
        foreach (Collider2D collider in ListOfColliders)
        {
            // dont add itself
            if (collider.gameObject == this.gameObject)
                continue;

            // Add it into the list if it isnt the environment
            // the only objects that would end up here are player, kitten, rats
            if (collider.gameObject.tag == "Player" || collider.gameObject.tag == "Kitten" || collider.gameObject.tag == "Prey")
            {
                ObjectsInRange.Add(collider.gameObject);
            }
        }

        // if they have found objects in range
        // then return true
        if (ObjectsInRange.Count > 0)
            return true;

        return false;
    }

    public virtual bool CheckForCharacters(CHARACTERS characterToCheck, Vector2Int tilePosition)
    {

        return false;
    }

    public virtual bool DetectInView(CHARACTERS characterToCheck)
    {
        Vector2Int currentTilePos = MapManager.Instance.GetWorldToTilePos(transform.position);
        bool characterFound = false;

        // Increment it first because you shouldn't check ur own position
        // thats not a good idea lmao
        switch (viewDir)
        {
            case DIRECTIONS.UP:
                currentTilePos.y++;
                break;
            case DIRECTIONS.DOWN:
                currentTilePos.y--;
                break;
            case DIRECTIONS.LEFT:
                currentTilePos.x--;
                break;
            case DIRECTIONS.RIGHT:
                currentTilePos.x++;
                break;
            default:
                Debug.Log("It shouldn't reach here lol if it does then fuk we screwed");
                break;
        }

        for (int index = 0; index < SightDistance; ++index)
        {
            // Checks the tile if any of the objects are here
            if (MapManager.Instance.IsThereTileOnMap(currentTilePos))
                 return false;

            // use a function to check
            characterFound = CheckForCharacters(characterToCheck, currentTilePos);

            // If no character was found
            // go next
            // if not then return that character
            //if (characterFound != CHARACTERS.NONE)
            //    return characterFound;
            if (characterFound == true)
                return true;

            // move on to the next tile position
            switch (viewDir)
            {
                case DIRECTIONS.UP:
                    currentTilePos.y++;
                    break;
                case DIRECTIONS.DOWN:
                    currentTilePos.y--;
                    break;
                case DIRECTIONS.LEFT:
                    currentTilePos.x--;
                    break;
                case DIRECTIONS.RIGHT:
                    currentTilePos.x++;
                    break;
                default:
                    Debug.Log("It shouldn't reach here lol if it does then fuk we screwed");
                    break;
            }
        }


        return false;
    }

    /// <summary>
    /// Checks the area infront of the character
    /// Returns the characters it finds
    /// </summary>
    /// <returns></returns>
    public virtual CHARACTERS DetectInView()
    {
        Vector2Int currentTilePos = MapManager.Instance.GetWorldToTilePos(transform.position);
        //Vector2Int playerTilePos = MapManager.Instance.GetWorldToTilePos(playerObject.transform.position);
        //Vector2Int mouseTilePos = MapManager.Instance.GetWorldToTilePos(ratObject.transform.position);

        CHARACTERS characterFound = CHARACTERS.NONE;

        // Increment it first because you shouldn't check ur own position
        // thats not a good idea lmao
        switch (viewDir)
        {
            case DIRECTIONS.UP:
                currentTilePos.y++;
                break;
            case DIRECTIONS.DOWN:
                currentTilePos.y--;
                break;
            case DIRECTIONS.LEFT:
                currentTilePos.x--;
                break;
            case DIRECTIONS.RIGHT:
                currentTilePos.x++;
                break;
            default:
                Debug.Log("It shouldn't reach here lol if it does then fuk we screwed");
                break;
        }

        // Loop through a fixed amount of times
        for (int index = 0; index < SightDistance; ++index)
        {
            // Checks the tile if any of the objects are here
            if (MapManager.Instance.IsThereTileOnMap(currentTilePos))
                return CHARACTERS.NONE;

            // use a function to check
            characterFound = CheckForCharacters(currentTilePos);

            // If no character was found
            // go next
            // if not then return that character
            if (characterFound != CHARACTERS.NONE)
                return characterFound;

            // move on to the next tile position
            switch (viewDir)
            {
                case DIRECTIONS.UP:
                    currentTilePos.y++;
                    break;
                case DIRECTIONS.DOWN:
                    currentTilePos.y--;
                    break;
                case DIRECTIONS.LEFT:
                    currentTilePos.x--;
                    break;
                case DIRECTIONS.RIGHT:
                    currentTilePos.x++;
                    break;
                default:
                    Debug.Log("It shouldn't reach here lol if it does then fuk we screwed");
                    break;
            }
        }

        // It didnt find anything here
        return CHARACTERS.NONE;
    }

    /// <summary>
    /// Made this function so that each object can check for hte character it needs
    /// it can be overrided
    /// </summary>
    /// <param name="tilePosition">The tile that it is checking</param>
    /// <returns> returns the character it found</returns>
    public virtual CHARACTERS CheckForCharacters(Vector2Int tilePosition)
    {


        return CHARACTERS.NONE;
    }

    /// <summary>
    /// because i didnt want to keep writing down these three lines everytime
    /// </summary>
    /// <returns>returns the direction vector of the target object</returns>
    public virtual Vector2 GetTargetDirVector()
    {
        // Get the two positions we need
        Vector2 TargetPos = targetObject.transform.position;
        Vector2 CurrentPos = transform.position;

        // Get the direction of the target
        Vector2 Dir = (TargetPos - CurrentPos).normalized;

        return Dir;
    }


    public virtual DIRECTIONS GetTargetDirection(Vector2 targetPos)
    {
        Vector2Int currentTilePos = MapManager.Instance.GetWorldToTilePos(transform.position);
        Vector2Int targetTilePos = MapManager.Instance.GetWorldToTilePos(targetPos);
        // Get the direction of the target

        Vector2Int DirInt = (targetTilePos - currentTilePos);

        Vector2 Dir = new Vector2(DirInt.x, DirInt.y).normalized;
        //Debug.Log(targetDir);
        // Debug.Log(Dir);

        // Checking for Up and Down first
        // if we're checking for up and down 
        // the X dir will be within a small buffer
        // 0.3f is just a number i used as a buffer
        // anything more than that means its on a diagonal
        // and player cant move in diagonal
        if (Dir.x <= 0.3f || Dir.x >= -0.3f)
        {
            // 0.5f is a buffer i used
            if (Dir.y > 0.5f)
            {
                return DIRECTIONS.UP;
            }
            else if (Dir.y < -0.5f)
            {
                return DIRECTIONS.DOWN;
            }
        }
        if (Dir.y <= 0.3f || Dir.y >= -0.3f)
        {
            if (Dir.x > 0.5f)
            {
                return DIRECTIONS.RIGHT;
            }
            else if (Dir.x < -0.5f)
            {
                return DIRECTIONS.LEFT;
            }
        }

        return DIRECTIONS.NONE;
    }

    /// <summary>
    /// Call this to check if they detected a target through the wall
    /// If the direction to the target is not empty
    /// It means it was through the wall
    /// </summary>
    /// <param name="dirToCheck">The direction to check</param>
    /// <returns> returns true if its clear </returns>
    public bool CheckIfClear(DIRECTIONS dirToCheck, Vector2 targetPosition)
    {
        Vector2Int currentTilePos = MapManager.Instance.GetWorldToTilePos(transform.position);
        Vector2Int targetTilePos = MapManager.Instance.GetWorldToTilePos(targetPosition);

       // Debug.Log("Distance between : " + Vector2Int.Distance(targetTilePos, currentTilePos));

         if (Vector2Int.Distance(targetTilePos, currentTilePos) <= 1)
        {
            if (MapManager.Instance.IsThereTileOnMap(currentTilePos) == false && MapManager.Instance.IsThereTileOnMap(targetTilePos) == false)
                return true;
        }

        //Debug.Log("Direction" + dirToCheck);
        //Debug.Log("Original Position : " + currentTilePos);

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

        //Debug.Log("Check Position" + currentTilePos);


        if (MapManager.Instance.IsThereTileOnMap(currentTilePos) == false)
            return true;


        return false;
    }

    /// <summary>
    /// Gets the direction of the target object
    /// If the prey has to run, it will run in the opposite direction
    /// </summary>
    /// <returns>Returns the direction of the object</returns>
    public virtual DIRECTIONS GetTargetDirection()
    {
        // Get the two positions we need
        Vector2 TargetPos = targetObject.transform.position;
        Vector2 CurrentPos = transform.position;

        // Get the direction of the target
        Vector2 Dir = (TargetPos - CurrentPos).normalized;

        //Debug.Log(targetDir);
        // Debug.Log(Dir);

        // Checking for Up and Down first
        // if we're checking for up and down 
        // the X dir will be within a small buffer
        // 0.3f is just a number i used as a buffer
        // anything more than that means its on a diagonal
        // and player cant move in diagonal
        if (Dir.x <= 0.3f || Dir.x >= -0.3f)
        {
            // 0.5f is a buffer i used
            if (Dir.y > 0.5f)
            {
                return DIRECTIONS.UP;
            }
            else if (Dir.y < -0.5f)
            {
                return DIRECTIONS.DOWN;
            }
        }
        if (Dir.y <= 0.3f || Dir.y >= -0.3f)
        {
            if (Dir.x > 0.5f)
            {
                return DIRECTIONS.RIGHT;
            }
            else if (Dir.x < -0.5f)
            {
                return DIRECTIONS.LEFT;
            }
        }


        return DIRECTIONS.NONE;
    }

}
