using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    public string m_stateID_;

    public State(string _m_stateID)
    {
        m_stateID_ = _m_stateID;
    }

    // Start is called before the first frame update
    public virtual void Enter()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }

    public virtual void Exit()
    {

    }

    //protected string GetStateID()
    //{

    //}
}
