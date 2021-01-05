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

    public void MoveKitten(Detection.DIRECTIONS dir, Detection.DIRECTIONS targetDir)
    {

    }
}
