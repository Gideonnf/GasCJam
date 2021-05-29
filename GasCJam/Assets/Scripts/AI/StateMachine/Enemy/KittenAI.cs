using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

/// <summary>
/// Each class state inherits from State base class
/// Over ride the main 3 functions that each state has
/// Enter will only be called once when it changes state
/// Update will be called as long as it remains in that state
/// Exit will becalled once when its changing state
/// 
/// </summary>



public class KittenAI_ChaseState : State
{
    // keep track of the game object
    GameObject go_;
    // Constructor
    public KittenAI_ChaseState(string m_stateID, GameObject _go) : base(m_stateID) { go_ = _go; }
    
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class KittenAI_IdleState : State
{
    // Keep track of the game object
    GameObject go_;

    public KittenAI_IdleState(string m_stateID, GameObject _go) :  base(m_stateID) { go_ = _go; }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class KittenAI_DeadState : State
{
    // Keep track of GO
    GameObject go_;

    public KittenAI_DeadState(string m_stateID, GameObject _go) : base (m_stateID) { go_ = _go; }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
    }
}