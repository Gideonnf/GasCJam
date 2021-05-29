using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAI_ChaseState : State
{
    GameObject go_;

    // Constructor
    public MouseAI_ChaseState(string m_stateID, GameObject _go) : base(m_stateID) { go_ = _go; }

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

public class MouseAI_IdleState : State
{
    GameObject go_;

    // Constructor
    public MouseAI_IdleState(string m_stateID, GameObject _go) : base(m_stateID) { go_ = _go; }

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

public class MouseAI_DeadState : State
{
    GameObject go_;

    // Constructor
    public MouseAI_DeadState(string m_stateID, GameObject _go) : base(m_stateID) { go_ = _go; }

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
