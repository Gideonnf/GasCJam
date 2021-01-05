using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detection : MonoBehaviour
{
    [Header("Detection settings")]
    [Tooltip("Detection Radius")]
    [SerializeField] float CircleRadius = 1;

    [Tooltip("Line of Sight Distance?")]
    [SerializeField] float SightDistance;

    protected List<GameObject> ObjectsInRange = new List<GameObject>();

    public enum DIRECTIONS
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        NONE
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
            // Add it into the list if it isnt the environment
            // the only objects that would end up here are player, kitten, rats
            if (collider.gameObject.tag != "Environment")
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

    //TODO: idk how do this yet
    // i'll cross this bridge when i get there
    public virtual bool DetectInView()
    {


        return false;
    }


}
