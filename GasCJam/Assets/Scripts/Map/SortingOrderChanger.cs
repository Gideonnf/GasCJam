using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingOrderChanger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // Get the sprite renderer
            SpriteRenderer catRenderer = collision.gameObject.GetComponentInChildren<SpriteRenderer>();

            // set the sorting order
            catRenderer.sortingOrder = 50;


        }
        //else if (collision.gameObject.tag == "SortingColliders")
        //{
        //    Debug.Log("Colliding 2");
        //}
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // Get the sprite renderer
            SpriteRenderer catRenderer = collision.gameObject.GetComponentInChildren<SpriteRenderer>();

            // Reset the sorting order
            catRenderer.sortingOrder = 0;

        }
        //else if (collision.gameObject.tag == "SortingColliders")
        //{
        //    Debug.Log("Colliding 4");
        //}

    }
}
