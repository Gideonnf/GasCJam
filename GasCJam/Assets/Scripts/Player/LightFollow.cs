using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFollow : MonoBehaviour
{
    GameObject playerObject;

    // Start is called before the first frame update
    void Start()
    {
        // get the player object
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (playerObject == null)
            return;

        // move the light with the player
        transform.position = playerObject.transform.position;
 
    }
}
