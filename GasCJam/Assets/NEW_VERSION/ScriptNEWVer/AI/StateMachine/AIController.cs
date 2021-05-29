using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [System.NonSerialized]
    public StateMachine sm;

    [Header("AI State Configuration")]
    [Tooltip("List of the state names related to the AI")]
    public string[] stateNames;


    // Any other components for the AI
    // rigidbody, animation, etc
    // any variables to control the NPC like speed, etc

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
